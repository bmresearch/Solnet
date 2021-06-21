using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace Solnet.Rpc
{
    /// <summary>
    /// Implements a client factory for Solana RPC and Streaming RPC APIs.
    /// </summary>
    public static class ClientFactory
    {
        /// <summary>
        /// The dev net cluster.
        /// </summary>
        private const string RpcDevNet = "https://api.devnet.solana.com";

        /// <summary>
        /// The test net cluster.
        /// </summary>
        private const string RpcTestNet = "https://api.testnet.solana.com";

        /// <summary>
        /// The main net cluster.
        /// </summary>
        private const string RpcMainNet = "https://api.mainnet-beta.solana.com";


        /// <summary>
        /// The dev net cluster.
        /// </summary>
        private const string StreamingRpcDevNet = "wss://devnet.solana.com";

        /// <summary>
        /// The test net cluster.
        /// </summary>
        private const string StreamingRpcTestNet = "wss://api.testnet.solana.com";

        /// <summary>
        /// The main net cluster.
        /// </summary>
        private const string StreamingRpcMainNet = "wss://api.mainnet-beta.solana.com";

        /// <summary>
        /// Instantiate a http client.
        /// </summary>
        /// <param name="cluster">The network cluster.</param>
        /// <param name="logger">The logger.</param>
        /// <returns>The http client.</returns>
        public static IRpcClient GetClient(Cluster cluster, ILogger logger = null)
        {
            var url = cluster switch
            {
                Cluster.DevNet => RpcDevNet,
                Cluster.TestNet => RpcTestNet,
                _ => RpcMainNet,
            };

#if DEBUG
            logger ??= LoggerFactory.Create(x =>
            {
                x.AddSimpleConsole(o =>
                {
                    o.UseUtcTimestamp = true;
                    o.IncludeScopes = true;
                    o.ColorBehavior = LoggerColorBehavior.Enabled;
                    o.TimestampFormat = "HH:mm:ss ";
                })
                .SetMinimumLevel(LogLevel.Debug);
            }).CreateLogger<IRpcClient>();
#endif

            return GetClient(url, logger);
        }

        /// <summary>
        /// Instantiate a http client.
        /// </summary>
        /// <param name="url">The network cluster url.</param>
        /// <param name="logger">The logger.</param>
        /// <returns>The http client.</returns>
        public static IRpcClient GetClient(string url, ILogger logger = null)
        {

            return new SolanaRpcClient(url, logger);
        }

        /// <summary>
        /// Instantiate a streaming client.
        /// </summary>
        /// <param name="cluster">The network cluster.</param>
        /// <param name="logger">The logger.</param>
        /// <returns>The streaming client.</returns>
        public static IStreamingRpcClient GetStreamingClient(Cluster cluster, ILogger logger = null)
        {
            var url = cluster switch
            {
                Cluster.DevNet => StreamingRpcDevNet,
                Cluster.TestNet => StreamingRpcTestNet,
                _ => StreamingRpcMainNet,
            };
#if DEBUG
            logger ??= LoggerFactory.Create(x =>
            {
                x.AddSimpleConsole(o =>
               {
                   o.UseUtcTimestamp = true;
                   o.IncludeScopes = true;
                   o.ColorBehavior = LoggerColorBehavior.Enabled;
                   o.TimestampFormat = "HH:mm:ss ";
               })
                .SetMinimumLevel(LogLevel.Debug);
            }).CreateLogger<IStreamingRpcClient>();
#endif
            return GetStreamingClient(url, logger);
        }

        /// <summary>
        /// Instantiate a streaming client.
        /// </summary>
        /// <param name="url">The network cluster url.</param>
        /// <param name="logger">The logger.</param>
        /// <returns>The streaming client.</returns>
        public static IStreamingRpcClient GetStreamingClient(string url, ILogger logger = null)
        {
            return new SolanaStreamingRpcClient(url, logger);
        }
    }
}