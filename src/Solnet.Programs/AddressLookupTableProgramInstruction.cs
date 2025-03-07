using System.Collections.Generic;

namespace Solnet.Programs
{
    internal static class AddressLookupTableProgramInstruction
    {
        /// <summary>
        /// Represents the user-friendly names for the instruction types for the <see cref="AddressLookupTableProgram"/>.
        /// </summary>
        internal static readonly Dictionary<Values, string> Names = new()
        {
            { Values.CreateLookupTable, "Create Lookup Table" },
            { Values.FreezeLookupTable, "Freeze Lookup Table" },
            { Values.ExtendLookupTable, "Extend Lookup Table" },
            { Values.DeactivateLookupTable, "Deactivate Lookup Table" },
            { Values.CloseLookupTable, "Close Lookup Table" }
        };

        /// <summary>
        /// Represents the instruction types for the <see cref="AddressLookupTableProgram"/>.
        /// </summary>
        internal enum Values : byte
        {
            CreateLookupTable = 0,
            FreezeLookupTable = 1,
            ExtendLookupTable = 2,
            DeactivateLookupTable =3,
            CloseLookupTable =4
        }
    }
}
