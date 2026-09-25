using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography.X509Certificates;

namespace HairForm.Models
{
    public class Order
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "Введите имя")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Выберите тип")]
        public MessageType Type { get; set; }
        [Required(ErrorMessage = "Введите ваш id")]
        public string MessageTypeId { get; set; }
        [Required(ErrorMessage = "введите число")]
        public int HairCount { get; set; }
        public ICollection<OrderAccessory> Accessories { get; set; } = new List<OrderAccessory>();
        public double Total { get; set; }
        public DateTime DateTime { get; set; }
        public OrderStatus Status { get; set; }
        public bool IsPaid { get; set; }
        [NotMapped]
        public User User { get; set; }
    }

    public class OrderAccessory
    {
        public int Id { get; set; }
        public string OrderId { get; set; }
        public Order Order { get; set; }
        public AccessoryType Type { get; set; }
        public int Quantity { get; set; }
    }

    public class BoockedDate
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }


    public enum OrderStatus
    {
        [Description("Новая")]
        New,

        [Description("В работе")]
        InProgress,

        [Description("Завершена")]
        Completed
    }

    public enum MessageType
    {
        Tumbler = 1,
        Boosty = 2,
        Telegram = 3
    }

    public enum AccessoryType
    {
        [Description("Маленький")]
        small = 1,
        [Description("Средний")]
        medium = 2,
        [Description("Большой")]
        large = 3,
    }
}
