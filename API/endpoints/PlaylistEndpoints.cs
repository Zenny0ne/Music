using System.Security.Claims;
using API.Common;
using API.DTOs;
using API.Models;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.endpoints;

public static class PlaylistEndpoints
{
    public static WebApplication MapPlaylistEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/playlist");

        group.MapPost("/playlist", async ([FromServices] PlaylistService playlistService, [FromServices] UserService userService, [FromForm] CreatePlaylistDto dto, HttpContext context) =>
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Results.Unauthorized();
            }
            var user = await userService.GetUserById(userId);
            if(user is null)
            {
                return Results.NotFound(Response<string>.Failure("User not found"));
            }

            var playlist = new Playlist
            {
                Name = dto.Name,
                UserId = userId,
                Songs = new List<Song>(),
                CreateDate = DateTime.UtcNow,
                LastUpdate = DateTime.UtcNow,
            };
            
            await playlistService.CreatePlaylistAsync(playlist);
            return Results.Ok(Response<Playlist>.Success(playlist,"Playlist created successfully"));

        }).DisableAntiforgery().RequireAuthorization();

        group.MapPut("/update/{playlistId}", async ([FromServices] PlaylistService playlistService, [FromServices] UserService userService, [FromForm] UpdatePlaylistDto dto, HttpContext context, string playlistId) =>
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Results.Unauthorized();
            }

            var user = await userService.GetUserById(userId);
            if(user is null)
            {
                return Results.NotFound(Response<string>.Failure("User not found"));
            }

            var playlist = await playlistService.GetPlaylistById(playlistId);

            if (!string.IsNullOrWhiteSpace(dto.Title))
            {
                playlist.Name = dto.Title;
            }

            playlist.LastUpdate = DateTime.UtcNow;

            await playlistService.UpdatePlaylistAsync(playlist);
            return Results.Ok(Response<Playlist>.Success(playlist,"Playlist created successfully"));

        }).DisableAntiforgery().RequireAuthorization();

        group.MapPut("/update/add/{playlistId}/{songId}", async ([FromServices] PlaylistService playlistService,  [FromServices] UserService userService, [FromServices] SongService songService, HttpContext context, string playlistId, string songId) =>
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Results.Unauthorized();
            }

            var user = await userService.GetUserById(userId);
            if(user is null)
            {
                return Results.NotFound(Response<string>.Failure("User not found"));
            }

            var playlist = await playlistService.GetPlaylistById(playlistId);

            var song = await songService.GetSongById(songId);
            if(song is null)
            {
                return Results.BadRequest(Response<string>.Failure("Invalid song")); 
            }

            playlist.Songs?.Add(song);
            playlist.LastUpdate = DateTime.UtcNow;

            await playlistService.UpdatePlaylistAsync(playlist);
            return Results.Ok(Response<Playlist>.Success(playlist,"Song added successfully"));

        }).DisableAntiforgery().RequireAuthorization();

        group.MapPut("/update/remove/{playlistId}/{songId}", async ([FromServices] PlaylistService playlistService,  [FromServices] UserService userService, [FromServices] SongService songService, HttpContext context, string playlistId, string songId) =>
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Results.Unauthorized();
            }

            var user = await userService.GetUserById(userId);
            if(user is null)
            {
                return Results.NotFound(Response<string>.Failure("User not found"));
            }

            var playlist = await playlistService.GetPlaylistById(playlistId);

            var song = await songService.GetSongById(songId);
            if(song is null)
            {
                return Results.BadRequest(Response<string>.Failure("Invalid song")); 
            }

            playlist.Songs?.Remove(song);
            playlist.LastUpdate = DateTime.UtcNow;
            
            await playlistService.UpdatePlaylistAsync(playlist);
            return Results.Ok(Response<Playlist>.Success(playlist,"Song added successfully"));  
        }).DisableAntiforgery().RequireAuthorization();

        group.MapDelete("/delete/{playlistId}", async ([FromServices] PlaylistService playlistService, [FromServices] UserService userService, HttpContext context, string playlistId) =>
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Results.Unauthorized();
            }

            var user = await userService.GetUserById(userId);
            if(user is null)
            {
                return Results.NotFound(Response<string>.Failure("User not found"));
            }

            var playlist = await playlistService.GetPlaylistById(playlistId);

            if(playlist is null)
            {
                return Results.NotFound(Response<string>.Failure("Playlist not found"));
            }

            await playlistService.DeletePlaylistAsync(playlist);
            return Results.Ok(Response<string>.Success("Playlist deleted successfully"));

        }).RequireAuthorization();

        group.MapGet("/myplaylist", async ([FromServices] PlaylistService playlistService, [FromServices] UserService userService, HttpContext context) =>
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Results.Unauthorized();
            }

            var user = await userService.GetUserById(userId);
            if(user is null)
            {
                return Results.NotFound(Response<string>.Failure("User not found"));
            }

            var playlists = await playlistService.GetMyPlaylist(user.Id);
            return Results.Ok(Response<List<Playlist>>.Success(playlists, "Playlists retrieved successfully"));

        }).RequireAuthorization();

        return app;
    }
}