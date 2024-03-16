
using WebCrawler.Services;
using WebCrawler.Tree;

namespace WebCrawler
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            // Register the IHttpClientFactory
            builder.Services.AddHttpClient();
            builder.Services.AddScoped<IHtmlParser, HtmlParser>();
            builder.Services.AddScoped<CrawlerService>();
            builder.Services.AddScoped<HtmlTreeSearch>();

            // Configure logging with default providers
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole(); // Add console logging
            builder.Logging.AddDebug();   // Add debug logging

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

            app.Run();
        }
    }
}
