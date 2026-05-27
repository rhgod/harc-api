using FastEndpoints;

namespace Harc.Api.Modules.Identity.Features.LoginWithGoogle;

// Sınıf tanımlamasında girdi ve çıktı tiplerini vererek %100 tip güvenliği sağlıyoruz
public class LoginWithGoogleEndpoint : Endpoint<LoginWithGoogleRequest, LoginWithGoogleResponse>
{
    // Konfigürasyonlar (Rota, Yetki, Swagger Ayarları) bu metodun içindedir
    public override void Configure()
    {
        Post("api/identity/login-google");
        AllowAnonymous(); // Giriş yaparken token aranmaz
        Description(b => b
            .WithName("LoginWithGoogle")
            .WithTags("Identity"));
    }

    // Gerçek iş mantığının döndüğü asenkron metot
    public override async Task HandleAsync(LoginWithGoogleRequest req, CancellationToken ct)
    {
        // Geliştirme aşaması için sahte gecikme ve mock data
        await Task.Delay(500, ct);

        var response = new LoginWithGoogleResponse
        {
            Token = "secure-jwt-token-from-fastendpoints",
            User = new UserDto
            {
                Id = "usr_10293",
                Email = "admin@harcportal.com",
                FullName = "Caner Yılmaz",
                Role = "Admin"
            }
        };

        // FastEndpoints'in optimize edilmiş yerleşik yanıt metodu
        await Send.OkAsync(response, ct);
    }
}