using Microsoft.EntityFrameworkCore;
using Q3_CodeLink_EMS.Data;
using Q3_CodeLink_EMS.Dtos.AdminUser;
using Q3_CodeLink_EMS.Helpers;
using Q3_CodeLink_EMS.Models;

namespace Q3_CodeLink_EMS.Services
{
    public class AdminUserService
    {
        private readonly CodeLinkEmsDbContext dbContext;
        private readonly ILogger<AdminUserService> logger;

        public AdminUserService(CodeLinkEmsDbContext dbContext, ILogger<AdminUserService> logger)
        {
            this.dbContext = dbContext;
            this.logger = logger;
        }

        // Craete (register)
        public async Task<bool> RegisterAdminUser(RegisterAdminUserDto dto)
        {
            try
            {
                // Checking if email already exists
                if (await dbContext.AdminUsers.AnyAsync(a => a.EmailAddress.ToLower() == dto.EmailAddress.ToLower()))
                {
                    return false;
                }

                var (salt, hash) = PasswordHelper.SaltAndHashPassword(dto.PasswordInPlainText); // Generating salt and hash

                var newAdminUser = new AdminUser // Mapping Dto to Model
                {
                    Id = Guid.NewGuid(),
                    FullName = dto.FullName,
                    EmailAddress = dto.EmailAddress,
                    PasswordHash = hash,
                    PasswordSalt = salt,
                    Role = dto.Role
                };

                dbContext.AdminUsers.Add(newAdminUser);
                await dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex) // Catching and logging any unexpected error
            {
                logger.LogError(ex, "Unexpected error registering admin user: {FullName} - {EmailAddress}.", dto.FullName, dto.EmailAddress);
                return false;
            }
        }

        // Read (specified, login)
        public async Task<bool> LoginAdminUser(LoginAdminUserDto dto)
        {
            try
            {
                var adminUser = await dbContext.AdminUsers.SingleOrDefaultAsync(a => a.EmailAddress.ToLower() == dto.EmailAddress.ToLower()); // Finding user by email

                if (adminUser == null)
                {

                    return false;
                }

                bool isPasswordValid = PasswordHelper.VerifyPassword(dto.PasswordInPlainText, adminUser.PasswordSalt, adminUser.PasswordHash); // Verifying password using stored salt and hash
                return isPasswordValid;
            }
            catch (InvalidOperationException ex) // Catching and logging Exception of SingleOrDefault
            {
                logger.LogError(ex, "Multiple admin users found with EmailAddress: {EmailAddress}.", dto.EmailAddress);
                return false;
            }
            catch (Exception ex) // Catching and logging any unexpected error
            {
                logger.LogError(ex, "Unexpected error during login attempt for: {EmailAddress}", dto.EmailAddress);
                return false;
            }
        }

        public async Task<AdminUser?> GetByEmail(string email)
        {
            try
            {
                return await dbContext.AdminUsers.SingleOrDefaultAsync(a => a.EmailAddress.ToLower() == email.ToLower());
            }
            catch (InvalidOperationException ex) // Catching and logging Exception of SingleOrDefault
            {
                logger.LogError(ex, "Multiple admin users found with EmailAddress: {email}.", email);
                return null;
            }
            catch (Exception ex) // Catching and logging any unexpected error
            {
                logger.LogError(ex, "Unexpected error during login attempt for: {email}", email);
                return null;
            }
        }
    }
}
