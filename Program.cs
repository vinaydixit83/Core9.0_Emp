
using EmployeeManagement.DataAccess;
using EmployeeManagement.Middleware;
using EmployeeManagement.Models;
using EmployeeManagement.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;

namespace EmployeeManagement
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
 
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Payment API", Version = "v1" });
            });

            builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMQ"));
            builder.Services.AddHostedService<RabbitMqConsumerService>();


            // Configure database
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddSingleton(new Repository(connectionString));

            builder.Services.AddTransient<EmployeeService>();

            builder.Services.AddTransient<IPaymentRepository,PaymentRepository>();
            builder.Services.AddTransient<IPaymentService, PaymentService>();



            //builder.Services.AddTransient<RabbitMsgRepository>();

            var app = builder.Build();
            // Add custom error-handling middleware
            app.UseMiddleware<ErrorHandlingMiddleware>();
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Payment API v1"));
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
