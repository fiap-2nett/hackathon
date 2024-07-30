namespace HealthMed.Api.Contracts
{
    public static class ApiRoutes
    {
        public static class Authentication
        {
            public const string Login = "authentication/login";
            public const string Register = "authentication/register";
        }

        public static class Schedule
        {
            public const string Create = "schedule/create";
            public const string Update = "schedule/update";
        }
    }
}
