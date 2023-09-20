using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace SalesRepo.Domain.Helpers
{
    public static class SerialisationSettings
    {
        public static JsonSerializerOptions DefaultOptions
        {
            get
            {
                var options = new JsonSerializerOptions
                {
                    DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                };

                options.Converters.Add(new JsonStringEnumConverter());

                return options;
            }
        }
    }
}
