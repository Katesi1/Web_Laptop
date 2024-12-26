using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcLaptop.Models;

public class User
{
    public int Id { get; set; }

    [Required]  // Chỉ ra rằng thuộc tính này là bắt buộc
    [StringLength(100)]
    public string UserName { get; set; }
    
    [Required]  // Chỉ ra rằng thuộc tính này là bắt buộc
    [StringLength(100)]
    public string Password { get; set; }

    [Required]  // Chỉ ra rằng thuộc tính này là bắt buộc
    [EmailAddress]  // Đảm bảo rằng giá trị là một email hợp lệ
    public string Email { get; set; }

    public User() { }

      public User(string userName, string password , string email)
    {
        UserName = userName;
        Password = password;
        Email = email;
        
    }
}
