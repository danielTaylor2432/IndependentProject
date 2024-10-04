using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;

public class Ratings : BaseModel
{
    [PrimaryKey("Rating_Id")]
    public int RatingID { get; set; }

    [Column("User_id")]
    public int UserId { get; set; }  // Foreign key linking to the user

    [Column("Show_Id")]
    public string ShowId { get; set; }  // Foreign key linking to the movie

    [Column("Rating")]
    public int Rating { get; set; }  // Rating out of 10 (1-5 stars with half-star increments)

    [Column("Recommend")]
    public string Recommend { get; set; }  // Whether the user recommends the movie (yes/no)

    [Column("Description")]
    public string Description { get; set; }  // Review description
}
