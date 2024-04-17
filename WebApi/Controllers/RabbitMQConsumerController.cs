using RabbitMQ.Client;

using Application.DTO;
using RabbitMQ.Client.Events;
using System.Text;
using Application.Services;
using Newtonsoft.Json;
 
namespace WebApi.Controllers
{
    public class RabbitMQConsumerController : IRabbitMQConsumerController
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ConnectionFactory _factory;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private string queueName;
        private List<string> _errorMessages = new List<string>();
 
        public RabbitMQConsumerController(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
            _factory = new ConnectionFactory { HostName = "localhost" };
            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();
 
            _channel.ExchangeDeclare(exchange: "holiday_logs", type: ExchangeType.Fanout);
           
            queueName = _channel.QueueDeclare().QueueName;
 
            _channel.QueueBind(queue: queueName, exchange: "holiday_logs", routingKey: "holidayKey");
 
            Console.WriteLine(" [*] Waiting for messages.");
        }

        public void StartConsuming()
        {
            Console.WriteLine("holidays");
           
            var consumer = new EventingBasicConsumer(_channel);
           
            consumer.Received += async (model, ea) =>
            {
                byte[] body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                //_colaboratorIdService.Add(colaborador);
                var holidayResult = JsonConvert.DeserializeObject<HolidayDTO>(message);
                var holidayDTO = new HolidayDTO
                {
                    Id = holidayResult.Id,
                    _colabId = holidayResult._colabId,
                    _holidayPeriods = holidayResult._holidayPeriods
                };
           
 
 
                using (var scope = _scopeFactory.CreateScope()){
                var holidayService = scope.ServiceProvider.GetRequiredService<HolidayService>();
                await holidayService.Add(holidayDTO, _errorMessages);
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