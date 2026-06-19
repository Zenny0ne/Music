using API.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using NAudio.Wave;

namespace API.Services;

public class SongService
{
    private readonly IMongoCollection<Song> _songs;

    public SongService(IOptions<MongoDbSettings> mongoDbSettings)
    {
        var settings = mongoDbSettings.Value;
        var client = new MongoClient(settings.ConnectionString);
        var database = client.GetDatabase(settings.DatabaseName);
        _songs = database.GetCollection<Song>(settings.SongsCollectionName);
    }

    public TimeSpan GetAudioDuration(string filePath)
    {
        using (var audioFileReader = new AudioFileReader(filePath))
        {
            return audioFileReader.TotalTime;
        }
    }

    public async Task<List<Song>> GetMySongs(Artist artist)
    {
        return await _songs.Find(s => s.ArtistId == artist.Id).ToListAsync();
    }

    public async Task<List<Song>> GetAllSongs()
    {
        return await _songs.Find(Builders<Song>.Filter.Empty).ToListAsync();
    }

    public async Task<Song> GetSongById(string id)
    {
        return await _songs.Find(s => s.Id == id).FirstOrDefaultAsync();
    }

    public async Task UpdateSongAsync(Song song){
        await _songs.ReplaceOneAsync(s => s.Id == song.Id, song);
    }

    public async Task DeleteSongAsync(Song song)
    {
        await _songs.DeleteOneAsync(s => s.Id == song.Id);
    }

    public async Task CreateSongAsync(Song song)
    {
        await _songs.InsertOneAsync(song);
    }

}
