using Microsoft.Extensions.DependencyInjection;

namespace MyroWebClient.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMyroWebTypes(this IServiceCollection collection)
            => collection.AddSingleton<HttpService>()
            .AddSingleton<MyroDataService>()
            .AddSingleton<DataParser>()
            .AddSingleton<MyroDatabase>();
    }
}
