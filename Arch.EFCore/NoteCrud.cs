using Microsoft.EntityFrameworkCore;

namespace Arch.EFCore;

public class NoteCrud
{
    public static async Task<Note> Create(string text, DateTimeOffset createdAt, int userId)
    {
        await using var db = new DataContext();
        var note = new Note
        {
            Text = text,
            CreatedAt = createdAt,
            UserId = userId
        };

        db.Notes.Add(note);
        await db.SaveChangesAsync();
        return note;
    }

    public static async Task<List<Note>> Read(string search)
    {
        await using var db = new DataContext();
        var result = await db.Notes
            .Where(x => EF.Functions.Like(x.Text, $"%{search}%"))
            .ToListAsync();
        return result;
    }

    public static async Task Update(Note note, string text, DateTimeOffset createdAt)
    {
        await using var db = new DataContext();
        note.Text = text;
        note.CreatedAt = createdAt;
        db.Notes.Update(note);
        await db.SaveChangesAsync();
    }

    public static async Task Delete(Note note)
    {
        await using var db = new DataContext();
        db.Notes.Remove(note);
        await db.SaveChangesAsync();
    }
}
