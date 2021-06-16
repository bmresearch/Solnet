namespace Solnet.Programs
{
    /// <summary>
    /// Represents the types of authorities for <see cref="TokenProgram.SetAuthority"/> instructions.
    /// </summary>
    public enum AuthorityType : byte
    {
        /// <summary>
        /// Authority to mint new tokens.
        /// </summary>
        MintTokens = 1,
        
        /// <summary>
        /// Authority to freeze any account associated with the mint.
        /// </summary>
        FreezeAccount = 2,
        
        /// <summary>
        /// Owner of a given account token.
        /// </summary>
        AccountOwner = 3,
        
        /// <summary>
        /// Authority to close a given account.
        /// </summary>
        CloseAccount = 4,
    }
}