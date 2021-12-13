using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Solnet.Programs.Utilities;

namespace Solnet.Programs.Models
{
    public class TokenData
    {
        public string Name { get; set; }

        public string Ticker { get; set; }

        public PublicKey Mint { get; set; }

        public byte Decimals { get; set; }

        public string Website { get; set; }

        public string LogoUri { get; set; }

        public static TokenData Deserialize(byte[] input)
        {
            var data = new ReadOnlySpan<byte>(input, 96, input.Length - 96);
            int offset = 0;

            offset += data.GetString(0, out var name);
            offset += data.GetString(offset, out var ticker);

            var mint = data.GetPubKey(offset);
            offset += 32;

            var decimals = data.GetU8(offset);
            offset++;
            string website = null, logo = null;

            if (data.GetBool(offset++))
                offset += data.GetString(offset, out website);

            if (data.GetBool(offset++))
                data.GetString(offset, out logo);

            return new TokenData() { Name = name, Ticker = ticker, Decimals = decimals, LogoUri = logo, Mint = mint, Website = website };
        }
    }
}
