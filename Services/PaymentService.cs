using EmployeeManagement.DataAccess;
using EmployeeManagement.Models;
using Microsoft.AspNetCore.Connections;
using RabbitMQ.Client;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Xml;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;

namespace EmployeeManagement.Services
{
    public interface IPaymentService
    {
        PaymentResult ProcessPayment(PaymentRequest paymentRequest);
        OrderResult ProcessOrder(OrderRequest orderRequest);
    }

    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;

        public PaymentService(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public PaymentResult ProcessPayment(PaymentRequest paymentRequest)
        {
            paymentRequest.RequestId = Guid.NewGuid().ToString();
            paymentRequest.EventName = paymentRequest.EventName; //"PROCESS_PAYMENT";

            // Simulate payment processing (e.g., integrate with a payment gateway)
            var transactionId = Guid.NewGuid().ToString(); // Simulate transaction ID

            // Save payment details to the database
            var isSaved = _paymentRepository.SavePayment(new Payment
            {
                UserName = paymentRequest.UserName,
                Amount = paymentRequest.Amount,
                PaymentMethod = paymentRequest.PaymentMethod,
                Notes = paymentRequest.Notes,
                TransactionId = transactionId
            });
            
            if (!isSaved)
            {
                return new PaymentResult
                {
                    IsSuccess = false,
                    ErrorMessage = "Failed to save payment details."
                };
            }

            //put into the Queue..PAYMENT after successfull payment 
            if (isSaved)
            {
                RabbitMQSend.SendMessageToRabbitMq(paymentRequest, "PaymentQueue", "OrderExchange", "order.created");
            }

            return new PaymentResult
            {
                IsSuccess = true,
                TransactionId = transactionId
            };
        }

        public int GenerateRandomNo()
        {
            int _min = 1000;
            int _max = 9999;
            Random _rdm = new Random();
            return _rdm.Next(_min, _max);
        }

        //Creating order and putting into the queue
        public OrderResult ProcessOrder(OrderRequest orderRequest)
        {
            orderRequest.requestId = Guid.NewGuid().ToString();
            orderRequest.orderId= GenerateRandomNo().ToString();
            orderRequest.timeStamp=DateTime.Now.ToString();
            List<Items> lst=new List<Items>();

            Items itemOne=new Items();
            itemOne.itemID = "book1";
            itemOne.quantity = 2;
            lst.Add(itemOne);

            Items itemTwo = new Items();
            itemTwo.itemID = "book2";
            itemTwo.quantity = 3;
            lst.Add(itemTwo);
            orderRequest.ItemsList = lst.ToArray();
            orderRequest.eventName = "ORDER_CREATED";
            orderRequest.totalAmount = 55;


            // Save payment details to the database
            var isSaved = _paymentRepository.CreateOrder(new OrderRequest
            {
                orderId = orderRequest.orderId,
                requestId = orderRequest.requestId,
                customerId = "107935",
                eventName = orderRequest.eventName,
                ItemsList = orderRequest.ItemsList,
                totalAmount=orderRequest.totalAmount,
                timeStamp=DateTime.Now.ToString()
            });

            if (!isSaved)
            {
                return new OrderResult
                {
                    IsSuccess = false,
                    ErrorMessage = "Failed to save ProcessOrder."
                };
            }

            //put the order into the queue.
            if (isSaved)
            {
                RabbitMQSend.SendMessageToRabbitMq(orderRequest, "OrderQueue", "OrderExchange", "order.created");
            }

            return new OrderResult
            {
                IsSuccess = true,
                TransactionId = Guid.NewGuid().ToString()
            };
        }


        //var factory = new ConnectionFactory()
        //    {
        //        // server
        //        HostName = RabbitQueue.Item1,
        //        // user
        //        UserName = RabbitQueue.Item2,
        //        // pass
        //        Password = RabbitQueue.Item3,
        //        // port
        //        Port = RabbitQueue.Item4,
        //        // virtualHost is case-sensitive
        //        VirtualHost = RabbitQueue.Item5
        //    };

        //    try
        //    {
        //        using (var connection = factory.CreateConnection())
        //        {
        //            using (IModel channel = connection.CreateModel())
        //            {
        //                channel.QueueDeclare(queue: queueName,
        //                                     durable: true,
        //                                     exclusive: false,
        //                                     autoDelete: false,
        //                                     arguments: arguments);

        //                var body = System.Text.Encoding.UTF8.GetBytes(messageData);
        //                IBasicProperties properties = channel.CreateBasicProperties();
        //                properties.ContentType = "text/plain";
        //                properties.DeliveryMode = 2;
        //                properties.Headers = new Dictionary<string, object>();
        //                properties.Persistent = true;

        //                Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        //                properties.Timestamp = new RabbitMQ.Client.AmqpTimestamp(unixTimestamp);
        //                properties.Headers.Add("Sent", DateTime.Now.ToString());
        //                properties.Headers.Add("Label", label);
        //                properties.Headers.Add("Server", Environment.MachineName);
        //                properties.Headers.Add("Application", System.Configuration.ConfigurationManager.AppSettings["ApplicationName"]);

        //                channel.BasicPublish(exchange: "",
        //                                     routingKey: queueName,
        //                                     mandatory: false,
        //                                     basicProperties: properties,
        //                                     body: body);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //ClientService.Logger.Error($"Exception occured in {MethodBase.GetCurrentMethod().Name} of file:{MethodBase.GetCurrentMethod().DeclaringType.Name},RabbitMQ failure - Exception:{ex.Message}, Url:{factory.Uri}, data:{messageData}, \n Stacktrace: {ex.StackTrace} ");
        //        //Console.WriteLine($"{ex.Message}, Url:{factory.Uri}, data:{messageData}");
        //        //return false;
        //    }

        //ClientService.Logger.Info($" {writeLock}, \"WorkOrders\", \"RabbitMQ -SendMessage\",  {messageData}, \"Client.WorkOrderMessage.cs ");

        //return true;
    }


    public class RabbitMQSend
    {

        public static void SendMessageToRabbitMq(object paymentRequest, string queueName, String exchangeName, string routingKey)
        {
            string jsonRequest = JsonConvert.SerializeObject(paymentRequest, Formatting.Indented);

            string label = $"Work Order:  order Sent: {DateTime.Now}";

            //var rabbitMQ = GetRabbitMQ_WorkOrderQueue();
            var arguments = new Dictionary<string, object>
                        {
                            { "x-queue-type", "classic" }
                        };
            var factory = new ConnectionFactory()
            {
                HostName = "localhost", // RabbitMQ server hostname or IP
                UserName = "guest",    // Default username
                Password = "guest",     // Default password
                VirtualHost="sapient"
            };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {

                    //Declare the Exchange...
                    //string exchangeName = "sapient_direct_exchange";
                    channel.ExchangeDeclare(exchange: exchangeName, type: "direct");


                    // Declare a queue (idempotent operation)
                   // string queueName = "work_order_to_Sapient";
                    channel.QueueDeclare(
                        queue: queueName,
                        durable: true,  // Persistent queue
                        exclusive: false,
                        autoDelete: false,
                        arguments: arguments);


                    //Define a routing key...
                  // Bind the queue to the exchange with a routing key
                    //string routingKey = "sapient_routing_key";
                    channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: routingKey);



                    // Message to send
                    var body = System.Text.Encoding.UTF8.GetBytes(jsonRequest);   //Encoding.UTF8.GetBytes(message);

                    IBasicProperties properties = channel.CreateBasicProperties();
                    properties.ContentType = "text/plain";
                    properties.DeliveryMode = 2;
                    properties.Headers = new Dictionary<string, object>();
                    properties.Persistent = true;

                    Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                    properties.Timestamp = new RabbitMQ.Client.AmqpTimestamp(unixTimestamp);
                    properties.Headers.Add("Sent", DateTime.Now.ToString());
                    properties.Headers.Add("Label", label);
                    properties.Headers.Add("Server", Environment.MachineName);
                    properties.Headers.Add("Application", "EmployeeSystem");


                    // Publish the message to the queue
                    channel.BasicPublish(
                        exchange: "",       // Default exchange
                        routingKey: queueName, // Queue name as the routing key
                        basicProperties: properties,
                        body: body);

                    //Console.WriteLine($"[x] Sent: {message}");
                }
            }
            

            Console.WriteLine("Press [enter] to exit.");
            Console.ReadLine();
        }

    }
}
