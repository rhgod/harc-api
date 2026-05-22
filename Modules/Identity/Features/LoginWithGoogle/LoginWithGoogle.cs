using Carter;
using MediatR;

namespace Harc.Api.Modules.Identity.Features.LoginWithGoogle;

public record LoginWithGoogleCommand(string GoogleToken) : IRequest<IResult>;
public class LoginWithGoogleHandler : IRequestHandler<LoginWithGoogleCommand, IResult>
{
    public async Task<IResult> Handle(LoginWithGoogleCommand request, CancellationToken cancellationToken)
    {
        await Task.Delay(500, cancellationToken); // Sahte gecikme

        return Results.Ok(new {
            Token = "secure-jwt-token-from-api",
            User = new { Id = "1", Email = "test@sirket.com", Role = "Admin", FullName = "Caner Yılmaz" }
        });
    }
}

public class LoginWithGoogleEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/identity/login-google", async (LoginWithGoogleCommand command, ISender sender) =>
        {
            return await sender.Send(command);
        })
        .WithTags("Identity");
    }
}