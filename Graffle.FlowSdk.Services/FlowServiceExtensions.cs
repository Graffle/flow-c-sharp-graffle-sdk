using System;
using Graffle.FlowSdk;
using Graffle.FlowSdk.Services;
using Graffle.FlowSdk.Services.Nodes;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class FlowServiceExtensions
    {
        public static IServiceCollection AddFlowClientFactory(this IServiceCollection services)
        {
            Console.WriteLine("Set Up Flow Client Factory");
            var nodeName = System.Environment.GetEnvironmentVariable("FlowNode");
            var spork = Sporks.GetSporkByName(nodeName);

            Console.WriteLine($"Spork Selected: {spork.Name}");

            var flowClientFactory = new FlowClientFactory(nodeName);
            services.AddSingleton(flowClientFactory); //todo remove this eventually?
            services.AddSingleton(typeof(IFlowClientFactory), flowClientFactory);

            Console.WriteLine("Set Up Flow Client Factory Complete");
            return services;
        }

        public static IServiceCollection AddGraffleClientFactory(this IServiceCollection services, string flowNode)
        {
            var factory = new GraffleClientFactory(flowNode);
            services.AddSingleton(typeof(IGraffleClientFactory), factory);
            return services;
        }
    }
}