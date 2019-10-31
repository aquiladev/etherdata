using System;

namespace EtherData.Func.MythX
{
    public class AccountLimitExceedException : Exception
    {
        public string Address { get; private set; }

        public AccountLimitExceedException(string address)
        {
            Address = address;
        }
    }
}
