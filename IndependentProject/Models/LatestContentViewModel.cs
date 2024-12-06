using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System.Collections.Generic;

public class LatestContentViewModel
{
    public List<Movies> LatestMovies { get; set; }
    public List<Movies> LatestTvShows { get; set; }
    public List<Movies> TopRatedMovies { get; set; } // New property for top-rated movies
    public List<Movies> TopRatedTvShows { get; set; } // New property for top-rated tv shows
    public Dictionary<string, IEnumerable<Movies>> Categories { get; set; } // this is for scalability



}
