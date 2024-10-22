using Graffle.FlowSdk.Services.Nodes;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Graffle.FlowSdk.Services
{
    public static class FlowServiceExtensions
    {
        [Obsolete("Manually inject FlowClientFactory")]
        public static IServiceCollection AddFlowClientFactory(this IServiceCollection services, bool useCrescendoSerializerForAllSporks = true)
        {
            Console.WriteLine("Set Up Flow Client Factory");
            var nodeName = System.Environment.GetEnvironmentVariable("FlowNode");
            var spork = Sporks.GetSporkByName(nodeName);

            Console.WriteLine($"Spork Selected: {spork.Name}");

            var flowClientFactory = new FlowClientFactory(nodeName) { UseCrescendoSerializerForAllSporks = useCrescendoSerializerForAllSporks };
            services.AddSingleton(flowClientFactory); //todo remove this eventually?
            services.AddSingleton(typeof(IFlowClientFactory), flowClientFactory);

            Console.WriteLine("Set Up Flow Client Factory Complete");
            return services;
        }
    }
}