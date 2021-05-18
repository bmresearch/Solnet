namespace Solnet.Wallet
{
    /// <summary>
    /// Specifies the available seed modes for key generation.
    /// <remarks>
    /// Available modes:
    /// <para> <see cref="SeedMode.Ed25519Bip32"/> </para>
    /// <para> <see cref="SeedMode.Bip39"/> </para>
    /// </remarks>
    /// </summary>
    public enum SeedMode
    {
        /// <summary>
        /// Generates Ed25519 based BIP32 keys.
        /// <remarks>This seed mode is compatible with the keys generated in the Sollet/SPL Token Wallet, it does not use a passphrase to harden the mnemonic seed.</remarks>
        /// </summary>
        Ed25519Bip32,
        /// <summary>
        /// Generates BIP39 keys.
        /// <remarks>This seed mode is compatible with the keys generated in the solana-keygen cli, it uses a passphrase to harden the mnemonic seed.</remarks>
        /// </summary>
        Bip39
    }
}