using Azure.Core;

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
          
        public static class Schedule
        {
            public const string GetAll = "schedules";
            public const string GetById = "schedules/{idSchedule:int}";
            public const string Create = "schedules";
            public const string Update = "schedules/{scheduleId:int}";
        }

        public static class Appointment
        {
            public const string List = "doctor/{idUserDoctor:int}/appointments";
            public const string Reserve = "doctor/{idUserDoctor:int}/appointments/{idAppointment:int}";
        }
    }
}
