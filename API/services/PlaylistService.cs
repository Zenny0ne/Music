using System.Security.Claims;
using API.DTOs;
using API.Models;
using API.Services;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

public class PlaylistService
{
    private readonly IMongoCollection<Playlist> _playlists;
    private readonly IMongoCollection<Song> _songs;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserService _userService;
    public PlaylistService(IOptions<MongoDbSettings> mongoDbSettings, IHttpContextAccessor httpContextAccessor, UserService userService)
    {
        var settings = mongoDbSettings.Value;
        var client = new MongoClient(settings.ConnectionString);
        var database = client.GetDatabase(settings.DatabaseName);
        _playlists = database.GetCollection<Playlist>(settings.PlaylistsCollectionName);
        _songs = database.GetCollection<Song>(settings.SongsCollectionName);
        _httpContextAccessor = httpContextAccessor;
        _userService = userService;
    }

    public async Task CreatePlaylistAsync(Playlist playlist)
    {
        await _playlists.InsertOneAsync(playlist);
    }

    public async Task<Playlist> GetPlaylistById(string id)
    {
        return await _playlists.Find(p => p.Id == id).FirstOrDefaultAsync();
    }

    public async Task UpdatePlaylistAsync(Playlist playlist){
        await _playlists.ReplaceOneAsync(s => s.Id == playlist.Id, playlist);
    }

    public async Task DeletePlaylistAsync(Playlist playlist)
    {
        await _playlists.DeleteOneAsync(s => s.Id == playlist.Id);
    }

    public async Task<List<Playlist>> GetMyPlaylist(string userId)
    {
        return await _playlists.Find(p => p.UserId == userId).ToListAsync();
    }

}