using Microsoft.AspNetCore.Identity;

namespace auth.Entities;

public class User: IdentityUser
{
    public string FullName { get; set; }
    
    public DateTimeOffset Birthdate { get; set; }
}