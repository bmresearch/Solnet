// unset

using Solnet.Wallet;
using System;
using System.Collections.Generic;

namespace Solnet.Programs
{
    public static class InstructionDecoder
    {
        private static Dictionary<PublicKey, DecodeMethodType> InstructionDictionary = new();
        public delegate DecodedInstruction DecodeMethodType(ReadOnlySpan<byte> data);

        static InstructionDecoder()
        {
            InstructionDictionary.Add(MemoProgram.ProgramIdKey, MemoProgram.Decode);            
        }
        
        public static void Register(PublicKey programKey, DecodeMethodType methodType)
        {
            InstructionDictionary.Add(programKey, methodType);
        }
        
        public static DecodedInstruction Decode(PublicKey programKey, ReadOnlySpan<byte> data)
        {
            DecodeMethodType method = InstructionDictionary[programKey];
            return method?.Invoke(data);
        }
    }
}