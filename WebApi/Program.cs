using Microsoft.EntityFrameworkCore;

using Application.Services;
using DataModel.Repository;
using DataModel.Mapper;
using Domain.Factory;
using Domain.IRepository;
using WebApi.Controllers;
using Gateway;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<AbsanteeContext>(opt =>
    //opt.UseInMemoryDatabase("AbsanteeList")
    //opt.UseSqlite("Data Source=AbsanteeDatabase.sqlite")
    opt.UseSqlite(Host.CreateApplicationBuilder().Configuration.GetConnectionString("AbsanteeDatabase"))
    );

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.MapControllers();

var rabbitMQConsumerService = app.Services.GetRequiredService<IRabbitMQConsumerController>();
var rabbitMQColabConsumerService = app.Services.GetRequiredService<IRabbitMQColabConsumerController>();
var rabbitMQHolidayWithPeriodConsumerService = app.Services.GetRequiredService<IRabbitMQHolidayWithPeriodConsumerController>();
rabbitMQConsumerService.StartConsuming();
rabbitMQColabConsumerService.StartConsuming();
rabbitMQHolidayWithPeriodConsumerService.StartConsuming();

app.Run();
