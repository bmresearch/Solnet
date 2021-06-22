// unset

namespace Solnet.Programs
{
    /// <summary>
    /// Represents the instruction types for the <see cref="NameServiceProgram"/>.
    /// </summary>
    internal enum NameServiceInstructions : byte
    {
        /// <summary>
        /// Create a name record.
        /// <remarks>
        /// The address of the name record (account #1) is a program-derived address with the following
        /// seeds to ensure uniqueness:
        /// <para>
        /// <list type="bullet">
        /// <item>
        /// SHA256(HASH_PREFIX, <c>name</c>)
        /// </item> 
        /// <item>
        /// Account class (account #3)
        /// </item>
        /// <item>
        /// Parent name record address (account #4)
        /// </item>
        /// </list>
        ///</para>
        /// If this is a child record, the parent record's owner must approve by signing (account #5)
        /// </remarks>
        /// </summary>
        Create = 0,

        /// <summary>
        /// Update the data in a name record.
        /// </summary>
        Update = 1,

        /// <summary>
        /// Transfer ownership of a name record.
        /// </summary>
        Transfer = 2,

        /// <summary>
        /// Delete a name record.
        /// <remarks>
        /// Any lamports left in the account will be transferred to the refund account.
        /// </remarks>
        /// </summary>
        Delete = 3,
    }
}