using Common.Logging;
using Contracts.Sagas.OrderManager;
using Infrastructure.Extensions;
using Infrastructure.Policies;
using Saga.Orchestrator.HttpRepository;
using Saga.Orchestrator.HttpRepository.Interfaces;
using Saga.Orchestrator.OrderManager;
using Saga.Orchestrator.Services;
using Saga.Orchestrator.Services.Interfaces;
using Shared.Configurations;
using Shared.DTOs.Basket;

namespace Saga.Orchestrator.Extensions;

public static class ServiceExtensions
{
    internal static IServiceCollection AddConfigurationSettings(this IServiceCollection services,
        IConfiguration configuration)
    {
        return services.AddSingleton(x => configuration.GetSection(nameof(ServiceUrls)));
    }

    public static IServiceCollection ConfigureServices(this IServiceCollection services)
    {
        return services.AddTransient<ICheckoutSagaService, CheckoutSagaService>()
            .AddTransient<ISagaOrderManager<BasketCheckoutDto, OrderResponse>, SagaOrderManager>()
            .AddTransient<LoggingDelegatingHandler>();
    }

    public static IServiceCollection ConfigureHttpRepository(this IServiceCollection services)
    {
        return services.AddScoped<IOrderHttpRepository, OrderHttpRepository>()
            .AddScoped<IBasketHttpRepository, BasketHttpRepository>()
            .AddScoped<IInventoryHttpRepository, InventoryHttpRepository>();
    }

    public static void ConfigureHttpClients(this IServiceCollection services)
    {
        ConfigureOrderHttpClient(services);
        ConfigureBasketHttpClient(services);
        ConfigureInventoryHttpClient(services);
    }

    private static void ConfigureOrderHttpClient(this IServiceCollection services)
    {
        var urls = services.GetOptions<ServiceUrls>(nameof(ServiceUrls));
        if (urls == null || string.IsNullOrEmpty(urls.Orders))
            throw new ArgumentNullException("ServiceUrls Orders is not configured");

        services.AddHttpClient<IOrderHttpRepository, OrderHttpRepository>("OrdersAPI",
                (sp, cl) => { cl.BaseAddress = new Uri($"{urls.Orders}/api/v1/"); })
            .AddHttpMessageHandler<LoggingDelegatingHandler>()
            .UseExponentialHttpRetryPolicy();
        services.AddScoped(sp => sp.GetService<IHttpClientFactory>()
            .CreateClient("OrdersAPI"));
    }

    private static void ConfigureBasketHttpClient(this IServiceCollection services)
    {
        var urls = services.GetOptions<ServiceUrls>(nameof(ServiceUrls));
        if (urls == null || string.IsNullOrEmpty(urls.Basket))
            throw new ArgumentNullException("ServiceUrls Basket is not configured");

        services.AddHttpClient<IBasketHttpRepository, BasketHttpRepository>("BasketsAPI",
                (sp, cl) => { cl.BaseAddress = new Uri($"{urls.Basket}/api/"); })
            .AddHttpMessageHandler<LoggingDelegatingHandler>()
            .UseImmediateHttpRetryPolicy();
        services.AddScoped(sp => sp.GetService<IHttpClientFactory>()
            .CreateClient("BasketsAPI"));
    }

    private static void ConfigureInventoryHttpClient(this IServiceCollection services)
    {
        var urls = services.GetOptions<ServiceUrls>(nameof(ServiceUrls));
        if (urls == null || string.IsNullOrEmpty(urls.Inventory))
            throw new ArgumentNullException("ServiceUrls Inventory is not configured");

        services.AddHttpClient<IInventoryHttpRepository, InventoryHttpRepository>("InventoryAPI",
                (sp, cl) => { cl.BaseAddress = new Uri($"{urls.Inventory}/api/"); })
            .AddHttpMessageHandler<LoggingDelegatingHandler>()
            .UseExponentialHttpRetryPolicy();
        services.AddScoped(sp => sp.GetService<IHttpClientFactory>()
            .CreateClient("InventoryAPI"));
    }
}