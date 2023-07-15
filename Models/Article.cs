using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Models;

public class Article
{
    [Key]
    public int Id { get; set; }
    [StringLength(255,MinimumLength =5, ErrorMessage ="{0} phai nhap tu {2} den {1}")]
    [Required(ErrorMessage ="Phai nhap {0}")]
    [Column(TypeName = "LONGTEXT")]
    [DisplayName("Tieu de")]
    public string Title { get; set; }
    [DataType(DataType.Date)]
    [Required(ErrorMessage ="Phai nhap {0}")]
     [DisplayName("Ngay tao")]
    public DateTime Created { get; set; }
    [Column(TypeName = "TEXT")]
     [DisplayName("Noi dung")]
    public string Content { get; set; }
}