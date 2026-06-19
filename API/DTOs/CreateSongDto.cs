using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class CreateSongDto
{
    [Required]
    public string Title { get; set; } = string.Empty;
}