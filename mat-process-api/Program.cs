using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace mat_process_api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseSentry(Environment.GetEnvironmentVariable("SENTRY_URL"))
                .UseStartup<Startup>();
    }
}
