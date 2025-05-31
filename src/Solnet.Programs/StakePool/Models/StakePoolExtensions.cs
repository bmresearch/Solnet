using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Programs.StakePool.Models
{
    // TODO: This needs to be implemented after Token2022
    //public static class StakePoolExtensions
    //{

    /// <summary>
    /// Checks if the given extension is supported for the stake pool mint.
    /// </summary>
    /// <param name="extensionType">The extension type to check.</param>
    /// <returns>True if supported, false otherwise.</returns>
    //public static bool IsExtensionSupportedForMint(ExtensionType extensionType)
    //{
    //    // Note: This list must match the Rust SUPPORTED_EXTENSIONS array.
    //    return extensionType == ExtensionType.Uninitialized
    //        || extensionType == ExtensionType.TransferFeeConfig
    //        || extensionType == ExtensionType.ConfidentialTransferMint
    //        || extensionType == ExtensionType.ConfidentialTransferFeeConfig
    //        || extensionType == ExtensionType.DefaultAccountState
    //        || extensionType == ExtensionType.InterestBearingConfig
    //        || extensionType == ExtensionType.MetadataPointer
    //        || extensionType == ExtensionType.TokenMetadata;
    //}
    //    /// <summary>
    //    /// Checks if the given extension is supported for the stake pool's fee account.
    //    /// </summary>
    //    /// <param name="extensionType">The extension type to check.</param>
    //    /// <returns>True if supported, false otherwise.</returns>
    //    public static bool IsExtensionSupportedForFeeAccount(ExtensionType extensionType)
    //    {
    //        // Note: This does not include ConfidentialTransferAccount for the same reason as in Rust.
    //        return extensionType == ExtensionType.Uninitialized
    //            || extensionType == ExtensionType.TransferFeeAmount
    //            || extensionType == ExtensionType.ImmutableOwner
    //            || extensionType == ExtensionType.CpiGuard;
    //    }
    //}
}
