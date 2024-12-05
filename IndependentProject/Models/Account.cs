using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

public class Account : BaseModel
{

    [PrimaryKey("Id")]  
    public int Id { get; set; }

    [Column("Name")]
    public string FirstName { get; set; }

    [Column("Email")]
    public string Email { get; set; }

    [Column("LastName")]
    public string LastName { get; set; }

    [Column("Password")]
    public string Password { get; set; }

    [Column("PhoneNumber")]
    public string PhoneNumber { get; set; }

}
