namespace Solnet.Rpc.Models
{
    public class BlockCommitment
    {
        public ulong[] Commitment { get; set; }

        public ulong TotalStake { get; set; }
    }
}
