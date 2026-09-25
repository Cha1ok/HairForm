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
                return new ErrorContainer("Нельзя больше 3 аксесуаров");
            if (order.HairCount == 0)
                return new ErrorContainer("Нельзя 0 причёсок");
            if (order.DateTime < DateTime.UtcNow)
                return new ErrorContainer("Ошибка в дате");
            

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

            // 1. Все пятницы и субботы на год вперёд (они всегда недоступны)
            var today = DateTime.Today;
            for (int i = 0; i < 365; i++)
            {
                var date = today.AddDays(i);
                if (date.DayOfWeek == DayOfWeek.Friday || date.DayOfWeek == DayOfWeek.Saturday)
                {
                    blockedDates.Add(date.ToString("yyyy-MM-dd"));
                }
            }

            // 2. Даты, занятые существующими заказами (только рабочие дни: вс, пн, вт, ср, чт)
            foreach (var order in orders)
            {
                var orderDate = order.DateTime.ToLocalTime().Date;

                // Определяем необходимое количество рабочих дней для блокировки
                int requiredWorkingDays = 1; // для 1 причёски без аксессуаров
                if (order.HairCount >= 2)
                    requiredWorkingDays = 7;
                else if (order.HairCount == 1)
                    requiredWorkingDays = 3;

                // Получаем список рабочих дней, начиная с даты заказа, пропуская пятницы и субботы
                var workingDates = GetWorkingDaysBlock(orderDate, requiredWorkingDays);
                foreach (var date in workingDates)
                {
                    blockedDates.Add(date.ToString("yyyy-MM-dd"));
                }
            }

            return blockedDates.ToList();
        }

        // Вспомогательный метод: возвращает заданное количество рабочих дней (не пт/сб), начиная с startDate
        private List<DateTime> GetWorkingDaysBlock(DateTime startDate, int workingDaysCount)
        {
            var blocked = new List<DateTime>();
            var current = startDate.Date;
            int added = 0;

            while (added < workingDaysCount)
            {
                // Пятница и суббота исключаются из подсчёта рабочих дней
                if (current.DayOfWeek != DayOfWeek.Friday && current.DayOfWeek != DayOfWeek.Saturday)
                {
                    blocked.Add(current);
                    added++;
                }
                current = current.AddDays(1);
            }

            return blocked;
        }
    }
}
