namespace EmployeeManagement.Models
{
    public class QueueMessage
    {
        public string eventName { get; set; }
        public string orderId { get; set; }
        public string message { get; set; }
        public string queuename { get; set; }
        public string exchangename { get; set; }
        public string routingkey { get; set; }

    }
}
