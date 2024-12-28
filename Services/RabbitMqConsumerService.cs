using Microsoft.Extensions.Options;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using EmployeeManagement.Models;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using EmployeeManagement.DataAccess;

namespace EmployeeManagement.Services
{
    public class RabbitMqConsumerService : BackgroundService
    {
        private readonly string _connectionString;

        private readonly ILogger<RabbitMqConsumerService> _logger;
        private readonly RabbitMqSettings _settings;
        private IConnection _connection;
        private IModel _channel;


        public RabbitMqConsumerService(string connectionString)
        {
            _connectionString = connectionString;

        }

        public RabbitMqConsumerService(IOptions<RabbitMqSettings> options, ILogger<RabbitMqConsumerService> logger)
        {
            _logger = logger;
            _settings = options.Value;



            // Create RabbitMQ connection and channel
            var factory = new ConnectionFactory()
            {
                HostName = _settings.HostName,
                UserName = _settings.UserName,
                Password = _settings.Password,
                VirtualHost = _settings.VirtualHost
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("RabbitMQ Consumer Service is starting.");

            // Declare exchange and queue
            _channel.ExchangeDeclare(exchange: _settings.ExchangeName, type: ExchangeType.Direct);
            _channel.QueueDeclare(queue: _settings.QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

            //_channel.QueueBind(queue: _settings.QueueName, exchange: _settings.ExchangeName, routingKey: _settings.RoutingKey);
            _channel.QueueBind(queue: "OrderQueue", exchange: "OrderExchange", routingKey: "order.created");

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                _logger.LogInformation($"Received message: {message}");

                try
                {
                    // Process the message (implement your business logic here)
                    QueueMessage queueMessage = new QueueMessage();
                    queueMessage.queuename= _settings.QueueName;
                    queueMessage.exchangename= _settings.ExchangeName;
                   queueMessage.message= message;
                    queueMessage.routingkey= _settings.RoutingKey;

                    ProcessMessage(queueMessage);

                    // Acknowledge the message to queue that message has been delivered.
                    _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);

                    _logger.LogInformation("Message acknowledged.");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error processing message: {ex.Message}");
                    // Optionally reject the message (requeue or send to a dead-letter queue)
                    _channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false);
                }
            };

            _channel.BasicConsume(queue: _settings.QueueName, autoAck: false, consumer: consumer);

            return Task.CompletedTask;
        }

        private void ProcessMessage(QueueMessage queueMessage)
        {
            //queueMessage.eventName
            AddMessagesToQueueAsync(queueMessage);

            // Add your custom business logic here
            _logger.LogInformation($"Processing message: {queueMessage.message}");
        }


        public async Task AddMessagesToQueueAsync(QueueMessage queueMessage)
        {
            using (var connection = new SqlConnection("data source=VINAY\\SQLEXPRESS;database=EmployeeManagementPortal; Persist Security Info=True;Integrated Security=true; Application Name=EmployeePortal"))
            {
                using (var command = new SqlCommand("AddMessageToMq", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@message", queueMessage.message);
                    command.Parameters.AddWithValue("@queuename", queueMessage.queuename);
                    command.Parameters.AddWithValue("@exchangename", queueMessage.exchangename);
                    command.Parameters.AddWithValue("@routingkey", queueMessage.routingkey);
                    connection.Open();
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public override void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            base.Dispose();
        }
    }
}
