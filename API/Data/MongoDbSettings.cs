public class MongoDbSettings
{
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
    public string UsersCollectionName { get; set; } = "Users";
    public string SongsCollectionName { get; set; } = "Songs";
    public string PlaylistsCollectionName { get; set; } = "Playlists";
    public string ArtistsCollectionName { get; set; } = "Artists";
    public string AlbumsCollectionName { get; set; } = "Albums";
}