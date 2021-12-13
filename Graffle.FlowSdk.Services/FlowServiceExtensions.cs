using System;
using Graffle.FlowSdk;
using Graffle.FlowSdk.Nodes;

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
            services.AddSingleton(flowClientFactory);
            Console.WriteLine("Set Up Flow Client Factory Complete");
            return services;
        }
    }
}