using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HealthMed.Application.Contracts.Users;
using HealthMed.Application.Core.Abstractions.Authentication;
using HealthMed.Application.Core.Abstractions.Cryptography;
using HealthMed.Application.Core.Abstractions.Data;
using HealthMed.Application.Core.Abstractions.Services;
using HealthMed.Domain.Entities;
using HealthMed.Domain.Enumerations;
using HealthMed.Domain.Errors;
using HealthMed.Domain.Exceptions;
using HealthMed.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.SqlServer.Server;

namespace HealthMed.Application.Services
{
    internal sealed class UserService : IUserService
    {
        #region Read-Only Fields

        private readonly IDbContext _dbContext;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtProvider _jwtProvider;
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;


        #endregion

        #region Constructors

        public UserService(IDbContext dbContext,
            IUnitOfWork unitOfWork,
            IJwtProvider jwtProvider,
            IUserRepository userRepository,
            IPasswordHasher passwordHasher
        )
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _jwtProvider = jwtProvider ?? throw new ArgumentNullException(nameof(jwtProvider));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        }

        #endregion

        #region IUserService Members

        public async Task<int> CreateAsync(string name, string cpf, string crm, string email, UserRoles userRole, string password)
        {
            var emailResult = Domain.ValueObjects.Email.Create(email);

            if (!await _userRepository.IsEmailUniqueAsync(emailResult))
                throw new DomainException(DomainErrors.User.DuplicateEmail);

            if (userRole == UserRoles.Doctor && !ValidCrm(crm))
                throw new DomainException(DomainErrors.User.InvalidCRM);

            if (!IsCpfValid(cpf))
                throw new DomainException(DomainErrors.User.InvalidCPF);

            var passwordHash = _passwordHasher.HashPassword(Domain.ValueObjects.Password.Create(password));

            var user = new Domain.Entities.User(name, cpf, crm, emailResult, userRole, passwordHash);

            _userRepository.Insert(user);
            await _unitOfWork.SaveChangesAsync();

            return user.Id;
        }

        public async Task<DetailedUserResponse> GetUserByEmailAsync(string email)
        {
            IQueryable<DetailedUserResponse> userQuery = (
                from user in _dbContext.Set<User, int>().AsNoTracking()
                join role in _dbContext.Set<Role, byte>().AsNoTracking()
                    on user.IdRole equals role.Id
                where user.Email.Value == email
                select new DetailedUserResponse
                {
                    IdUser = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Cpf = user.CPF,
                    Crm = user.CRM,
                    Role = role != null ? new RoleResponse { IdRole = role.Id, Name = role.Name } : null,
                    CreatedAt = user.CreatedAt,
                    LastUpdatedAt = user.LastUpdatedAt
                });

            return await userQuery.FirstOrDefaultAsync();
        }

        public async Task<DetailedUserResponse> GetUserByIdAsync(int idUser)
        {
            IQueryable<DetailedUserResponse> userQuery = (
                from user in _dbContext.Set<User, int>().AsNoTracking()
                join role in _dbContext.Set<Role, byte>().AsNoTracking()
                    on user.IdRole equals role.Id
                where user.Id == idUser
                select new DetailedUserResponse
                {
                    IdUser = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Cpf = user.CPF,
                    Crm = user.CRM,
                    Role = role != null ? new RoleResponse { IdRole = role.Id, Name = role.Name } : null,
                    CreatedAt = user.CreatedAt,
                    LastUpdatedAt = user.LastUpdatedAt
                });

            return await userQuery.FirstOrDefaultAsync();
        }

        public async Task<bool> IsEmailUniqueAsync(string email)
        {
            var userExists = await (
                 from user in _dbContext.Set<Domain.Entities.User, int>().AsNoTracking()
                 where user.Email == email
                 select user
             ).AnyAsync();

            return !userExists;
        }

        public bool IsCpfValid(string cpf)
        {
            string pattern = @"^\d+$";

            if (cpf.Length != 11) return false;

            if (new string(cpf[0], cpf.Length) == cpf) return false;

            if (!Regex.IsMatch(cpf, pattern)) return false;

            int[] firstMultipliers = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] secondMultipliers = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int sum;
            string digit;
            int reminder;

            var cpfTemp = cpf.Substring(0, 9);
            sum = 0;
            for (int i = 0; i < 9; i++)
            {
                sum += int.Parse(cpfTemp[i].ToString()) * firstMultipliers[i];
            }
            reminder = sum % 11;
            if (reminder < 2) reminder = 0;
            else reminder = 11 - reminder;

            digit = reminder.ToString();
            cpfTemp = cpfTemp + digit;

            sum = 0;

            for (int i = 0; i < 10; i++)
            {
                sum += int.Parse(cpfTemp[i].ToString()) * secondMultipliers[i];
            }

            reminder = sum % 11;
            if (reminder < 2) reminder = 0;
            else reminder = 11 - reminder;
            digit = digit + reminder.ToString();

            return cpf.EndsWith(digit);
        }

        private bool ValidCrm(string crm)
        {
            if (crm is null || crm.Length != 10) return false;
            string pattern = @"^\d{7}-[a-zA-Z]{2}$";
            return Regex.IsMatch(crm, pattern);
        }

        #endregion
    }
}
