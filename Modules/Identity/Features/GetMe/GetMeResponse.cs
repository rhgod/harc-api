namespace Harc.Api.Modules.Identity.Features.GetMe;

public class GetMeResponse
{
    public string? InternalUserId { get; set; }
    public string? UserEmail { get; set; }
    public string? AssignedRole { get; set; }
    public Dictionary<string, string>? AssignedRoleDisplayName { get; set; }
    public string? Avatar { get; set; }
    
    public GetMeTeamResponse? Team { get; set; }
    public GetMeTitleResponse? Title { get; set; }
    public GetMeManagerResponse? Manager { get; set; }
}

public class GetMeTeamResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Dictionary<string, string> DisplayName { get; set; } = new();
}

public class GetMeTitleResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Dictionary<string, string> DisplayName { get; set; } = new();
}

public class GetMeManagerResponse
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
}