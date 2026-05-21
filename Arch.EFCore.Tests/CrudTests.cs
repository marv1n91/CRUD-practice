using Arch.EFCore;
using Xunit;

namespace Arch.EFCore.Tests;

public class CRUDTests
{
    public CRUDTests()
    {
        using var db = new DataContext();
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();
    }

    // Проверяет создание заметки
    [Theory]
    [InlineData("First note")]
    [InlineData("")]
    [InlineData("123")]
    public void Create_PassValid_Success(string text)
    {
        using var db = new DataContext();
        var user = UserCrud.Create("User").Result;

        NoteCrud.Create(text, DateTimeOffset.UtcNow, user.Id).Wait();

        var result = db.Notes.Select(x => x.Text).Contains(text);

        Assert.True(result);
    }

    // Проверяет поиск заметок
    [Theory]
    [InlineData("EF")]
    [InlineData("")]
    [InlineData("4")]
    public void Read_PassValid_Success(string search)
    {
        using var db = new DataContext();
        var user = new User
        {
            Name = "User"
        };
        db.Users.Add(user);
        db.SaveChanges();

        db.Notes.Add(new Note
        {
            Text = search,
            CreatedAt = DateTimeOffset.UtcNow,
            UserId = user.Id
        });
        db.SaveChanges();

        var result = NoteCrud.Read(search).Result.Select(x => x.Text).Contains(search);

        Assert.True(result);
    }

    // Проверяет поиск отсутствующей заметки
    [Theory]
    [InlineData("Value")]
    [InlineData("5")]
    public void Read_PassError_Fail(string search)
    {
        using var db = new DataContext();
        var result = NoteCrud.Read(search).Result.Select(x => x.Text).Contains(search);

        Assert.False(result);
    }

    // Проверяет обновление заметки
    [Theory]
    [InlineData("Changed note")]
    [InlineData("6")]
    public void Update_PassValid_Success(string changes)
    {
        using var db = new DataContext();
        var user = new User
        {
            Name = "User"
        };
        db.Users.Add(user);
        db.SaveChanges();

        var record = new Note
        {
            Text = "Old text",
            CreatedAt = DateTimeOffset.UtcNow,
            UserId = user.Id
        };
        db.Notes.Add(record);
        db.SaveChanges();

        NoteCrud.Update(record, changes, DateTimeOffset.UtcNow).Wait();
        var result = db.Notes.Select(x => x.Text).Contains(changes);

        Assert.True(result);
    }

    // Проверяет удаление заметки
    [Theory]
    [InlineData("Deleted note")]
    [InlineData("7")]
    public void Delete_PassValid_Success(string search)
    {
        using var db = new DataContext();
        var user = new User
        {
            Name = "User"
        };
        db.Users.Add(user);
        db.SaveChanges();

        var record = new Note
        {
            Text = search,
            CreatedAt = DateTimeOffset.UtcNow,
            UserId = user.Id
        };
        db.Notes.Add(record);
        db.SaveChanges();

        NoteCrud.Delete(record).Wait();
        var result = db.Notes.Select(x => x.Text).Contains(search);

        Assert.False(result);
    }

    // Проверяет создание пользователя
    [Theory]
    [InlineData("Ivan")]
    [InlineData("Anna")]
    public void CreateUser_PassValid_Success(string name)
    {
        using var db = new DataContext();
        UserCrud.Create(name).Wait();

        var result = db.Users.Select(x => x.Name).Contains(name);

        Assert.True(result);
    }

    // Проверяет поиск пользователя
    [Theory]
    [InlineData("Ivan")]
    [InlineData("Anna")]
    public void ReadUser_PassValid_Success(string search)
    {
        using var db = new DataContext();
        db.Users.Add(new User
        {
            Name = search
        });
        db.SaveChanges();

        var result = UserCrud.Read(search).Result.Select(x => x.Name).Contains(search);

        Assert.True(result);
    }

    // Проверяет обновление пользователя
    [Theory]
    [InlineData("New name")]
    [InlineData("User 2")]
    public void UpdateUser_PassValid_Success(string name)
    {
        using var db = new DataContext();
        var user = new User
        {
            Name = "Old name"
        };
        db.Users.Add(user);
        db.SaveChanges();

        UserCrud.Update(user, name).Wait();
        var result = db.Users.Select(x => x.Name).Contains(name);

        Assert.True(result);
    }

    // Проверяет удаление пользователя
    [Theory]
    [InlineData("Deleted user")]
    [InlineData("User 3")]
    public void DeleteUser_PassValid_Success(string name)
    {
        using var db = new DataContext();
        var user = new User
        {
            Name = name
        };
        db.Users.Add(user);
        db.SaveChanges();

        UserCrud.Delete(user).Wait();
        var result = db.Users.Select(x => x.Name).Contains(name);

        Assert.False(result);
    }

    // Проверяет получение всех заметок конкретного пользователя
    [Fact]
    public void ReadNotesByUser_PassValid_Success()
    {
        using var db = new DataContext();
        var firstUser = new User
        {
            Name = "First user"
        };
        var secondUser = new User
        {
            Name = "Second user"
        };

        db.Users.Add(firstUser);
        db.Users.Add(secondUser);
        db.SaveChanges();

        db.Notes.Add(new Note
        {
            Text = "First note",
            CreatedAt = DateTimeOffset.UtcNow,
            UserId = firstUser.Id
        });
        db.Notes.Add(new Note
        {
            Text = "Second note",
            CreatedAt = DateTimeOffset.UtcNow,
            UserId = firstUser.Id
        });
        db.Notes.Add(new Note
        {
            Text = "Other note",
            CreatedAt = DateTimeOffset.UtcNow,
            UserId = secondUser.Id
        });
        db.SaveChanges();

        var result = UserCrud.ReadNotes(firstUser.Id).Result;

        Assert.Equal(2, result.Count);
        Assert.True(result.Select(x => x.Text).Contains("First note"));
        Assert.True(result.Select(x => x.Text).Contains("Second note"));
        Assert.False(result.Select(x => x.Text).Contains("Other note"));
    }
}
