using Solnet.Programs.Governance.Enums;
using Solnet.Programs.Utilities;
using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Programs.Governance.Models
{
    /// <summary>
    /// The governance account.
    /// </summary>
    public class GovernanceAccount : GovernanceProgramAccount
    {
        /// <summary>
        /// The layout of the <see cref="GovernanceAccount"/> structure.
        /// </summary>
        public static class ExtraLayout
        {
            /// <summary>
            /// 
            /// </summary>
            public const int Length = 101;

            /// <summary>
            /// 
            /// </summary>
            public const int RealmOffset = 1;

            /// <summary>
            /// 
            /// </summary>
            public const int GovernedAccountOffset = 33;

            /// <summary>
            /// 
            /// </summary>
            public const int ProposalsCountOffset = 65;

            /// <summary>
            /// 
            /// </summary>
            public const int ConfigOffset = 69;
        }

        /// <summary>
        /// 
        /// </summary>
        public PublicKey Realm;

        /// <summary>
        /// 
        /// </summary>
        public PublicKey GovernedAccount;

        /// <summary>
        /// 
        /// </summary>
        public uint ProposalsCount;

        /// <summary>
        /// 
        /// </summary>
        public GovernanceConfig Config;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static GovernanceAccount Deserialize(byte[] data)
        {
            ReadOnlySpan<byte> span = data.AsSpan();

            return new GovernanceAccount
            {
                AccountType = (GovernanceAccountType)Enum.Parse(typeof(GovernanceAccountType), span.GetU8(Layout.AccountTypeOffset).ToString()),
                Realm = span.GetPubKey(ExtraLayout.RealmOffset),
                GovernedAccount = span.GetPubKey(ExtraLayout.GovernedAccountOffset),
                ProposalsCount = span.GetU32(ExtraLayout.ProposalsCountOffset),
                Config = GovernanceConfig.Deserialize(span.GetSpan(ExtraLayout.ConfigOffset, GovernanceConfig.Layout.Length))
            };
        }
    }
}
