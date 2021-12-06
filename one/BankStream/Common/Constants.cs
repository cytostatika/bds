using System;

namespace Common
{
    public static class Constants
    {
        public const string StreamProvider = "SMSProvider";

        public const string WithdrawStreamName = "withdraw";
        public const string DepositStreamName = "deposit";

        public static Guid WithdrawId = Guid.NewGuid();
        public static Guid DepositId = Guid.NewGuid();
    }
}