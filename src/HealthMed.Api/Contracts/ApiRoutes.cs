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
            public const string GetAll = "schedules";
            public const string GetById = "schedules/{idSchedule:int}";
            public const string Create = "schedules";
            public const string Update = "schedules/{scheduleId:int}";
        }
    }
}
