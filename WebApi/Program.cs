using Microsoft.EntityFrameworkCore;

using Application.Services;
using DataModel.Repository;
using DataModel.Mapper;
using Domain.Factory;
using Domain.IRepository;
using WebApi.Controllers;
using Gateway;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Any;


var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;
var holidayQueueName = config["HolidayQueues:" + args[0]];
var colaboratorQueueName = config["ColaboratorQueues:" + args[0]];
var holidayPeriodQueueName = config["HolidayPeriodQueues:" + args[0]];
var connection = config["ConnectionStrings:" + args[0]];

var port = getPort(holidayQueueName);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<AbsanteeContext>(opt =>
    //opt.UseInMemoryDatabase("AbsanteeList")
    //opt.UseSqlite("Data Source=AbsanteeDatabase.sqlite")
    opt.UseSqlite(Host.CreateApplicationBuilder().Configuration.GetConnectionString(args[0]))
    );

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
    opt.MapType<DateOnly>(() => new OpenApiSchema
    {
        Type = "string",
        Format = "date",
        Example = new OpenApiString(DateTime.Today.ToString("yyyy-MM-dd"))
    })
);

builder.Services.AddTransient<IHolidayRepository, HolidayRepository>();
builder.Services.AddTransient<IHolidayFactory, HolidayFactory>();
builder.Services.AddTransient<HolidayMapper>();
builder.Services.AddTransient<HolidayService>();
builder.Services.AddTransient<HolidayAmpqGateway>();

builder.Services.AddTransient<IHolidayPeriodRepository, HolidayPeriodRepository>();
builder.Services.AddTransient<IHolidayPeriodFactory, HolidayPeriodFactory>();
builder.Services.AddTransient<HolidayPeriodMapper>();
builder.Services.AddTransient<HolidayPeriodService>();

builder.Services.AddSingleton<IRabbitMQHolidayWithPeriodConsumerController, RabbitMQHolidayWithPeriodConsumerController>();
builder.Services.AddSingleton<IRabbitMQConsumerController, RabbitMQConsumerController>();

builder.Services.AddTransient<IColaboratorsIdRepository, ColaboratorsIdRepository>();
builder.Services.AddTransient<IColaboratorIdFactory, ColaboratorIdFactory>();
builder.Services.AddTransient<ColaboratorsIdMapper>();
builder.Services.AddTransient<ColaboratorIdService>();
builder.Services.AddTransient<IRabbitMQColabConsumerController, RabbitMQColabConsumerController>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection(); 

app.UseAuthorization();



var rabbitMQConsumerService = app.Services.GetRequiredService<IRabbitMQConsumerController>();
var rabbitMQColabConsumerService = app.Services.GetRequiredService<IRabbitMQColabConsumerController>();
var rabbitMQHolidayWithPeriodConsumerService = app.Services.GetRequiredService<IRabbitMQHolidayWithPeriodConsumerController>();

rabbitMQColabConsumerService.ConfigQueue(colaboratorQueueName);
rabbitMQConsumerService.ConfigQueue(holidayQueueName);
rabbitMQHolidayWithPeriodConsumerService.ConfigQueue(holidayPeriodQueueName);

rabbitMQConsumerService.StartConsuming();
rabbitMQColabConsumerService.StartConsuming();
rabbitMQHolidayWithPeriodConsumerService.StartConsuming();

app.MapControllers();

app.Run($"https://localhost:{port}");

int getPort(string name)
{
    // Implement logic to map queue name to a unique port number
    // Example: Assign a unique port number based on the queue name suffix
    int basePort = 5010; // Start from port 5000
    int queueIndex = int.Parse(name.Substring(21)); // Extract the numeric part of the queue name (assuming it starts with 'Q')
    return basePort + queueIndex;
}
