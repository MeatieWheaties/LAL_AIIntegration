using LunchAndLearn_AIIntegration.AI;
using LunchAndLearn_AIIntegration.Data;
using LunchAndLearn_AIIntegration.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using OpenAI.Chat;
using OpenAI.Responses;
using System;

namespace LunchAndLearn_AIIntegration.Startup
{
    public static class ServiceRegistry
    {
        public static void RegisterServices(this WebApplicationBuilder _builder)
        {
            _builder.Services.AddMudServices();

            //_builder.Services.AddSingleton(sp =>
            //{
            //    return new ResponsesClient(
            //        _builder.Configuration["OpenAI:Model"],
            //        _builder.Configuration["OpenAI:ApiKey"]);
            //});

            _builder.Services.AddSingleton(sp =>
            {
                var cfg = sp.GetRequiredService<IConfiguration>();
                var apiKey = cfg["OpenAI:ApiKey"];
                var model = cfg["OpenAI:Model"]; // e.g. "gpt-4.1-mini" etc.

                return new ChatClient(model, apiKey);
            });

            _builder.Services.AddScoped<AiService>();

            //_builder.Services.AddDbContext<DB>(options =>
            //    options.UseSqlite(_builder.Configuration.GetConnectionString("AppDb")));

            _builder.Services.AddDbContextFactory<DB>(options =>
            {
                var dbPath = Path.Combine(_builder.Environment.ContentRootPath, "app.db");
                options.UseSqlite($"Data Source={dbPath}");
            });

            _builder.Services.AddTransient<AiService>();
            _builder.Services.AddTransient<AIRepository>();
            _builder.Services.AddTransient<KoboldRepository>();
            _builder.Services.AddTransient<ItemRepository>();

        }
    }
}
