using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class UpdateArtistDto
{
    public string? FullName { get; set; } = string.Empty;
    public string? UserName { get; set; } = string.Empty;
    [EmailAddress]
    public string? Email { get; set; } = string.Empty;
    public string? ProfileImage { get; set; }
}