using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Programs.Governance.Models
{
    /// <summary>
    /// Governance Token Owner Record
    /// Account PDA seeds: ['governance', realm, token_mint, token_owner ]
    /// </summary>
    public class TokenOwnerRecord : GovernanceProgramAccount
    {
        /// <summary>
        /// The Realm the TokenOwnerRecord belongs to
        /// </summary>
        public PublicKey Realm;

        /// <summary>
        /// Governing Token Mint the TokenOwnerRecord holds deposit for
        /// </summary>
        public PublicKey GoverningTokenMint;

        /// <summary>
        /// The owner (either single or multisig) of the deposited governing SPL Tokens
        /// This is who can authorize a withdrawal of the tokens
        /// </summary>
        public PublicKey GoverningTokenOwner;

        /// <summary>
        /// The amount of governing tokens deposited into the Realm
        /// This amount is the voter weight used when voting on proposals
        /// </summary>
        public ulong GoverningTokenDepositAmount;

        /// <summary>
        /// The number of votes cast by TokenOwner but not relinquished yet
        /// Every time a vote is cast this number is increased and it's always decreased when relinquishing a vote regardless of the vote state
        /// </summary>
        public uint UnrelinquishedVotesCount;

        /// <summary>
        /// The total number of votes cast by the TokenOwner
        /// If TokenOwner withdraws vote while voting is still in progress total_votes_count is decreased  and the vote doesn't count towards the total
        /// </summary>
        public uint TotalVotesCount;

        /// <summary>
        /// The number of outstanding proposals the TokenOwner currently owns
        /// The count is increased when TokenOwner creates a proposal
        /// and decreased  once it's either voted on (Succeeded or Defeated) or Cancelled
        /// By default it's restricted to 1 outstanding Proposal per token owner
        /// </summary>
        public byte OutstandingProposalCount;

        /// <summary>
        /// A single account that is allowed to operate governance with the deposited governing tokens
        /// It can be delegated to by the governing_token_owner or current governance_delegate
        /// </summary>
        public PublicKey GovernanceDelegate;
    }
}
