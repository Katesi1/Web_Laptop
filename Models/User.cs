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

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public User() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    public User(string userName, string password , string email)
    {
        UserName = userName;
        Password = password;
        Email = email;
        
    }
}
