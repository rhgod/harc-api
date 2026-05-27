using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Harc.Api.Modules.Identity.Data;
using Microsoft.AspNetCore.Authentication;
using System.Globalization;

namespace Harc.Api.Modules.Identity.Infrastructure;

public class ClaimsTransformation : IClaimsTransformation
{
    private readonly IdentityDbContext _dbContext;
    private readonly IConfiguration _configuration; // Yapılandırma servisi
    private const string EmployeeRole = "Employee";
    private const string AdminRole = "Admin";

    public ClaimsTransformation(IdentityDbContext dbContext, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _configuration = configuration;
    }

    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if (principal.HasClaim(c => c.Type == "role"))
        {
            return principal;
        }

        var clone = principal.Clone();
        var identity = (ClaimsIdentity)clone.Identity!;    
        var email = identity.FindFirst("email")?.Value;
        if (string.IsNullOrEmpty(email))
        {
            return principal;
        }

        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
        {
            var systemAdminEmail = _configuration["IdentitySettings:SystemAdminEmail"];
            string defaultRole = EmployeeRole;
            if (email.Equals(systemAdminEmail, StringComparison.OrdinalIgnoreCase)) 
            {
                defaultRole = AdminRole;
            }

            var fullName = identity.FindFirst("name")?.Value ?? ExtractFullNameFromEmail(email);
            var avatarUrl = identity.FindFirst("picture")?.Value;

            user = new User
            {
                Email = email,
                FullName = fullName,
                Role = defaultRole,
                AvatarUrl = avatarUrl
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
        }

        identity.AddClaim(new Claim("role", user.Role));
        identity.AddClaim(new Claim("harc_user_id", user.Id.ToString()));

        return clone;
    }

    private string ExtractFullNameFromEmail(string email)
    {
        try
        {
            var partBeforeAt = email.Split('@')[0];
            var words = partBeforeAt.Split(new[] { '.', '_', '-' }, StringSplitOptions.RemoveEmptyEntries);
            var textInfo = new CultureInfo("tr-TR", false).TextInfo;
            var formattedWords = words.Select(w => textInfo.ToTitleCase(w));
            return string.Join(" ", formattedWords);
        }
        catch
        {
            return "-";
        }
    }
}