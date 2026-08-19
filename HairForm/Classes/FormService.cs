using HairForm.Database;
using HairForm.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HairForm.Classes
{
    public class FormService
    {
        private readonly ApplicationDbContext _database;
        public FormService(ApplicationDbContext database)
        {
            _database = database;
        }

        public object FromCreate(Order order, Dictionary<AccessoryType, int> accessories)
        {
            if (order == null)
                return new ErrorContainer("Пустая форма");
            if (order.Accessories.Any(x => x.Quantity > 3))
                return new ErrorContainer("Нельяз больше 3 аксесуаров");

            // Очищаем коллекцию на всякий случай
            order.Id = Guid.NewGuid().ToString();
            order.Accessories = new List<OrderAccessory>();
            order.DateTime = DateTime.SpecifyKind(order.DateTime, DateTimeKind.Utc);
            order.Status = OrderStatus.New;
            order.IsPaid = false;

            // Добавляем только выбранные аксессуары (количество > 0)
            if (accessories != null)
            {
                foreach (var kv in accessories.Where(kv => kv.Value > 0))
                {
                    order.Accessories.Add(new OrderAccessory
                    {
                        Type = kv.Key,
                        Quantity = kv.Value
                    });
                }
            }

            _database.Orders.Add(order);
            _database.SaveChanges();
            return order;
        }

        public async Task<object> GetBookedDates()
        {
            var orders = await _database.Orders
                .Include(x => x.Accessories)
                .ToListAsync();

            var blockedDates = new HashSet<string>();

            // 1. Все пятницы и субботы на год вперёд
            var today = DateTime.Today;
            for (int i = 0; i < 365; i++)
            {
                var date = today.AddDays(i);
                if (date.DayOfWeek == DayOfWeek.Friday || date.DayOfWeek == DayOfWeek.Saturday)
                {
                    blockedDates.Add(date.ToString("yyyy-MM-dd"));
                }
            }

            // 2. Даты, занятые существующими заказами
            foreach (var order in orders)
            {
                var orderDate = order.DateTime.ToLocalTime().Date; // только дата без времени

                // Сама дата заказа
                blockedDates.Add(orderDate.ToString("yyyy-MM-dd"));

                // Если 2 и более причёсок – блокируем неделю (7 дней)
                if (order.HairCount >= 2)
                {
                    for (int i = 0; i < 7; i++)
                    {
                        blockedDates.Add(orderDate.AddDays(i).ToString("yyyy-MM-dd"));
                    }
                }
                // Если 1 причёска и есть аксессуары – блокируем 3 дня
                else if (order.HairCount == 1 && order.Accessories != null && order.Accessories.Any())
                {
                    for (int i = 0; i < 3; i++)
                    {
                        blockedDates.Add(orderDate.AddDays(i).ToString("yyyy-MM-dd"));
                    }
                }
                // Для 1 причёски без аксессуаров дополнительных дней не добавляем
            }

            return blockedDates.ToList();
        }
    }
}
