using FastEndpoints;
using System.Security.Claims;

namespace Harc.Api.Modules.Identity.Features.GetMe;

public class GetMeEndpoint : EndpointWithoutRequest<GetMeResponse>
{
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

        var response = new GetMeResponse
        {
            Message = "FastEndpoints Zero Trust koruması başarıyla çalıştı!",
            InternalUserId = userId,
            UserEmail = email,
            AssignedRole = role
        };

        await Send.OkAsync(response, ct);
    }
}