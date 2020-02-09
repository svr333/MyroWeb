using Microsoft.Extensions.DependencyInjection;

namespace MyroWebClient.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMyroWebTypes(this IServiceCollection collection)
            => collection
            .AddTransient<HttpService>()
            .AddTransient<MyroDataService>()
            .AddTransient<DataParser>()
            .AddSingleton<MyroDatabase>();
    }
}
