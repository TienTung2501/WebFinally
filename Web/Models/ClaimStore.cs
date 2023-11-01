using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;

namespace Web.ViewModels
{
    [NotMapped]// không thêm model này vào csdl khi migration
    public class ClaimStore
    {
        public static List<Claim> claimsList = new()
    {
        new Claim("Create", "Create"),
        new Claim("Edit", "Edit"),
        new Claim("Delete", "Delete")
    };
    }
}
