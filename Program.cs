
using EmployeeManagement.DataAccess;
using EmployeeManagement.Middleware;
using EmployeeManagement.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace EmployeeManagement
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            //builder.Services.AddSwaggerGen();


            // Configure database
            //var connectionString = new Microsoft.Extensions.Configuration.ConfigurationManager.ConfigurationManagerDebugView(builder.Configuration).Items[11]
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            //var connectionString = builder.Configuration.GetSection("AppSettings.ConnectionString").Value;
            //new Microsoft.Extensions.Configuration.ConfigurationManager.ConfigurationManagerDebugView(builder.Configuration).Items[11]
            builder.Services.AddSingleton(new Repository(connectionString));
            builder.Services.AddTransient<EmployeeService>();



            var app = builder.Build();
            // Add custom error-handling middleware
            app.UseMiddleware<ErrorHandlingMiddleware>();



            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                //app.UseSwagger();
               // app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
