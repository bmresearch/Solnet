namespace Solnet.Rpc.Models
{
    /// <summary>
    /// Represents inflation governor information.
    /// </summary>
    public class InflationGovernor
    {
        /// <summary>
        /// The initial inflation percentage from time zero.
        /// </summary>
        public decimal Initial { get; set; }
        
        /// <summary>
        /// The terminal inflation percentage.
        /// </summary>
        public decimal Terminal { get; set; }
        
        /// <summary>
        /// The rate per year at which inflation is lowered.
        /// <remarks>Rate reduction is derived using the target slot time as per genesis config.</remarks>
        /// </summary>
        public decimal Taper { get; set; }
        
        /// <summary>
        /// Percentage of total inflation allocated to the foundation.
        /// </summary>
        public decimal Foundation { get; set; }
        
        /// <summary>
        /// Duration of foundation pool inflation in years.
        /// </summary>
        public decimal FoundationTerm { get; set; }
    }
}