using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class CreateSongDto
{
    [Required]
    public string Title { get; set; } = string.Empty;
    [Required]
    public string? ArtistId { get; set; }
    public int AlbumId { get; set; }
    public string? Image { get; set; }
    [Required]
    public string? Audio { get; set; }
}