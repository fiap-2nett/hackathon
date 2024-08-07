using HealthMed.Domain.Core.Primitives;

namespace HealthMed.Domain.Errors
{
    public static class DomainErrors
    {
        public static class General
        {
            public static Error UnProcessableRequest => new Error(
                "General.UnProcessableRequest",
                "The server could not process the request.");

            public static Error ServerError => new Error(
                "General.ServerError",
                "The server encountered an unrecoverable error.");
        }

        public static class Authentication
        {
            public static Error InvalidEmailOrPassword => new Error(
                "Authentication.InvalidEmailOrPassword",
                "The specified email or password are incorrect.");
        }

        public static class Email
        {
            public static Error NullOrEmpty => new Error(
                "Email.NullOrEmpty",
                "The email is required.");

            public static Error LongerThanAllowed => new Error(
                "Email.LongerThanAllowed",
                "The email is longer than allowed.");

            public static Error InvalidFormat => new Error(
                "Email.InvalidFormat",
                "The email format is invalid.");
        }

        public static class Password
        {
            public static Error NullOrEmpty => new Error(
                "Password.NullOrEmpty",
                "The password is required.");

            public static Error TooShort => new Error(
                "Password.TooShort",
                "The password is too short.");

            public static Error MissingUppercaseLetter => new Error(
                "Password.MissingUppercaseLetter",
                "The password requires at least one uppercase letter.");

            public static Error MissingLowercaseLetter => new Error(
                "Password.MissingLowercaseLetter",
                "The password requires at least one lowercase letter.");

            public static Error MissingDigit => new Error(
                "Password.MissingDigit",
                "The password requires at least one digit.");

            public static Error MissingNonAlphaNumeric => new Error(
                "Password.MissingNonAlphaNumeric",
                "The password requires at least one non-alphanumeric.");
        }

        public static class User
        {
            public static Error NotFound => new Error(
                "User.NotFound",
                "The user with the specified identifier was not found.");

            public static Error InvalidPermissions => new Error(
                "User.InvalidPermissions",
                "The current user does not have the permissions to perform that operation.");

            public static Error DuplicateEmail => new Error(
                "User.DuplicateEmail",
                "The specified email is already in use.");
            
            public static Error NameIsRequired = new Error(
                "User.NameIsRequired",
                "The user name is required.");

            public static Error CRMCannotBeNull = new Error(
                "User.CRMCannotBeNull",
                "The doctor's CRM is required.");

            public static Error InvalidCPF = new Error(
                "User.InvalidCPF",
                "The informed CPF is invalid.");

            public static Error InvalidCRM = new Error(
                "User.InvalidCRM",
                "The informed CRM is invalid.");
        }

        public static class Schedule
        {
            public static Error DataSentIsInvalid => new Error(
                "Schedule.DataSentIsInvalid",
                "The schedule data sent in the request is invalid.");

            public static Error InvalidPermissions => new Error(
                "Schedule.InvalidPermissions",
                "The current user does not have the permissions to perform that operation.");

            public static Error ScheduleInvalid => new Error(
                "Schedule.ScheduleInvalid",
                "Invalid schedule. Check for overlapping times, broken schedules, or incorrect duration.");

            public static Error Conflicting => new Error(
                "Schedule.Conflicting",
                "There is a conflicting appointment in the specified period.");

            public static Error NotFound => new Error(
                "Schedule.NotFound",
                "The schedule with the specified identifier was not found.");

            public static Error DifferentDate => new Error(
                "Schedule.DifferentDate",
                "The start date and time cannot be the same as the end date and time.");
        }

        public static class Appointment
        {
            public static Error NotFound => new Error(
                "Appointment.NotFound",
                "The appointment with the specified identifier was not found.");

            public static Error RetroactiveReserve => new Error(
                "Appointment.RetroactiveReserve",
                "Retroactive bookings are not permitted.");

            public static Error CannotBeReserved => new Error(
                "Appointment.CannotBeReserved",
                "The appointment is already booked for another patient.");

            public static Error Overlap = new Error(
                "Appointment.Overlap",
                "There was an overlap in appointments, please check availability.");
        }
    }
}
