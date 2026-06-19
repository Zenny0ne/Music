using API.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using NAudio.Wave;

namespace API.Services;

public class AlbumService
{
    private readonly IMongoCollection<Album> _albums;

    public AlbumService(IOptions<MongoDbSettings> mongoDbSettings)
    {
        var settings = mongoDbSettings.Value;
        var client = new MongoClient(settings.ConnectionString);
        var database = client.GetDatabase(settings.DatabaseName);
        _albums = database.GetCollection<Album>(settings.AlbumsCollectionName);
    }

    public async Task CreateAlbumAsync(Album album)
    {
        await _albums.InsertOneAsync(album);
    }

    public async Task<Album?> GetAlbumById(string id)
    {
        return await _albums.Find(a => a.Id == id).FirstOrDefaultAsync();
    }

    public async Task UpdateAlbumAsync(Album album)
    {
        await _albums.ReplaceOneAsync(u => u.Id == album.Id, album);
    }

    public async Task DeleteAlbum(Album album)
    {
        await _albums.DeleteOneAsync(u => u.Id == album.Id);
    }

}