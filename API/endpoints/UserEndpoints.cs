using API.Common;
using API.DTOs;
using API.Models;
using API.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.endpoints;

public static class UserEndPoints
{
    public static WebApplication MapUserEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/user");

        group.MapPost("/register", async([FromServices] UserService userService, [FromForm] RegisterDto dto, IFormFile? profileImage) =>
        {
            if(dto is null)
            {
                return Results.BadRequest(Response<string>.Failure("Invalid register request"));
            }
            
            var userFromDb = await userService.GetUserByEmail(dto.Email);
            if (userFromDb is not null)
            {
                return Results.BadRequest(Response<string>.Failure("User is already exist"));
            }

            var profileImagePath = "/uploads/picture/default_user.png";
            if (profileImage is not null)
            {
                var fileName = await FileUploads.Upload(profileImage);
                profileImagePath = $"/uploads/picture/{fileName}";
            }

            var user = new User
            {
                FullName = dto.FullName,
                UserName = dto.UserName,
                Email = dto.Email,
                ProfileImage = profileImagePath,
                FavoriteAlbums = new List<Album>(),
                FavoriteArtists = new List<Artist>(),
                FavoriteSongs = null
            };

            await userService.CreateUserAsync(user, dto.Password);
            return Results.Ok(Response<User>.Success(user, "Registered successfully"));
        }).DisableAntiforgery();

        group.MapPost("/login", async([FromServices] UserService userService, TokenService tokenService, [FromForm] LoginDto dto) =>
        {
            if(dto is null)
            {
                return Results.BadRequest(Response<string>.Failure("Invalid login request"));
            }

            var user = await userService.GetUserByEmail(dto.Email);

            if(user is null)
            {
                return Results.BadRequest(Response<string>.Failure("User not found"));
            }

            var isPasswordValid = userService.VerifyPassword(user, dto.Password);
            if (!isPasswordValid)
            {
                return Results.BadRequest(Response<string>.Failure("Invalid password"));
            }

            var token = tokenService.GenerateToken(user.Id, user.UserName!);
            return Results.Ok(Response<string>.Success(token, "Login successful"));
        }).DisableAntiforgery();

        group.MapGet("/profile", async([FromServices] UserService userService, HttpContext context) =>
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Results.Unauthorized();
            }

            var user = await userService.GetUserById(userId);
            if (user is null)
            {
                return Results.NotFound(Response<string>.Failure("User not found"));
            }

            if(user.isArtist == true){
                return Results.Unauthorized();
            }

            return Results.Ok(Response<User>.Success(user, "Profile loaded"));
        }).RequireAuthorization();

        group.MapPut("/profile/update", async([FromServices] UserService userService, [FromForm] UpdateUserDto dto, IFormFile? profileImage, HttpContext context) =>
        {
            if(dto is null)
            {
                return Results.BadRequest(Response<string>.Failure("Invalid update request"));
            }

            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Results.Unauthorized();
            }

            var user = await userService.GetUserById(userId);
            if (user is null)
            {
                return Results.NotFound(Response<string>.Failure("User not found"));
            }

            if (user.isArtist == true)
            {
                return Results.Unauthorized();
            }

            if (!string.IsNullOrWhiteSpace(dto.Email) && dto.Email != user.Email)
            {
                var existing = await userService.GetUserByEmail(dto.Email);
                if (existing is not null && existing.Id != user.Id)
                {
                    return Results.BadRequest(Response<string>.Failure("Email is already in use"));
                }
                user.Email = dto.Email;
            }

            if (!string.IsNullOrWhiteSpace(dto.FullName))
            {
                user.FullName = dto.FullName;
            }

            if (!string.IsNullOrWhiteSpace(dto.UserName))
            {
                user.UserName = dto.UserName;
            }

            if (profileImage is not null)
            {
                var fileName = await FileUploads.Upload(profileImage);
                user.ProfileImage = $"/uploads/picture/{fileName}";
            }

            await userService.UpdateUserAsync(user);
            return Results.Ok(Response<User>.Success(user, "Profile updated"));
        }).DisableAntiforgery().RequireAuthorization();

        group.MapDelete("/profile/delete", async([FromServices] UserService userService, HttpContext context) =>
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Results.Unauthorized();
            }

            var user = await userService.GetUserById(userId);
            if (user is null)
            {
                return Results.NotFound(Response<string>.Failure("User not found"));
            }

            if (user.isArtist == true)
            {
                return Results.Unauthorized();
            }

            await userService.DeleteUser(user);
            return Results.Ok(Response<string>.Success("User deleted successfully"));
        }).RequireAuthorization();

        group.MapPut("/favouritesong/{songId}", async ([FromServices] UserService userService, [FromServices] PlaylistService playlistService, [FromServices] SongService songService, HttpContext context, string songId) =>
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Results.Unauthorized();
            }

            var user = await userService.GetUserById(userId);
            if (user is null)
            {
                return Results.NotFound(Response<string>.Failure("User not found"));
            }

            var song = await songService.GetSongById(songId);
            if(song is null)
            {
                return Results.NotFound(Response<string>.Failure("Song not found"));
            }

            if(user.FavoriteSongs is null)
            {
                user.FavoriteSongs = new Playlist
                {
                    Name = "Favourite songs",
                    UserId = userId,
                    Songs = new List<Song>(),
                    CreateDate = DateTime.UtcNow,
                    LastUpdate = DateTime.UtcNow
                };
                await playlistService.CreatePlaylistAsync(user.FavoriteSongs);
            }
            user.FavoriteSongs.Songs?.Add(song);
            await playlistService.UpdatePlaylistAsync(user.FavoriteSongs);
            await userService.UpdateUserAsync(user);

           return Results.Ok(Response<User>.Success(user, "Favourite songs updates"));

        }).DisableAntiforgery().RequireAuthorization();

        group.MapPost("/favouritealbum/{albumId}", async([FromServices] UserService userService, [FromServices] AlbumService albumService, HttpContext context, string albumId) =>
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Results.Unauthorized();
            }

            var user = await userService.GetUserById(userId);
            if (user is null)
            {
                return Results.NotFound(Response<string>.Failure("User not found"));
            }

            var album = await albumService.GetAlbumById(albumId);
            if(album is null)
            {
                return Results.NotFound(Response<string>.Failure("Album not found"));
            }

            user.FavoriteAlbums ??= new List<Album>();
            user.FavoriteAlbums.Add(album);

            await userService.UpdateUserAsync(user);

            return Results.Ok(Response<User>.Success(user, "Favourite albums updates"));

        }).DisableAntiforgery().RequireAuthorization();

        group.MapPost("/favouriteartist/{artistId}", async([FromServices] UserService userService, [FromServices] ArtistService artistService, HttpContext context, string artistId) =>
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Results.Unauthorized();
            }

            var user = await userService.GetUserById(userId);
            if (user is null)
            {
                return Results.NotFound(Response<string>.Failure("User not found"));
            }

            var artist = await artistService.GetArtistById(artistId);
            if(artist is null)
            {
                return Results.NotFound(Response<string>.Failure("Artist not found"));
            }

            user.FavoriteArtists ??= new List<Artist>();
            user.FavoriteArtists.Add(artist);

            await userService.UpdateUserAsync(user);

            return Results.Ok(Response<User>.Success(user, "Favourite artists updates"));
        }).DisableAntiforgery().RequireAuthorization();

        return app;
    }
}