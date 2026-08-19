using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;

namespace HairForm.Models
{
    public class Order
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public MessageType Type { get; set; }
        public string MessageTypeId { get; set; }
        public int HairCount { get; set; }
        public ICollection<OrderAccessory> Accessories { get; set; } = new List<OrderAccessory>();
        public double Total { get; set; }
        public DateTime DateTime { get; set; }
        public OrderStatus Status { get; set; }
        public bool IsPaid { get; set; }
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
        small = 1,
        medium = 2,
        large = 3,
    }
}
