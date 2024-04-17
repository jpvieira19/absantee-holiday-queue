using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO;
using Application.Services;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace WebApi.Controllers
{
    public class RabbitMQHolidayWithPeriodConsumerController : IRabbitMQHolidayWithPeriodConsumerController
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ConnectionFactory _factory;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private string queueName;
        private List<string> _errorMessages = new List<string>();
 
        public RabbitMQHolidayWithPeriodConsumerController(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
            _factory = new ConnectionFactory { HostName = "localhost" };
            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();
 
            _channel.ExchangeDeclare(exchange: "holiday_WithHolidayPeriod_logs", type: ExchangeType.Fanout);
           
            queueName = _channel.QueueDeclare().QueueName;
 
            _channel.QueueBind(queue: queueName, exchange: "holiday_WithHolidayPeriod_logs", routingKey: "holiday_WithHolidayPeriod_Key");
 
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
                //await holidayService.UpdateHoliday(holidayDTO, _errorMessages);
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