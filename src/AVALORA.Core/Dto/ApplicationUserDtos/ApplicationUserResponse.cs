using AutoMapper.Configuration.Annotations;

namespace AVALORA.Core.Dto.ApplicationUserDtos;

public class ApplicationUserResponse
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string? Address { get; set; }

    [Ignore]
    public string Role { get; set; } = null!;

    [Ignore]
    public DateTimeOffset? LockoutEnd { get; set; }
}