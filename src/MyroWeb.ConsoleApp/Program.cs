using Microsoft.Extensions.DependencyInjection;
using System;
using MyroWebClient.Extensions;
using MyroWebClient;
using MyroWebClient.Entities;

namespace MyroWeb.ConsoleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var services = ConfigureServices();
            var client = services.GetRequiredService<MyroDataService>();

            var test = client.GetAllMyroData(new User
            {
                UserName = "USERNAME",
                Password = "PASSWORD",
                SchoolAbreviation = "SCHOOLABR"
            });

            Console.WriteLine(test.Terms[0].Name);
        }

        private static ServiceProvider ConfigureServices()
        => new ServiceCollection()
            .AddMyroWebTypes()
            .BuildServiceProvider();
    }
}
