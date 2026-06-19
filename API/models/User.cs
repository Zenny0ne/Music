using Microsoft.AspNetCore.Identity;

namespace API.Models;

public class User : IdentityUser
{
    public string? FullName { get; set; }
    public string ProfileImage { get; set; } = "/uploads/picture/default_user.png";
    public Playlist? FavoriteSongs { get; set; }
    public List<Album>? FavoriteAlbums { get; set; }
    public List<Artist>? FavoriteArtists { get; set; }
    public bool isArtist = false;
}