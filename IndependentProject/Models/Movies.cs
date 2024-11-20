using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

public class Movies : BaseModel
{
    [PrimaryKey("show_id")]  // The primary key for the table
    public string ShowId { get; set; }

    [Column("type")]
    public string Type { get; set; }  // Either "Movie" or "TV Show"

    [Column("title")]
    public string Title { get; set; }

    [Column("director")]
    public string Director { get; set; }

    [Column("cast")]
    public string Cast { get; set; }

    [Column("country")]
    public string Country { get; set; }

    [Column("date_added")]
    public string DateAdded { get; set; }

    [Column("release_year")]
    public int ReleaseYear { get; set; }

    [Column("rating")]
    public string Rating { get; set; }

    [Column("duration")]
    public string Duration { get; set; }

    [Column("listed_in")]
    public string ListedIn { get; set; }  // The category, e.g., "Drama, Action"

    [Column("description")]
    public string Description { get; set; }

    public double AverageRating { get; set; } // Add average rating
}
