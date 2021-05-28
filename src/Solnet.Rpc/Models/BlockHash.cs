namespace Solnet.Rpc.Models
{
    public class FeeCalculator
    {
        public ulong LamportsPerSignature { get; set; }
    }
    
    public class BlockHash
    {
        public string Blockhash { get; set; }
        
        public FeeCalculator FeeCalculator { get; set; }
    }
}