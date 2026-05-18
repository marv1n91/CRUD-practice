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
        NoteCrud.Create(text, DateTimeOffset.UtcNow).Wait();

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
        db.Notes.Add(new Note
        {
            Text = search,
            CreatedAt = DateTimeOffset.UtcNow
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
        var record = new Note
        {
            Text = "Old text",
            CreatedAt = DateTimeOffset.UtcNow
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
        var record = new Note
        {
            Text = search,
            CreatedAt = DateTimeOffset.UtcNow
        };
        db.Notes.Add(record);
        db.SaveChanges();

        NoteCrud.Delete(record).Wait();
        var result = db.Notes.Select(x => x.Text).Contains(search);

        Assert.False(result);
    }
}
