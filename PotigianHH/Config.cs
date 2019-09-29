namespace PotigianHH
{
    public static class Config
    {
        public static class Database
        {
            private const string Hostname = "192.168.10.98";
            private const string User = "SA";
            private const string Password = "Admin01";
            private const string Name = "POTIGIAN_QA";

            public static string ConnectionString = $"Data Source={Hostname};Initial Catalog={Name};Persist Security Info=True;User ID={User};Password={Password}";
        }

        public static class Requests
        {
            public const int StateAvailableToPrepare = 6;
            public const int StateInPreparation = 7;
            public const int RequestsPerPreparer = 5;
        }
    }
}
