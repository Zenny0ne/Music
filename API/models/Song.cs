namespace API.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Song
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public string? Title { get; set; }
    public string? ArtistId { get; set; }
    public string? AlbumId { get; set; }
    public TimeSpan Duration { get; set; }
    public DateTime ReleaseDate { get; set; }
    public string? Image { get; set; } = "uploads/picture/default_audio.png";
    public string? Audio { get; set; }
}