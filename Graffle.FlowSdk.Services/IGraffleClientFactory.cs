using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Graffle.FlowSdk.Services
{
    public interface IGraffleClientFactory
    {
        IGraffleClient Create();
        IGraffleClient Create(ulong blockHeight);
        IGraffleClient Create(string accessNodeUri);
    }
}