using System.ComponentModel.DataAnnotations;

public class UpdateSongDto
{
    [Required]
    public int Id { get; set; }
    public string? Title { get; set; }
    public string ? image { get; set; }
    public string? audio { get; set; }
}