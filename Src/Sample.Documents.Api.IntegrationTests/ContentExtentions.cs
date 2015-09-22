using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Documents.Api.IntegrationTests
{
    public static class ContentExtentions
    {
        public static Task<dynamic> ReadAsJsonAsync(this HttpContent content)
        {
            if (content == null)
            {
                throw new ArgumentNullException("content");
            }

            return content.ReadAsStringAsync().ContinueWith(t => JsonConvert.DeserializeObject(t.Result));
        }

        public static dynamic ToJObject(this object value)
        {
            return JsonConvert.DeserializeObject(JsonConvert.SerializeObject(value));
        }
    }
}
