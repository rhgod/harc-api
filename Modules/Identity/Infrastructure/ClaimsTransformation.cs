using System.Globalization;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Harc.Api.Modules.Identity.Data;
using Microsoft.AspNetCore.Authentication;

namespace Harc.Api.Modules.Identity.Infrastructure;

public class ClaimsTransformation : IClaimsTransformation
{
    private readonly IdentityDbContext _dbContext;
    private const int DEFAULT_ROLE_ID = 3;

    public ClaimsTransformation(IdentityDbContext dbContext)
    {
        _dbContext = dbContext;
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

        var user = await _dbContext.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
        {
            var role = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Id == DEFAULT_ROLE_ID)
                ?? throw new InvalidOperationException($"Role with ID {DEFAULT_ROLE_ID} was not found.");

            var fullName = identity.FindFirst("name")?.Value ?? ExtractFullNameFromEmail(email);
            var avatarUrl = identity.FindFirst("picture")?.Value;

            user = new User
            {
                Email = email,
                FullName = fullName,
                RoleId = role.Id,
                AvatarUrl = avatarUrl
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            user.Role = role;
        }

        identity.AddClaim(new Claim("role", user.Role.Name));
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