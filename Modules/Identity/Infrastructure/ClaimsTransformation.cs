using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Harc.Api.Modules.Identity.Data;
using Microsoft.AspNetCore.Authentication;

namespace Harc.Api.Modules.Identity.Infrastructure;

public class ClaimsTransformation : IClaimsTransformation
{
    private readonly IdentityDbContext _dbContext;

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
            throw new BusinessException("ERR_USER_NOT_FOUND");
        }

        if (user.Status == UserStatus.Terminated)
        {
            throw new BusinessException("ERR_ACCOUNT_TERMINATED");
        }

        identity.AddClaim(new Claim("role", user.Role.Name));
        identity.AddClaim(new Claim("harc_user_id", user.Id.ToString()));

        return clone;
    }
}