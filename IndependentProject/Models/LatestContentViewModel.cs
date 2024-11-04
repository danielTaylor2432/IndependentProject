using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
//using System.ComponentModel.DataAnnotations.Schema;  // Make sure this namespace is correct


public class LatestContentViewModel
{
    public List<Movies> LatestMovies { get; set; }
    public List<Movies> LatestTvShows { get; set; }
}
