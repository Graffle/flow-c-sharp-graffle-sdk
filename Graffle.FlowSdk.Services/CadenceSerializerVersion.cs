namespace Graffle.FlowSdk.Services;

public enum CadenceSerializerVersion : int
{
    /// <summary>
    /// Legacy cadence json deserialization, GraffleCompositeTypeConverter
    /// </summary>
    Legacy = 0,

    /// <summary>
    /// Updated cadence json deserialization, CadenceJsonInterpreter
    /// </summary>
    Crescendo
}
