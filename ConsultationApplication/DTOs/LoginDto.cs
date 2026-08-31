using System.ComponentModel.DataAnnotations;

namespace ConsultationApplication.DTOs;

public class LoginDto
{
    [Required, StringLength(100)]
    public string Username { get; set; } = string.Empty;

    [Required, DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}