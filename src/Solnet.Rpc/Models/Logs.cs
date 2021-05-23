using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Solnet.Rpc.Models
{
    public class LogsInfo
    {
        public string Signature { get; set; }

        [JsonPropertyName("err")]
        public string Error { get; set; }

        public string[] Logs { get; set; }

    }
}
