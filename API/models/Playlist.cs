namespace API.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
public class Playlist
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? UserId { get; set; }
    public List<Song>? Songs { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime LastUpdate { get; set; }
}