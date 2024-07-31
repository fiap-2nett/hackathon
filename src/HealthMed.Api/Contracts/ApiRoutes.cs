namespace HealthMed.Api.Contracts
{
    public static class ApiRoutes
    {
        public static class Authentication
        {
            public const string Login = "authentication/login";
            public const string Register = "authentication/register";
        }

        public static class Users
        {
            public const string Insert = "users";
            public const string GetById = "users/{idUser:int}";
            public const string GetByEmail = "users/email/{email}";
        }
    }
}
