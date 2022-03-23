namespace Sol.Unity.Rpc
{
    /// <summary>
    /// Represents the public solana clusters.
    /// </summary>
    public enum Cluster
    {
        /// <summary>
        /// Devnet serves as a playground for anyone who wants to take Solana for a test drive, as a user, token holder, app developer, or validator.
        /// </summary>
        /// <remarks>
        /// Application developers should target Devnet.
        /// Potential validators should first target Devnet.
        /// Key points:
        ///  <list type="bullet">  
        ///    <item>Devnet tokens are not real</item>
        ///    <item>Devnet includes a token faucet for airdrops for application testing</item>
        ///    <item>Devnet may be subject to ledger resets</item>
        ///    <item>Devnet typically runs a newer software version than Mainnet Beta</item>
        /// </list>
        /// </remarks>
        DevNet,

        /// <summary>
        /// Testnet is where Solana stress tests recent release features on a live cluster, particularly focused on network performance, stability and validator behavior.
        /// </summary>
        /// <remarks>
        /// Tour de SOL initiative runs on Testnet, where malicious behavior and attacks are encouraged on the network to help find and squash bugs or network vulnerabilities.
        /// Key points:
        /// <list type="bullet">
        ///    <item>Devnet tokens are not real</item>
        ///    <item>Devnet includes a token faucet for airdrops for application testing</item>
        ///    <item>Devnet may be subject to ledger resets</item>
        ///    <item>Testnet typically runs a newer software release than both Devnet and Mainnet Beta</item>
        /// </list>
        /// </remarks>
        TestNet,

        /// <summary>
        /// A permissionless, persistent cluster for early token holders and launch partners. Currently, rewards and inflation are disabled.
        /// </summary>
        /// <remarks>
        /// Tokens that are issued on Mainnet Beta are real SOL.
        /// </remarks>
        MainNet
    }
}