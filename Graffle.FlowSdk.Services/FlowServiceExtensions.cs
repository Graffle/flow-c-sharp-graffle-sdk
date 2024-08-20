using System;
using Graffle.FlowSdk;
using Graffle.FlowSdk.Services;
using Graffle.FlowSdk.Services.Nodes;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class FlowServiceExtensions
    {
        public static IServiceCollection AddFlowClientFactory(this IServiceCollection services, CadenceSerializerVersion serializer = CadenceSerializerVersion.Expando)
        {
            Console.WriteLine("Set Up Flow Client Factory");
            var nodeName = System.Environment.GetEnvironmentVariable("FlowNode");
            var spork = Sporks.GetSporkByName(nodeName);

            Console.WriteLine($"Spork Selected: {spork.Name}");

            var flowClientFactory = new FlowClientFactory(nodeName) { CandeceSerializer = serializer };
            services.AddSingleton(flowClientFactory); //todo remove this eventually?
            services.AddSingleton(typeof(IFlowClientFactory), flowClientFactory);

            Console.WriteLine("Set Up Flow Client Factory Complete");
            return services;
        }
    }
}