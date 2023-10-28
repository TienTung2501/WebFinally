using Microsoft.AspNetCore.Identity;

namespace Web.Models
{
    public class AppUser :IdentityUser
    {
        public ICollection<TblOrder> Orders { get; set; }
    }
}
