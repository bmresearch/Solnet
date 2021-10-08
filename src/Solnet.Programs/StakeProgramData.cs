using Solnet.Programs.Models;
using Solnet.Programs.Utilities;
using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Programs
{
    /// <summary>
    /// Implements the stake program data encodings.
    /// </summary>
    internal static class StakeProgramData
    {
        /// <summary>
        /// The offset at which the value which defines the program method begins. 
        /// </summary>
        internal const int MethodOffset = 0;
        /// <summary>
        /// Summary text here
        /// </summary>
        internal static byte[] EncodeInitializeData(Authorized authorized, Lockup lockup)
        {
            byte[] data = new byte[116];

            data.WriteU32((uint)StakeProgramInstructions.Values.Initialize, MethodOffset);
            data.WritePubKey(authorized.staker,4);
            data.WritePubKey(authorized.withdrawer,36);
            data.WriteS64(lockup.unix_timestamp, 68);
            data.WriteU64(lockup.epoch, 76);
            data.WritePubKey(lockup.custodian, 84);

            return data;
        }
        internal static byte[] EncodeAuthorizeData(PublicKey new_authorized_pubkey, PublicKey stake_authorize)
        {
            byte[] data = new byte[68];

            data.WriteU32((uint)StakeProgramInstructions.Values.Authorize, MethodOffset);
            data.WritePubKey(new_authorized_pubkey, 4);
            data.WritePubKey(stake_authorize, 36);

            return data;
        }

        internal static byte[] EncodeDelegateStakeData()
        {
            byte[] data = new byte[4];

            data.WriteU32((uint)StakeProgramInstructions.Values.DelegateStake, MethodOffset);

            return data;
        }
        internal static byte[] EncodeSplitData(ulong lamports)
        {
            byte[] data = new byte[12];

            data.WriteU32((uint)StakeProgramInstructions.Values.Split, MethodOffset);
            data.WriteU64(lamports, 4);

            return data;
        }

        internal static byte[] EncodeWithdrawData(ulong lamports)
        {
            byte[] data = new byte[12];

            data.WriteU32((uint)StakeProgramInstructions.Values.Withdraw, MethodOffset);
            data.WriteU64(lamports, 4);

            return data;
        }

        internal static byte[] EncodeDeactivateData()
        {
            byte[] data = new byte[4];

            data.WriteU32((uint)StakeProgramInstructions.Values.Deactivate, MethodOffset);

            return data;
        }

        internal static byte[] EncodeSetLockup(LockupArgs lockup)
        {
            throw new NotImplementedException();
        }
    }
}
