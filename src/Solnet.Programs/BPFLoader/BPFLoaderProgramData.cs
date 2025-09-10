using Solnet.Programs.Utilities;
using System;

namespace Solnet.Programs;

/// <summary>
/// BPF Loader Program
/// </summary>
public class BPFLoaderProgramData
{
    /// <summary>
    /// Method offset is zero for this program
    /// </summary>
    private const int MethodOffset = 0;
    /// <summary>
    /// Encode Write data
    /// </summary>
    /// <param name="offset">the offset</param>
    /// <param name="buffer">buffer to be written</param>
    /// <returns>The encoded data</returns>
    internal static Span<byte> EncodeWrite(uint offset, Span<byte> buffer)
    {
        byte[] data = new byte[sizeof(uint) + sizeof(uint)  +sizeof(ulong) + buffer.Length];
        data.WriteU32( (uint) BPFLoaderProgramInstructions.Values.Write, MethodOffset);
        data.WriteU32(  offset, MethodOffset + sizeof(uint));
        data.WriteBorshByteVector(buffer, sizeof(uint) + sizeof(uint) + MethodOffset);
        return data;
    }
    /// <summary>
    /// Encode InitializeBuffer data
    /// </summary>
    /// <returns>The encoded data</returns>
    internal static Span<byte> EncodeInitializeBuffer()
    {
        byte[] data = new byte[sizeof(uint)];
        data.WriteU32((uint)BPFLoaderProgramInstructions.Values.InitializeBuffer, MethodOffset);
        return data;
    }
    /// <summary>
    /// Encode DeployWithMaxDataLen data
    /// </summary>
    /// <param name="maxDataLenght">the max data lenght</param>
    /// <returns>The encoded data</returns>
    public static Span<byte> EncodeDeployWithMaxDataLen(ulong maxDataLenght)
    {
        byte[] data = new byte[sizeof(uint) + sizeof(ulong)];
        data.WriteU32((uint)BPFLoaderProgramInstructions.Values.DeployWithMaxDataLen, MethodOffset);
        data.WriteU64(maxDataLenght, MethodOffset + sizeof(uint));
        return data;
    }
    /// <summary>
    /// Encode Upgrade data
    /// </summary>
    /// <returns>The encoded data</returns>
    public static Span<byte> EncodeUpgrade()
    {
        byte[] data = new byte[sizeof(uint)];
        data.WriteU32((uint)BPFLoaderProgramInstructions.Values.Upgrade, MethodOffset);
        return data;
    }
    /// <summary>
    /// Encode SetAuthority data
    /// </summary>
    /// <returns>The encoded data</returns>
    public static Span<byte> EncodeSetAuthority()
    {
        byte[] data = new byte[sizeof(uint)];
        data.WriteU32((uint)BPFLoaderProgramInstructions.Values.SetAuthority, MethodOffset);
        return data;
    }
    /// <summary>
    /// Encode Close data
    /// </summary>
    /// <returns>The encoded data</returns>
    public static Span<byte> EncodeClose()
    {
        byte[] data = new byte[sizeof(uint)];
        data.WriteU32((uint)BPFLoaderProgramInstructions.Values.Close, MethodOffset);
        return data;
    }
}