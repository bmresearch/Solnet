// unset

using Solnet.Programs;
using System;
using System.Text;

namespace Solnet.Examples
{
    public class InstructionDecoderExample : IExample
    {
        public void Run()
        {
            string memoInstructionData = "Hello from Solnet";
            DecodedInstruction decodedInstruction =
                InstructionDecoder.Decode(MemoProgram.ProgramIdKey, Encoding.UTF8.GetBytes(memoInstructionData));
            
            Console.WriteLine($"Decoded Instruction Values: {string.Join(Environment.NewLine, decodedInstruction.Values)}");
        }
    }
}