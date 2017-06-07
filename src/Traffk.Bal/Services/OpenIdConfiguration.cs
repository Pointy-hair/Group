using Newtonsoft.Json;

namespace Traffk.Bal.Services
{
    public class OpenIdConfiguration
    {
        public static OpenIdConfiguration CreateFromJson(string json)
        {
            return TraffkHelpers.JsonConvertDeserializeObjectOrFallback<OpenIdConfiguration>(json);
        }

        public string ToJson() => JsonConvert.SerializeObject(this);

        [JsonProperty("authorization_endpoint")]
        public string authorization_endpoint { get; set; }

        [JsonProperty("token_endpoint")]
        public string token_endpoint { get; set; }

        [JsonProperty("token_endpoint_auth_methods_supported")]
        public string[] token_endpoint_auth_methods_supported { get; set; }

        [JsonProperty("jwks_uri")]
        public string jwks_uri { get; set; }

        [JsonProperty("response_modes_supported")]
        public string[] response_modes_supported { get; set; }

        [JsonProperty("subject_types_supported")]
        public string[] subject_types_supported { get; set; }

        [JsonProperty("id_token_signing_alg_values_supported")]
        public string[] id_token_signing_alg_values_supported { get; set; }

        [JsonProperty("http_logout_supported")]
        public bool http_logout_supported { get; set; }

        [JsonProperty("frontchannel_logout_supported")]
        public bool frontchannel_logout_supported { get; set; }

        [JsonProperty("end_session_endpoint")]
        public string end_session_endpoint { get; set; }

        [JsonProperty("response_types_supported")]
        public string[] response_types_supported { get; set; }

        [JsonProperty("scopes_supported")]
        public string[] scopes_supported { get; set; }

        [JsonProperty("issuer")]
        public string Issuer { get; set; }

        [JsonProperty("claims_supported")]
        public string[] claims_supported { get; set; }

        [JsonProperty("microsoft_multi_refresh_token")]
        public bool microsoft_multi_refresh_token { get; set; }

        [JsonProperty("check_session_iframe")]
        public string check_session_iframe { get; set; }

        [JsonProperty("userinfo_endpoint")]
        public string userinfo_endpoint { get; set; }

        [JsonProperty("tenant_region_scope")]
        public string tenant_region_scope { get; set; }

        [JsonProperty("cloud_instance_name")]
        public string cloud_instance_name { get; set; }

        [JsonProperty("cloud_graph_host_name")]
        public string cloud_graph_host_name { get; set; }
    }
}
