using Microsoft.AspNetCore.Identity;

namespace Web.ViewModels
{
    public class AppUser :IdentityUser
    {
        public ICollection<TblOrder> Orders { get; set; }
    }
}
