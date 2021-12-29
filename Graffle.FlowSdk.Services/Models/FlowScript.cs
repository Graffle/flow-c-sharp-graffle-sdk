namespace Graffle.FlowSdk.Services.Models
{
    public class FlowScript
    {
        public FlowScript(string rawScript)
        {
            RawScript = rawScript;
            ScriptHash = rawScript.GetHashString();
        }

        public string RawScript { get; }

        //TODO: This needs to remove variables and standardize them
        public string ScriptHash { get; }

    }
}