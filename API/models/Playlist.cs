namespace API.Models;
public class Playlist
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? UserId { get; set; }
    public List<Song>? Songs { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime LastUpdate { get; set; }
}