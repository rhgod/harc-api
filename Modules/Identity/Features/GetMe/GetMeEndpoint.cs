using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Harc.Api.Modules.Identity.Data;

namespace Harc.Api.Modules.Identity.Features.GetMe;

public class GetMeEndpoint : EndpointWithoutRequest<GetMeResponse>
{
    private readonly IdentityDbContext _dbContext;
    public GetMeEndpoint(IdentityDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override void Configure()
    {
        Get("api/identity/me");
        Description(b => b
            .WithName("GetMe")
            .WithTags("Identity"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        // .NET 10 kimlik kartındaki (Claims) bilgileri doğrudan çekiyoruz
        var email = User.FindFirst("email")?.Value;
        var role = User.FindFirst("role")?.Value;
        var userId = User.FindFirst("harc_user_id")?.Value;

        Dictionary<string, string> roleDisplayName = new Dictionary<string, string>();
        string? avatar = null;

        if (Guid.TryParse(userId, out var userIdGuid))
        {
            var user = await _dbContext.Users
                .AsNoTracking()
                .Where(u => u.Id == userIdGuid)
                .Select(u => new { u.Role, u.AvatarUrl })
                .FirstOrDefaultAsync(ct);

            if (user != null)
            {
                roleDisplayName = user.Role.DisplayName;
                avatar = user.AvatarUrl;
            }
        }

        GetMeResponse response = new GetMeResponse
        {
            Message = "FastEndpoints Zero Trust koruması başarıyla çalıştı!",
            InternalUserId = userId,
            UserEmail = email,
            AssignedRole = role,
            AssignedRoleDisplayName = roleDisplayName,
            Avatar = avatar
        };

        await Send.OkAsync(response, ct);
    }
}