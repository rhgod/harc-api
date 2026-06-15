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
        var email = User.FindFirst("email")?.Value;
        var role = User.FindFirst("role")?.Value;
        var userId = User.FindFirst("harc_user_id")?.Value;

        Dictionary<string, string> roleDisplayName = new Dictionary<string, string>();
        string? avatar = null;
        GetMeTeamResponse? teamInfo = null;
        GetMeTitleResponse? titleInfo = null;
        GetMeManagerResponse? managerInfo = null;

        if (Guid.TryParse(userId, out var userIdGuid))
        {
            var user = await _dbContext.Users
                .AsNoTracking()
                .Where(u => u.Id == userIdGuid)
                .Select(u => new 
                { 
                    u.Role, 
                    u.AvatarUrl,
                    Team = u.Team != null ? new GetMeTeamResponse
                    {
                        Id = u.Team.Id,
                        Name = u.Team.Name,
                        DisplayName = u.Team.DisplayName
                    } : null,
                    Title = new GetMeTitleResponse
                    {
                        Id = u.Title.Id,
                        Name = u.Title.Name,
                        DisplayName = u.Title.DisplayName
                    },
                    Manager = u.Manager != null ? new GetMeManagerResponse
                    {
                        Id = u.Manager.Id,
                        FullName = u.Manager.FullName,
                        Email = u.Manager.Email,
                        AvatarUrl = u.Manager.AvatarUrl
                    } : null
                })
                .FirstOrDefaultAsync(ct);

            if (user != null)
            {
                roleDisplayName = user.Role.DisplayName;
                avatar = user.AvatarUrl;
                teamInfo = user.Team;
                titleInfo = user.Title;
                managerInfo = user.Manager;
            }
        }

        GetMeResponse response = new GetMeResponse
        {
            InternalUserId = userId,
            UserEmail = email,
            AssignedRole = role,
            AssignedRoleDisplayName = roleDisplayName,
            Avatar = avatar,
            Team = teamInfo,
            Title = titleInfo,
            Manager = managerInfo
        };

        await Send.OkAsync(response, ct);
    }
}