using Solnet.Wallet;
using System.Collections.Generic;

namespace Solnet.Rpc.Models
{
    public class AddressLookupTableState
    {
        public long DeactivationSlot { get; set; }
        public int LastExtendedSlot { get; set; }
        public int LastExtendedSlowStartIndex { get ; set; }
        public PublicKey Authority { get; set; }
        public List<PublicKey> Addresses { get; set; }
    }
    
    
    public class AddressLookupTableAccount
    {
        private PublicKey _key;
        private AddressLookupTableState _state;

        public AddressLookupTableAccount(PublicKey key, AddressLookupTableState state)
        {
            _key = key;
            _state = state;
        }

        public bool IsActive 
        {
            get 
            {
                return _state.DeactivationSlot == long.MaxValue;
            }
        }

        public static AddressLookupTableState Deserialize(byte[] accountData)
        {
            var meta = DecodeData()
        }

    }
}