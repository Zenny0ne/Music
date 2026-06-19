using Microsoft.AspNetCore.Identity;
namespace API.Models;

public class Artist : IdentityUser
{
    public string? FullName { get; set; }
    public string? ProfileImage { get; set; } = "/uploads/picture/default_artist.png";
    public List<Album>? Albums { get; set; }
    public List<Song>? Singles { get; set; }
    public bool isArtist = true;

}