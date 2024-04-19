using RabbitMQ.Client;

using Application.DTO;
using RabbitMQ.Client.Events;
using System.Text;
using Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace WebApi.Controllers
{
    public class RabbitMQColabConsumerController : IRabbitMQColabConsumerController
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ConnectionFactory _factory;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        private List<string> _errorMessages = new List<string>();

        private string queueName;
 
        public RabbitMQColabConsumerController(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
            _factory = new ConnectionFactory { HostName = "localhost" };
            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(exchange: "colab_logs", type: ExchangeType.Fanout);
           
            queueName = _channel.QueueDeclare().QueueName;

            _channel.QueueBind(queue: queueName, exchange: "colab_logs", routingKey: "colabKey");
 
            Console.WriteLine(" [*] Waiting for messages.");

            
        }

 
        public void StartConsuming()
        {
            Console.WriteLine("colabs");
            
            var consumer = new EventingBasicConsumer(_channel);
            
            consumer.Received += async (model, ea) =>
            {
                byte[] body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                //_colaboratorIdService.Add(colaborador);
                var colabResult = JsonConvert.DeserializeObject<long>(message);
                var colaboratorIDDTO = new ColaboratorIdDTO
                {
                    _colabId =colabResult,
                };
           
 
 
                using (var scope = _scopeFactory.CreateScope()){
                var colaboratorIdService = scope.ServiceProvider.GetRequiredService<ColaboratorIdService>();
                await colaboratorIdService.Add(colaboratorIDDTO, _errorMessages);
                };
            };
            
            _channel.BasicConsume(queue: queueName,
                                autoAck: true,
                                consumer: consumer);
        }
 
        public void StopConsuming()
        {
            throw new NotImplementedException();
        }
    }
}