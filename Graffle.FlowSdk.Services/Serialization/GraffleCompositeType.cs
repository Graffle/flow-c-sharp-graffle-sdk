using Graffle.FlowSdk.Types;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Graffle.FlowSdk.Services
{
    public class GraffleCompositeType : FlowValueType //why does this derive from FlowValueType..
    {
        public GraffleCompositeType()
        {
        }
        public GraffleCompositeType(string type)
        {
            Type = type;
        }

        public override string Type { get; }
        public string Id { get; set; }

        public Dictionary<string, dynamic> Data { get; set; }

        [JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public CadenceSerializerVersion SerializerVersion { get; init; } = CadenceSerializerVersion.Legacy;

        public override string AsJsonCadenceDataFormat()
        {
            throw new NotImplementedException();
        }
        public override string DataAsJson()
        {
            throw new NotImplementedException();
        }
    }
}