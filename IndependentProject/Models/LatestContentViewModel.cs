using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System.Collections.Generic;

public class LatestContentViewModel
{
    public List<Movies> LatestMovies { get; set; }
    public List<Movies> LatestTvShows { get; set; }
    public List<Movies> TopRatedMovies { get; set; } // New property for top-rated movies

    public List<Movies> TopRatedTvShows { get; set; } // New property for top-rated tv shows

    //public IEnumerable<Movies> NewestTvShows { get; set; } // Newest TV Shows
    //public IEnumerable<Movies> NewestMovies { get; set; } // Newest Movies
    //public IEnumerable<Movies> NewestDramas { get; set; }  // Movies under "Drama" category                              
    //public IEnumerable<Movies> NewestComedies { get; set; }  // Movies under "Drama" category
    //public IEnumerable<Movies> NewestDocu { get; set; }  // Movies under "Drama" category
    public Dictionary<string, IEnumerable<Movies>> Categories { get; set; } // this is for scalability



}
