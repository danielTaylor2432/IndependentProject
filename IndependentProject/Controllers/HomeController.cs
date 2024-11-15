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

        // Debugging: Check total counts
        Console.WriteLine("Total Movies Fetched: " + allMovies.Count);
        Console.WriteLine("Total Ratings Fetched: " + ratings.Count);

        // Calculate average ratings for each ShowId
        var averageRatings = ratings
            .GroupBy(r => r.ShowId)
            .Select(g => new
            {
                ShowId = g.Key,
                AverageRating = g.Average(r => r.Rating)
            })
            .ToList();

        // Filter top 5 highest-rated movies
        var topRatedMovieIds = averageRatings
            .Where(r => allMovies.Any(m => m.ShowId == r.ShowId && m.Type == "Movie"))
            .OrderByDescending(r => r.AverageRating)
            .Take(5)
            .Select(r => r.ShowId)
            .ToList();

        // Filter top 5 highest-rated TV shows
        var topRatedTvShowIds = averageRatings
            .Where(r => allMovies.Any(tv => tv.ShowId == r.ShowId && tv.Type == "TV Show"))
            .OrderByDescending(r => r.AverageRating)
            .Take(5)
            .Select(r => r.ShowId)
            .ToList();

        // Fetch and filter all movies and shows based on the calculated top-rated IDs
        var topRatedMovies = allMovies
            .Where(movie => topRatedMovieIds.Contains(movie.ShowId) && movie.Type == "Movie")
            .ToList();

        var topRatedTvShows = allMovies
            .Where(show => topRatedTvShowIds.Contains(show.ShowId) && show.Type == "TV Show")
            .ToList();

        // Debugging: Check counts of top-rated results
        Console.WriteLine("Top Rated Movies Count: " + topRatedMovies.Count);
        Console.WriteLine("Top Rated TV Shows Count: " + topRatedTvShows.Count);

        // Pass the data to the view model
        var viewModel = new LatestContentViewModel
        {
            TopRatedMovies = topRatedMovies,
            TopRatedTvShows = topRatedTvShows
        };

        return View(viewModel);
    }





    public async Task<IActionResult> Details(string id)
    {
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
