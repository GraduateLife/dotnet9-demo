using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApiProject.Data;

[Table("Cool todo app")]
public class Todo
{
    // [Key]
    [Column("TodoId")]
    public int Id { get; set; }
    
    public string Title { get; set; }
    
    public string Description { get; set; }
    public bool IsCompleted { get; set; }
    
    
}

public class TodoCreateDto
{
    [Required(ErrorMessage = "Title is required")]
    // [MinLength(1)]
    [RegularExpression("^[a-zA-Z]+$", ErrorMessage = "letters only")]
    public string Title { get; set; }
    
    [Required(ErrorMessage = "Description is required")]
    [MaxLength(50,ErrorMessage = "max length is 50")]
    public string Description { get; set; }
}