using API.Common;
using API.DTOs;
using API.Models;
using API.Services;
using Microsoft.AspNetCore.Mvc;
using System.IO.Compression;
using System.Security.Claims;

namespace API.endpoints;

public static class AlbumEndPoints
{
    public static WebApplication MapAlbumEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/album");

        group.MapPost("/album", async ([FromServices] AlbumService albumService, [FromServices] ArtistService artistService, [FromServices] SongService songService, HttpContext context, IFormFile? albumImage, IFormFile? zipFile, [FromForm] CreateAlbumDto dto ) =>
        {
            var artistId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(artistId))
            {
                return Results.Unauthorized();
            }
            var artist = await artistService.GetArtistById(artistId);
            if(artist is null)
            {
                return Results.NotFound(Response<string>.Failure("artist not found"));
            }
            if(artist.isArtist == false)
            {
                return Results.Unauthorized();
            }

            var albumImagePath = "/uploads/picture/default_album.png";
            if(albumImage is not null)
            {
                var validImageTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
                if (!validImageTypes.Contains(albumImage.ContentType))
                {
                    return Results.BadRequest(Response<string>.Failure("Invalid image format. Allowed: JPEG, PNG, GIF, WebP"));
                }
                var fileName = await FileUploads.Upload(albumImage);
                albumImagePath = $"/uploads/picture/{fileName}";
            }

            if (zipFile is not null)
            {
                if (!zipFile.FileName.EndsWith(".zip", StringComparison.OrdinalIgnoreCase)
                    && zipFile.ContentType != "application/zip"
                    && zipFile.ContentType != "application/x-zip-compressed")
                {
                    return Results.BadRequest(Response<string>.Failure("Invalid ZIP file."));
                }
            }

            var nextAlbumId = await albumService.GetNextAlbumId();
            var album = new Album
            {
                Id = nextAlbumId,
                Title = dto.Title,
                ArtistId = artistId,
                ReleaseDate = DateTime.UtcNow,
                Image = albumImagePath,
                Songs = new List<Song>(),
                LastUpdate = DateTime.UtcNow,
            };

            var createdSongs = new List<Song>();
            var nextSongId = await songService.GetNextSongId();

            if (zipFile is not null)
            {
                using var archive = new ZipArchive(zipFile.OpenReadStream(), ZipArchiveMode.Read, false);
                foreach (var entry in archive.Entries)
                {
                    if (string.IsNullOrEmpty(entry.Name))
                    {
                        continue;
                    }

                    if (!entry.Name.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    await using var entryStream = entry.Open();
                    var storedFileName = await FileUploads.UploadAudioStreamAsync(entryStream, entry.Name);
                    var audioPath = $"/uploads/audio/{storedFileName}";
                    var physicalAudioPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads", "audio", storedFileName);
                    var songDuration = songService.GetAudioDuration(physicalAudioPath);

                    var song = new Song
                    {
                        Id = nextSongId++,
                        Title = Path.GetFileNameWithoutExtension(entry.Name),
                        ArtistId = artistId,
                        AlbumId = album.Id,
                        Duration = songDuration,
                        ReleaseDate = DateTime.UtcNow,
                        Image = "/uploads/picture/default_audio.png",
                        Audio = audioPath
                    };

                    createdSongs.Add(song);
                    await songService.CreateSongAsync(song);
                }

                if (!createdSongs.Any())
                {
                    return Results.BadRequest(Response<string>.Failure("ZIP archive contains no MP3 files."));
                }
            }else{
                return Results.BadRequest(Response<string>.Failure("Songs required"));
            }

            album.Songs = createdSongs;
            await albumService.CreateAlbumAsync(album);
            return Results.Ok(Response<Album>.Success(album, "Album created successfully"));
        }).DisableAntiforgery().RequireAuthorization();

        group.MapPut("/update/albumId", async ([FromServices] AlbumService albumService, [FromServices] ArtistService artistService, [FromForm] UpdateAlbumDto dto, HttpContext context, int albumId) =>
        {
            var artistId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(artistId))
            {
                return Results.Unauthorized();
            }
            
            var artist = await artistService.GetArtistById(artistId);
            if(artist is null)
            {
                return Results.NotFound(Response<string>.Failure("artist not found"));
            }
           
            if(artist.isArtist == false)
            {
                return Results.Unauthorized();
            }

            var album = await albumService.GetAlbumById(albumId);
            if(album is null)
            {
                return Results.NotFound(Response<string>.Failure("Album not found"));
            }

            if(album.ArtistId != artistId){
                return Results.Unauthorized();
            }

            if (!string.IsNullOrWhiteSpace(dto.Title))
            {
                album.Title = dto.Title;
            }

            await albumService.UpdateAlbumAsync(album);
            return Results.Ok(Response<Album>.Success(album, "Album updated"));

        }).DisableAntiforgery().RequireAuthorization();

        group.MapPut("/update/add/{albumId}/{songId}", async ([FromServices] AlbumService albumService, [FromServices] ArtistService artistService, [FromServices] SongService songService, HttpContext context, int albumId, int songId) =>
        {
            var artistId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(artistId))
            {
                return Results.Unauthorized();
            }
            
            var artist = await artistService.GetArtistById(artistId);
            if(artist is null)
            {
                return Results.NotFound(Response<string>.Failure("artist not found"));
            }
           
            if(artist.isArtist == false)
            {
                return Results.Unauthorized();
            }

            var album = await albumService.GetAlbumById(albumId);
            if(album is null)
            {
                return Results.NotFound(Response<string>.Failure("Album not found"));
            }

            if(album.ArtistId != artistId){
                return Results.Unauthorized();
            }

            var song = await songService.GetSongById(songId);
            if(song is null){
                return Results.NotFound(Response<string>.Failure("Song not found"));
            }

            album.Songs?.Add(song);
            album.LastUpdate = DateTime.UtcNow;

            await albumService.UpdateAlbumAsync(album);
            return Results.Ok(Response<Album>.Success(album, "Album updated"));

        }).DisableAntiforgery().RequireAuthorization();

        group.MapPut("/update/remove/{albumId}/{songId}", async ([FromServices] AlbumService albumService, [FromServices] ArtistService artistService, [FromServices] SongService songService, HttpContext context, int albumId, int songId) =>
        {
            var artistId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(artistId))
            {
                return Results.Unauthorized();
            }

            var artist = await artistService.GetArtistById(artistId);
            if(artist is null)
            {
                return Results.NotFound(Response<string>.Failure("artist not found"));
            }

            var album = await albumService.GetAlbumById(albumId);
            if(album is null){
                return Results.BadRequest(Response<string>.Failure("Invalid album")); 
            }

            var song = await songService.GetSongById(songId);
            if(song is null)
            {
                return Results.BadRequest(Response<string>.Failure("Invalid song")); 
            }

            album.Songs?.Remove(song);
            album.LastUpdate = DateTime.UtcNow;
            
            await albumService.UpdateAlbumAsync(album);
            return Results.Ok(Response<Album>.Success(album,"Song removed successfully"));  
        }).DisableAntiforgery().RequireAuthorization();

        group.MapDelete("/album/delete/{albumId}", async ([FromServices] AlbumService albumService, [FromServices] ArtistService artistService, HttpContext context, int albumId) =>
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

            var album = await albumService.GetAlbumById(albumId);
            if (album is null)
            {
                return Results.NotFound(Response<string>.Failure("Album not found"));
            }

            if (album.ArtistId != artistId)
            {
                return Results.Forbid();
            }

            await albumService.DeleteAlbum(album);
            return Results.Ok(Response<string>.Success("Album deleted successfully"));
        }).RequireAuthorization();

        

        return app;
    }
}