using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
//using System.ComponentModel.DataAnnotations.Schema;  // Make sure this namespace is correct

public class Account : BaseModel
{

    [PrimaryKey("id")]  // Specify the primary key column name in your table
    public int Id { get; set; }

    [Column("Name")]
    public string Name { get; set; }
    // Add other properties based on your table's schema
}
