using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace API.Models;
public class Album
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public string? Title { get; set; }
    public string? ArtistId { get; set; }
    public DateTime ReleaseDate { get; set; }
    public string? Image { get; set; }
    public List<Song>? Songs { get; set; }
    public DateTime LastUpdate {get; set;}
}