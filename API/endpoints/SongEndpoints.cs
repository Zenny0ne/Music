using System.Security.Claims;
using System.IO;
using API.Common;
using API.DTOs;
using API.Models;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.endpoints;

public static class SongEndpoints
{
    public static WebApplication MapSongEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/song");

        group.MapPost("/song", async([FromServices] ArtistService artistService, [FromServices] SongService songService, [FromForm] CreateSongDto songDto, IFormFile? songImage, IFormFile? audioFile, HttpContext context) =>
        {
            var artistId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(artistId))
            {
                return Results.Unauthorized();
            }
            var artist = await artistService.GetArtistById(artistId);
            if(artist is null)
            {
                return Results.NotFound(Response<string>.Failure("User not found"));
            }
            if(artist.isArtist == false)
            {
                return Results.Unauthorized();
            }

            var songImagePath = "/uploads/picture/default_audio.png";
            if (songImage is not null)
            {
                var validImageTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
                if (!validImageTypes.Contains(songImage.ContentType))
                {
                    return Results.BadRequest(Response<string>.Failure("Invalid image format. Allowed: JPEG, PNG, GIF, WebP"));
                }
                var fileName = await FileUploads.Upload(songImage);
                songImagePath = $"/uploads/picture/{fileName}";
            }

            var songAudioPath = "/uploads/audio";
            if (audioFile is null)
            {
                return Results.BadRequest(Response<string>.Failure("Invalid file audio"));
            }
            
            if (audioFile.ContentType != "audio/mpeg" && !audioFile.FileName.EndsWith(".mp3"))
            {
                return Results.BadRequest(Response<string>.Failure("Invalid audio format. Only MP3 files are allowed"));
            }
            
            var songName = await FileUploads.Upload(audioFile);
            songAudioPath = $"/uploads/audio/{songName}";
            var physicalAudioPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads", "audio", songName);

            var song = new Song
            {
                Title = songDto.Title,
                ArtistId = artistId,
                Image = songImagePath,
                Audio = songAudioPath,
                Duration = songService.GetAudioDuration(physicalAudioPath),
                ReleaseDate = DateTime.UtcNow,
                AlbumId = null
            };

            await songService.CreateSongAsync(song);
            artist.Singles?.Add(song);
            await artistService.UpdateArtistAsync(artist);
            return Results.Ok(Response<Song>.Success(song,"Song created successfully"));
        }).DisableAntiforgery().RequireAuthorization();

        group.MapGet("/mysongs", async ([FromServices] ArtistService artistService, [FromServices] SongService songService, HttpContext context) =>
        {
            var artistId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(artistId))
            {
                return Results.Unauthorized();
            }

            var artist = await artistService.GetArtistById(artistId);
            if (artist is null)
            {
                return Results.NotFound(Response<string>.Failure("User not found"));
            }

            if(artist.isArtist == false){
                return Results.Unauthorized();
            }

            var songs = await songService.GetMySongs(artist);

            return Results.Ok(Response<List<Song>>.Success(songs, "Songs retrieved successfully"));

        }).RequireAuthorization();

        group.MapGet("/songs", async ([FromServices] SongService songService) =>
        {
            var songs = await songService.GetAllSongs();
            return Results.Ok(Response<List<Song>>.Success(songs, "All songs retrieved"));
        });

        group.MapPut("/update/{songId}", async ([FromServices] ArtistService artistService, [FromServices] SongService songService, [FromForm] UpdateSongDto songDto, IFormFile? songImage, IFormFile? audioFile, HttpContext context, string songId) =>
        {
            if(songDto is null)
            {
                return Results.BadRequest(Response<string>.Failure("Invalid update request"));
            }

            var artistId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(artistId))
            {
                return Results.Unauthorized();
            }

            var artist = await artistService.GetArtistById(artistId);
            if (artist is null)
            {
                return Results.NotFound(Response<string>.Failure("User not found"));
            }

            if(artist.isArtist == false){
                return Results.Unauthorized();
            }

            var song = await songService.GetSongById(songId);

            if(!string.IsNullOrWhiteSpace(songDto.Title)){
                song.Title = songDto.Title;
            }

            if(songImage is not null){
                var validImageTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
                if (!validImageTypes.Contains(songImage.ContentType))
                {
                    return Results.BadRequest(Response<string>.Failure("Invalid image format. Allowed: JPEG, PNG, GIF, WebP"));
                }
                var fileName = await FileUploads.Upload(songImage);
                song.Image = $"/uploads/picture/{fileName}";
            }

            if(audioFile is not null){
                if (audioFile.ContentType != "audio/mpeg" && !audioFile.FileName.EndsWith(".mp3"))
                {
                    return Results.BadRequest(Response<string>.Failure("Invalid audio format. Only MP3 files are allowed"));
                }
                
                var songName = await FileUploads.Upload(audioFile);
                song.Audio = $"/uploads/audio/{songName}";
                var physicalAudioPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads", "audio", songName);
                song.Duration = songService.GetAudioDuration(physicalAudioPath);
            }
            
            await songService.UpdateSongAsync(song);
            return Results.Ok(Response<Song>.Success(song,"Song updated successfully"));

        }).DisableAntiforgery().RequireAuthorization();

        group.MapDelete("/delete/{songId}", async ([FromServices] ArtistService artistService, [FromServices] SongService songService, HttpContext context, string songId) =>
        {
            var artistId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(artistId))
            {
                return Results.Unauthorized();
            }

            var artist = await artistService.GetArtistById(artistId);
            if (artist is null)
            {
                return Results.NotFound(Response<string>.Failure("User not found"));
            }

            if (artist.isArtist == false)
            {
                return Results.Unauthorized();
            }

            var song = await songService.GetSongById(songId);
            if (song is null)
            {
                return Results.NotFound(Response<string>.Failure("Song not found"));
            }

            if (song.ArtistId != artistId)
            {
                return Results.Forbid();
            }

            await songService.DeleteSongAsync(song);
            return Results.Ok(Response<string>.Success("Song deleted successfully"));

        }).RequireAuthorization();

        return app;
    }
}