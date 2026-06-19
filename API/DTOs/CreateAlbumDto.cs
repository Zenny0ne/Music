using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class CreateAlbumDto
{
    [Required]
    public string Title { get; set; } = string.Empty;
}