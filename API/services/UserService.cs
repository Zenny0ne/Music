using API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace API.Services;

public class UserService
{
    private readonly IMongoCollection<User> _users;
    private readonly PasswordHasher<User> _passwordHasher;

    public UserService(IOptions<MongoDbSettings> mongoDbSettings)
    {
        var settings = mongoDbSettings.Value;
        var client = new MongoClient(settings.ConnectionString);
        var database = client.GetDatabase(settings.DatabaseName);
        _users = database.GetCollection<User>(settings.UsersCollectionName);
        _passwordHasher = new PasswordHasher<User>();
    }

    public async Task<User?> GetUserById(string id)
    {
        return await _users.Find(u => u.Id == id).FirstOrDefaultAsync();
    }

    public async Task<User?> GetUserByEmail(string email)
    {
        return await _users.Find(u => u.Email == email).FirstOrDefaultAsync();
    }

    public async Task CreateUserAsync(User user, string password)
    {
        user.PasswordHash = _passwordHasher.HashPassword(user, password);
        await _users.InsertOneAsync(user);
    }

    public bool VerifyPassword(User user, string password)
    {
        var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash ?? string.Empty, password);
        return verificationResult == PasswordVerificationResult.Success;
    }

    public async Task UpdateUserAsync(User user)
    {
        await _users.ReplaceOneAsync(u => u.Id == user.Id, user);
    }

    public async Task DeleteUser(User user)
    {
        await _users.DeleteOneAsync(u => u.Id == user.Id);
    }
}

