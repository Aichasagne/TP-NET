using BooksApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Security.Cryptography;
using System.Text;

namespace BooksApi.Services;

public class AuthService
{
    private readonly IMongoCollection<User> _usersCollection;

    public AuthService(IOptions<BookStoreDatabaseSettings> settings)
    {
        var mongoClient = new MongoClient(settings.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(settings.Value.DatabaseName);
        _usersCollection = mongoDatabase.GetCollection<User>("Users");
    }

    public async Task<User?> GetByUsernameAsync(string username) =>
        await _usersCollection.Find(x => x.Username == username).FirstOrDefaultAsync();

    public async Task CreateAsync(User user) =>
        await _usersCollection.InsertOneAsync(user);

    public string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }

    public bool VerifyPassword(string password, string hash) =>
        HashPassword(password) == hash;
}