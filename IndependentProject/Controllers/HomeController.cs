using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Supabase;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Supabase.Postgrest.Constants;

public class HomeController : Controller
{
    private readonly Client _supabaseClient;

    public HomeController(Client supabaseClient)
    {
        _supabaseClient = supabaseClient;
    }

    public async Task<IActionResult> Index()
    {
        // Fetch all movies and TV shows
        var allMoviesResponse = await _supabaseClient.From<Movies>().Get();
        var allMovies = allMoviesResponse.Models;

        // Fetch all ratings
        var ratingsResponse = await _supabaseClient.From<Ratings>().Get();
        var ratings = ratingsResponse.Models;

        // Normalize ShowIds to avoid mismatches
        foreach (var movie in allMovies)
        {
            movie.ShowId = movie.ShowId.ToLower();
        }

        foreach (var rating in ratings)
        {
            rating.ShowId = rating.ShowId.ToLower();
        }

        // Debug: Log total movies and ratings
        //Console.WriteLine($"Total Movies: {allMovies.Count}");
        //Console.WriteLine($"Total Ratings: {ratings.Count}");

        // Precompute average ratings
        var avgRatings = ratings
            .GroupBy(r => r.ShowId)
            .ToDictionary(g => g.Key, g => g.Average(r => r.Rating));

        // Assign average ratings to movies
        foreach (var movie in allMovies)
        {
            var matchedRatings = ratings.Where(r => r.ShowId == movie.ShowId).ToList();
            if (matchedRatings.Count == 0)
            {
                //Console.WriteLine($"No ratings found for Movie: {movie.Title} (ShowId: {movie.ShowId})");
            }
            else
            {
                Console.WriteLine($"Ratings found for Movie: {movie.Title} (ShowId: {movie.ShowId}): {string.Join(", ", matchedRatings.Select(r => r.Rating))}");
                
                int[] arr = matchedRatings.Select(r => r.Rating).ToArray();
                int sum = 0;
                for ( var i = 0; i < arr.Length; i++ ) {
                    sum += arr[i];
                }
                int avg = sum / arr.Length;
                movie.AverageRating = avg;
            }
        }

        Console.WriteLine(allMovies.Count.ToString());

        // Filter top-rated movies
        var topRatedMovies = allMovies
            .Where(m => m.Type == "Movie" && m.AverageRating > 0) // Ensure movies have ratings
            .OrderByDescending(m => m.AverageRating)
            .Take(9) // Increased limit for debugging
            .ToList();

        // Debug: Log top-rated movies
        Console.WriteLine($"Top Rated Movies Count: {topRatedMovies.Count}");

        // Filter top-rated TV shows
        var topRatedTvShows = allMovies
            .Where(m => m.Type == "TV Show" && m.AverageRating > 0) // Ensure shows have ratings
            .OrderByDescending(m => m.AverageRating)
            .Take(9) // Increased limit for debugging
            .ToList();

        // Debug: Log top-rated TV shows
        Console.WriteLine($"Top Rated TV Shows Count: {topRatedTvShows.Count}");

        // Add any other movie lists for your existing logic
        var latestMovies = allMovies
            .Where(m => m.Type == "Movie")
            .OrderByDescending(m => m.ReleaseYear)
            .Take(9)
            .ToList();

        var latestTvShows = allMovies
            .Where(m => m.Type == "TV Show")
            .OrderByDescending(m => m.ReleaseYear)
            .Take(9)
            .ToList();

        // Pass data to the view model
        var viewModel = new LatestContentViewModel
        {
            TopRatedMovies = topRatedMovies,
            TopRatedTvShows = topRatedTvShows,
            LatestMovies = latestMovies,
            LatestTvShows = latestTvShows
        };

        return View(viewModel);
    }

    public async Task<IActionResult> Details(string id)
    {
        // Normalize ShowId for lookup
        id = id.Trim().ToLower();

        // Retrieve the movie or show based on the ShowId
        var movieResponse = await _supabaseClient
            .From<Movies>()
            .Filter("show_id", Operator.Equals, id)
            .Get();

        var movie = movieResponse.Models.FirstOrDefault();

        if (movie == null)
        {
            return NotFound("Content not found.");
        }

        // Retrieve reviews for this movie or show
        var reviewResponse = await _supabaseClient
            .From<Ratings>()
            .Filter("Show_Id", Operator.Equals, id)
            .Get();

        var reviews = reviewResponse.Models;

        // Pass the content and reviews data to the view
        var viewModel = new MovieWithRatingsViewModel
        {
            Movie = movie,
            Ratings = reviews
        };

        return View(viewModel);
    }
}
