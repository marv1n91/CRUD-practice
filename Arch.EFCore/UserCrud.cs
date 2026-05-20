using Microsoft.EntityFrameworkCore;

namespace Arch.EFCore;

public class UserCrud
{
    public static async Task<User> Create(string name)
    {
        await using var db = new DataContext();
        var user = new User
        {
            Name = name
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();
        return user;
    }

    public static async Task<List<User>> Read(string search)
    {
        await using var db = new DataContext();
        var result = await db.Users
            .Where(x => EF.Functions.Like(x.Name, $"%{search}%"))
            .ToListAsync();
        return result;
    }

    public static async Task Update(User user, string name)
    {
        await using var db = new DataContext();
        user.Name = name;
        db.Users.Update(user);
        await db.SaveChangesAsync();
    }

    public static async Task Delete(User user)
    {
        await using var db = new DataContext();
        db.Users.Remove(user);
        await db.SaveChangesAsync();
    }

    public static async Task<List<Note>> ReadNotes(int userId)
    {
        await using var db = new DataContext();
        var result = await db.Notes
            .Where(x => x.UserId == userId)
            .ToListAsync();
        return result;
    }
}
