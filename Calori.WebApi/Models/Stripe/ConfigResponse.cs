﻿using Newtonsoft.Json;

namespace Calori.WebApi.Models.Stripe
{
    public class ConfigResponse
    {
        [JsonProperty("publishableKey")] public string PublishableKey { get; set; }

        [JsonProperty("proPrice")] public string ProPrice { get; set; }

        [JsonProperty("basicPrice")] public string BasicPrice { get; set; }
    }
}
