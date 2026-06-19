using API.DTOs;
using API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace API.Services;

public class ArtistService
{
    private readonly IMongoCollection<Artist> _artists;
    private readonly PasswordHasher<Artist> _passwordHasher;

    public ArtistService(IOptions<MongoDbSettings> mongoDbSettings)
    {
        var settings = mongoDbSettings.Value;
        var client = new MongoClient(settings.ConnectionString);
        var database = client.GetDatabase(settings.DatabaseName);
        _artists = database.GetCollection<Artist>(settings.ArtistsCollectionName);
        _passwordHasher = new PasswordHasher<Artist>();
    }

    public async Task<Artist?> GetArtistById(string id)
    {
        return await _artists.Find(u => u.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Artist?> GetArtistByEmail(string email)
    {
        return await _artists.Find(u => u.Email == email).FirstOrDefaultAsync();
    }

    public async Task CreateArtistAsync(Artist Artist, string password)
    {
        Artist.PasswordHash = _passwordHasher.HashPassword(Artist, password);
        await _artists.InsertOneAsync(Artist);
    }

    public bool VerifyPassword(Artist Artist, string password)
    {
        var verificationResult = _passwordHasher.VerifyHashedPassword(Artist, Artist.PasswordHash ?? string.Empty, password);
        return verificationResult == PasswordVerificationResult.Success;
    }
    public async Task UpdateArtistAsync(Artist artist)
    {
        await _artists.ReplaceOneAsync(u => u.Id == artist.Id, artist);
    }

    public async Task DeleteArtist(Artist artist)
    {
        await _artists.DeleteOneAsync(u => u.Id == artist.Id);
    }
}
