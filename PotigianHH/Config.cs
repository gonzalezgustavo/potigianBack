namespace PotigianHH
{
    public static class Config
    {
        public const string ProgramCode = "SD03P053";

        public static class Requests
        {
            public const int StateAvailableToPrepare = 6;
            public const int StateInPreparation = 7;
            public const int StateIncomplete = 8;
            public const int StateClosed = 12;
        }
    }
}
