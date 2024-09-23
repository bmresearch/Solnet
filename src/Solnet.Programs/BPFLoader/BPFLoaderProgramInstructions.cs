using System.Collections.Generic;

namespace Solnet.Programs;

internal static class BPFLoaderProgramInstructions
{
    internal static readonly Dictionary<Values, string> Names = new()
    {
        { Values.InitializeBuffer, "Initialize" },
        { Values.Write, "Write" },
        { Values.DeployWithMaxDataLen, "Deploy With Max Data Length" },
        { Values.Upgrade, "Upgrade" },
        { Values.SetAuthority, "SetAuthority" },
        { Values.Close, "Close" },
    };
    /// <summary>
    /// Represents the instruction types />.
    /// </summary>
    internal enum Values : byte
    {
        InitializeBuffer = 0,
        Write = 1,
        DeployWithMaxDataLen = 2,
        Upgrade = 3,
        SetAuthority = 4,
        Close =5,
        
    }
    
}