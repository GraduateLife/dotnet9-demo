namespace WebApiProject.Data;

public class Todo
{
    public int Id { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public bool IsCompleted { get; set; }

    public DateTime StartAtUtc { get; set; }

    public DateTime DueAtUtc { get; set; }
}
