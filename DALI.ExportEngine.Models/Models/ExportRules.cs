using Newtonsoft.Json;

namespace DALI.PolicyRequirements.DomainModels
{
    public class ExportRules
    {
        public const string ObjectsAndPolicyRules = "ObjectsAndPolicyRules";
        public const string Severity = "Severity";
        public const string Level = "Level";

        [JsonProperty("headerName")]
        public string HeaderName { get; set; }

        [JsonProperty("headerValue")]
        public string HeaderValue { get; set; }

        [JsonProperty("emptyValues")]
        public bool EmptyValues { get; set; }
    }
}
