using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Solnet.Rpc.Models
{
    public class ErrorResult
    {

        [JsonPropertyName("err")]
        public string Error { get; set; }
    }
}
