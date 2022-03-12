using Solnet.Programs.Abstract;
using Solnet.Programs.Models;
using Solnet.Programs.Models.NameService;
using Solnet.Rpc;
using Solnet.Rpc.Models;
using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Solnet.Programs.Clients
{
    /// <summary>
    /// A client for the Spl Name Service. 
    /// Enables easy lookup into names and addresses.
    /// </summary>
    public class NameServiceClient : BaseClient
    {
        /// <summary>
        /// The top level domain for the token registry.
        /// </summary>
        public static readonly PublicKey TokenTLD = new("6NSu2tci4apRKQtt257bAVcvqYjB3zV2H1dWo56vgpa6");

        /// <summary>
        /// The top level domain for the twitter registry.
        /// </summary>
        public static readonly PublicKey TwitterTLD = new("4YcexoW3r78zz16J2aqmukBLRwGq6rAvWzJpkYAXqebv");

        /// <summary>
        /// Class for the reverse twitter name derivation and lookup.
        /// </summary>
        public static readonly PublicKey ReverseTwitterNameClass = new("FvPH7PrVrLGKPfqaf3xJodFTjZriqrAXXLTVWEorTFBi");

        /// <summary>
        /// The top level domain for the .sol domain name registry.
        /// </summary>
        public static readonly PublicKey SolTLD = new("58PwtjSDuFHuUkYjH9BYnnQKHfwo9reZhC2zMJv9JPkx");

        /// <summary>
        /// Class for the reverse Sol name derivation and lookup.
        /// </summary>
        public static readonly PublicKey ReverseSolNameClass = new("33m47vH6Eav6jr5Ry86XjhRft2jRBLDnDgPSHoquXi2Z");

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="rpcClient">The rpc client to connect to the network.</param>
        public NameServiceClient(IRpcClient rpcClient) : base(rpcClient, null, NameServiceProgram.ProgramIdKey)
        {

        }

        /// <summary>
        /// Gets all records owned by a given account.
        /// </summary>
        /// <param name="address">The owner address.</param>
        /// <returns>A list containing all records.</returns>
        public async Task<List<RecordBase>> GetAllNamesByOwnerAsync(string address)
        {
            var res = await RpcClient.GetProgramAccountsAsync(ProgramIdKey, Rpc.Types.Commitment.Confirmed, null,
                new List<MemCmp>() { new MemCmp() { Bytes = address, Offset = 32 } });

            List<RecordBase> result = new();

            if(!res.WasSuccessful || res.Result == null || res.Result.Count == 0) return result;

            Dictionary<string, NameRecord> nameToRecordMap = new Dictionary<string, NameRecord>();

            foreach (var add in res.Result)
            {
                var data = Convert.FromBase64String(add.Account.Data[0]);
                var header = RecordHeader.Deserialize(data);

                if (header.ParentName == SolTLD)
                {
                    var hashedName = NameServiceProgram.ComputeHashedName(add.PublicKey);
                    var pda = NameServiceProgram.DeriveNameAccountKey(hashedName, ReverseSolNameClass, null);

                    var name = NameRecord.Deserialize(data);

                    nameToRecordMap.Add(pda, name);
                }
                else if (header.ParentName == TwitterTLD)
                {
                    if (header.Class == ReverseTwitterNameClass) //reverse twitter name res
                    {
                        var reverseTwitterName = ReverseTwitterRecord.Deserialize(data);
                        reverseTwitterName.AccountAddress = new(add.PublicKey);

                        result.Add(reverseTwitterName);
                    }
                    else //twitter name resolution
                    {
                        var twitterName = NameRecord.Deserialize(data);
                        twitterName.AccountAddress = new(add.PublicKey);

                        result.Add(twitterName);
                    }
                }
                else if (header.ParentName == TokenTLD)
                {
                    if (data.Length == 224) // it seems reverse token names are 224 bytes 
                    {
                        var revToken = ReverseTokenNameRecord.Deserialize(data);
                        revToken.AccountAddress = new(add.PublicKey);

                        result.Add(revToken);
                    }
                    else // bigger account data to store all metadata related to the token
                    {
                        var tokenName = TokenNameRecord.Deserialize(data);
                        tokenName.AccountAddress = new(add.PublicKey);

                        result.Add(tokenName);
                    }
                }
            }

            var reverseNameAddresses = nameToRecordMap.Keys.ToList();
            List<AccountInfo> accInfos = new();

            var addressesCopy = new List<string>(reverseNameAddresses);

            while (addressesCopy.Count > 0)
            {
                List<string> currentReq = null;
                if (addressesCopy.Count > 100)
                {
                    currentReq = addressesCopy.Take(100).ToList();
                    addressesCopy = addressesCopy.Skip(100).ToList();
                }
                else
                {
                    currentReq = new(addressesCopy);
                    addressesCopy.Clear();
                }

                var multipleAccs = await RpcClient.GetMultipleAccountsAsync(currentReq, Rpc.Types.Commitment.Confirmed);

                if (!multipleAccs.WasSuccessful)
                {
                    break;
                }

                multipleAccs.Result.Value.ForEach(x => accInfos.Add(x));
            }
            
            for (int i = 0; i < accInfos.Count; i++)
            {
                var nr = nameToRecordMap[reverseNameAddresses[i]];
                var current = accInfos[i];
                // it seems bonfida screwd up and some of these pdas are empty and impossible to get the name
                //
                if (current == null)
                {
                    continue;
                }

                var rev = ReverseNameRecord.Deserialize(Convert.FromBase64String(current.Data[0]));

                rev.Value = nr;
                rev.AccountAddress = new(reverseNameAddresses[i]);

                result.Add(rev);
            }

            return result;
        }

        /// <summary>
        /// Helper method that calls <c>GetAccount&lt;<typeparamref name="T"/>&gt;()</c> and sets common params on result object.
        /// </summary>
        /// <typeparam name="T">The type to Serialize to.</typeparam>
        /// <param name="accountAddress">The account to retrieve.</param>
        /// <param name="lookupValue">The lokup helper value to store in the returning object.</param>
        /// <returns>Returns the parsed account object according to the given type parameter <typeparamref name="T"/>.</returns>
        private async Task<AccountResultWrapper<T>> GetAccountAndSetMetadata<T>(PublicKey accountAddress, string lookupValue) where T : RecordBase
        {
            var res = await GetAccount<T>(accountAddress);

            if (res.WasSuccessful)
            {
                res.ParsedResult.AccountAddress = accountAddress;
                res.ParsedResult.LookupValue = lookupValue;
            }

            return res;
        }

        /// <summary>
        /// Gets the token info for a given token mint.
        /// </summary>
        /// <param name="address">The token mint address.</param>
        /// <returns>The token info record.</returns>
        public async Task<AccountResultWrapper<TokenNameRecord>> GetTokenInfoFromMintAsync(string address)
        {
            var hashedName = NameServiceProgram.ComputeHashedName(address);

            var pda = NameServiceProgram.DeriveNameAccountKey(hashedName, null, TokenTLD);


            return await GetAccountAndSetMetadata<TokenNameRecord>(pda, address);
        }

        /// <summary>
        /// Gets the token mint from a given token ticker.
        /// </summary>
        /// <param name="tokenTicker">The ticker for the token.</param>
        /// <returns>The record containing the token mint address.</returns>
        public async Task<AccountResultWrapper<ReverseTokenNameRecord>> GetMintFromTokenTickerAsync(string tokenTicker)
        {
            var hashedName = NameServiceProgram.ComputeHashedName(tokenTicker);

            var pda = NameServiceProgram.DeriveNameAccountKey(hashedName, null, TokenTLD);


            return await GetAccountAndSetMetadata<ReverseTokenNameRecord>(pda, tokenTicker);
        }

        /// <summary>
        /// Gets the name record from a given twitter handle.
        /// </summary>
        /// <param name="twitterHandle">The twitter handle.</param>
        /// <returns>The name record.</returns>
        public async Task<AccountResultWrapper<NameRecord>> GetAddressFromTwitterHandleAsync(string twitterHandle)
        {
            var hashedName = NameServiceProgram.ComputeHashedName(twitterHandle);

            var pda = NameServiceProgram.DeriveNameAccountKey(hashedName, null, TwitterTLD);

            return await GetAccountAndSetMetadata<NameRecord>(pda, twitterHandle);
        }

        /// <summary>
        /// Get the reverse twitter record from a given address.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <returns>The reverse record containing the twitter handle.</returns>
        public async Task<AccountResultWrapper<ReverseTwitterRecord>> GetTwitterHandleFromAddressAsync(string address)
        {
            var hashedName = NameServiceProgram.ComputeHashedName(address);

            var pda = NameServiceProgram.DeriveNameAccountKey(hashedName, ReverseTwitterNameClass, TwitterTLD);

            return await GetAccountAndSetMetadata<ReverseTwitterRecord>(pda, address);
        }

        /// <summary>
        /// Gets the address record from a given .sol name.
        /// </summary>
        /// <param name="name">The name (either with <c>".sol"</c> or without).</param>
        /// <returns>The address record from the given name.</returns>
        public async Task<AccountResultWrapper<NameRecord>> GetAddressFromNameAsync(string name)
        {
            if (name.EndsWith(".sol"))
            {
                name = name.Substring(0, name.Length - 4);
            }

            var hashedName = NameServiceProgram.ComputeHashedName(name);

            var pda = NameServiceProgram.DeriveNameAccountKey(hashedName, null, SolTLD);

            return await GetAccountAndSetMetadata<NameRecord>(pda, name);
        }

        /// <summary>
        /// Get name records owned by a given address.
        /// </summary>
        /// <param name="address">The owner address.</param>
        /// <returns>A collection of name records owned by the address.</returns>
        public async Task<List<ReverseNameRecord>> GetNamesFromAddressAsync(string address)
        {

            var res = await RpcClient.GetProgramAccountsAsync(ProgramIdKey, Rpc.Types.Commitment.Confirmed, null,
                new List<MemCmp>() { new MemCmp() { Bytes = SolTLD, Offset = 0 }, new MemCmp() { Bytes = address, Offset = 32 } });

            List<ReverseNameRecord> ret = new();
            if(!res.WasSuccessful || res.Result == null || res.Result.Count == 0) return ret;

            Dictionary<string, NameRecord> nameToRecordMap = new Dictionary<string, NameRecord>();
            foreach (var add in res.Result)
            {
                var name = NameRecord.Deserialize(Convert.FromBase64String(add.Account.Data[0]));
                var hashedName = NameServiceProgram.ComputeHashedName(add.PublicKey);

                var pda = NameServiceProgram.DeriveNameAccountKey(hashedName, ReverseSolNameClass, null);

                nameToRecordMap.Add(pda, name);
            }

            var reverseNameAddresses = nameToRecordMap.Keys.ToList();
            List<AccountInfo> accInfos = new();

            var addressesCopy = new List<string>(reverseNameAddresses);

            while (addressesCopy.Count > 0)
            {
                List<string> currentReq = null;
                if (addressesCopy.Count > 100)
                {
                    currentReq = addressesCopy.Take(100).ToList();
                    addressesCopy = addressesCopy.Skip(100).ToList();
                }
                else
                {
                    currentReq = new(addressesCopy);
                    addressesCopy.Clear();
                }

                var multipleAccs = await RpcClient.GetMultipleAccountsAsync(currentReq, Rpc.Types.Commitment.Confirmed);

                if (!multipleAccs.WasSuccessful)
                {
                    break;
                }

                multipleAccs.Result.Value.ForEach(x => accInfos.Add(x));
            }

            for (int i = 0; i < accInfos.Count; i++)
            {
                var nr = nameToRecordMap[reverseNameAddresses[i]];
                var current = accInfos[i];
                // it seems bonfida screwd up and some of these pdas are empty and impossible to get the name
                //
                if (current == null)
                {
                    continue;
                }

                var rev = ReverseNameRecord.Deserialize(Convert.FromBase64String(current.Data[0]));

                rev.Value = nr;
                rev.AccountAddress = new(reverseNameAddresses[i]);

                ret.Add(rev);
            }

            return ret;
        }
    }
}
