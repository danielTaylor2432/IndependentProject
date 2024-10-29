using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
//using System.ComponentModel.DataAnnotations.Schema;  // Make sure this namespace is correct



public class MovieWithRatingsViewModel
{
    public Movies Movie { get; set; }
    public List<Ratings> Ratings { get; set; }

    public MovieWithRatingsViewModel()
    {
        Ratings = new List<Ratings>();
    }
}
