using System.ComponentModel.DataAnnotations;
namespace Web.ViewModels
{
    public class UserRoleViewModel
    {
        public string UserId { get; set; }
        public string? UserName { get; set; }
        public string UserEmail{ get; set; }
        public string RoleId { get; set; }
        public string? RoleName { get; set; }
    }
}
