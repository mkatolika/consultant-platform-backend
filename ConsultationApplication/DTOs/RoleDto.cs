using System.ComponentModel.DataAnnotations;

namespace ConsultationApplication.DTOs;

public class RoleDto
{
    [Required, RegularExpression("^(Admin|Consultant|User)$", ErrorMessage = "Role must be Admin, Consultant, or User.")]
    public string Role { get; set; } = string.Empty;
}