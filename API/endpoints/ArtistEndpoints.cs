using API.Common;
using API.DTOs;
using API.Models;
using API.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.endpoints;

public static class ArtistEndPoints
{
    public static WebApplication MapArtistEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/artist");

        group.MapPost("/register", async([FromServices] ArtistService artistService, [FromForm] RegisterDto dto, IFormFile? profileImage) =>
        {
            if(dto is null)
            {
                return Results.BadRequest(Response<string>.Failure("Invalid register request"));
            }
            
            var userFromDb = await artistService.GetArtistByEmail(dto.Email);
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

            var artist = new Artist
            {
                FullName = dto.FullName,
                UserName = dto.UserName,
                Email = dto.Email,
                ProfileImage = profileImagePath,
                Albums = new List<Album>(),
                Singles = new List<Song>()
            };

            await artistService.CreateArtistAsync(artist, dto.Password);
            return Results.Ok(Response<Artist>.Success(artist, "Registered successfully"));
        }).DisableAntiforgery();

        group.MapPost("/login", async([FromServices] ArtistService artistService, TokenService tokenService, [FromForm] LoginDto dto) =>
        {
            if(dto is null)
            {
                return Results.BadRequest(Response<string>.Failure("Invalid login request"));
            }

            var artist = await artistService.GetArtistByEmail(dto.Email);

            if(artist is null)
            {
                return Results.BadRequest(Response<string>.Failure("User not found"));
            }

            var isPasswordValid = artistService.VerifyPassword(artist, dto.Password);
            if (!isPasswordValid)
            {
                return Results.BadRequest(Response<string>.Failure("Invalid password"));
            }

            var token = tokenService.GenerateToken(artist.Id, artist.UserName!);
            return Results.Ok(Response<string>.Success(token, "Login successful"));
        }).DisableAntiforgery();

        group.MapGet("/profile", async([FromServices] ArtistService artistService, HttpContext context) =>
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

            return Results.Ok(Response<Artist>.Success(artist, "Profile loaded"));
        }).RequireAuthorization();

        group.MapPut("/profile/update", async([FromServices] ArtistService artistService, [FromForm] UpdateArtistDto dto, IFormFile? profileImage, HttpContext context) =>
        {
            if(dto is null)
            {
                return Results.BadRequest(Response<string>.Failure("Invalid update request"));
            }

            var artistId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(artistId))
            {
                return Results.Unauthorized();
            }

            var user = await artistService.GetArtistById(artistId);
            if (user is null)
            {
                return Results.NotFound(Response<string>.Failure("User not found"));
            }

            if (user.isArtist == false)
            {
                return Results.Unauthorized();
            }

            if (!string.IsNullOrWhiteSpace(dto.Email) && dto.Email != user.Email)
            {
                var existing = await artistService.GetArtistByEmail(dto.Email);
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

            await artistService.UpdateArtistAsync(user);
            return Results.Ok(Response<Artist>.Success(user, "Profile updated"));
        }).DisableAntiforgery().RequireAuthorization();

        group.MapDelete("/profile/delete", async([FromServices] ArtistService artistService, HttpContext context) =>
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

            await artistService.DeleteArtist(artist);
            return Results.Ok(Response<string>.Success("", "Artist deleted successfully"));
        }).RequireAuthorization();

        return app;
    }
}