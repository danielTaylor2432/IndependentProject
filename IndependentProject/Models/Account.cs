using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
//using System.ComponentModel.DataAnnotations.Schema;  // Make sure this namespace is correct

public class Account : BaseModel
{

    [PrimaryKey("Id")]  // Specify the primary key column name in your table
    public int Id { get; set; }

    [Column("Name")]
    public string FirstName { get; set; }
    // Add other properties based on your table's schema
    [Column("Email")]
    public string Email { get; set; }

    [Column("LastName")]
    public string LastName { get; set; }

    [Column("Password")]
    public string Password { get; set; }

    [Column("PhoneNumber")]
    public string PhoneNumber { get; set; }

}
