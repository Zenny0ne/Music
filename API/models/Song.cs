namespace API.Models;

public class Song
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? ArtistId { get; set; }
    public int? AlbumId { get; set; }
    public TimeSpan Duration { get; set; }
    public DateTime ReleaseDate { get; set; }
    public string? Image { get; set; } = "uploads/picture/default_audio.png";
    public string? Audio { get; set; }
}