using KCK_APP.Models;
using KCK_APP.Services;

namespace KCK_APP.Controllers;

public class UserController
{
    private readonly DatabaseService _db;
    public UserController(DatabaseService db) => _db = db;

    public void Register(string username, string password)
    {
        var hash = BCrypt.Net.BCrypt.HashPassword(password);
        var user = new User { Username = username, PasswordHash = hash, Role = "User" };
        _db.AddUser(user);
    }

    public User Authenticate(string username, string password)
    {
        var user = _db.GetUserByUsername(username);
        if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return user;
        return null;
    }
    public List<User> GetAllUsers()
    {
        return _db.GetAllUsers();
    }
}