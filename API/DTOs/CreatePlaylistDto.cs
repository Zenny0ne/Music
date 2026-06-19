using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class CreatePlaylistDto
{
    [Required]
    public string Name { get; set; } = string.Empty;
}