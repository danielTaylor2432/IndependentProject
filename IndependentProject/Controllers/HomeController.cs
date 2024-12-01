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

        // Debug: Log the number of movies fetched
        Console.WriteLine($"Total Movies Fetched: {allMovies.Count}");

        // Fetch all ratings
        var ratingsResponse = await _supabaseClient.From<Ratings>().Get();
        var ratings = ratingsResponse.Models;

        // Debug: Log the number of ratings fetched
        Console.WriteLine($"Total Ratings Fetched: {ratings.Count}");

        // Normalize ShowIds to avoid mismatches
        foreach (var movie in allMovies)
        {
            movie.ShowId = movie.ShowId.ToLower();
        }

        foreach (var rating in ratings)
        {
            rating.ShowId = rating.ShowId.ToLower();
        }

        // Debug: Log normalized ShowIds
        Console.WriteLine("Normalized ShowIds:");
        foreach (var movie in allMovies)
        {
            Console.WriteLine($"Movie: {movie.Title}, ShowId: {movie.ShowId}");
        }

        foreach (var rating in ratings)
        {
            Console.WriteLine($"Rating: ShowId: {rating.ShowId}, Rating Value: {rating.Rating}");
        }

        // Assign average ratings to movies
        foreach (var movie in allMovies)
        {
            // Filter ratings specific to this movie
            var matchedRatings = ratings.Where(r => r.ShowId == movie.ShowId).ToList();

            if (matchedRatings.Count > 0)
            {
                // Debug: Log matched ratings for the current movie
                Console.WriteLine($"Matched Ratings for Movie '{movie.Title}' (ShowId: {movie.ShowId}): {string.Join(", ", matchedRatings.Select(r => r.Rating))}");

                // Calculate and assign the average rating
                movie.AverageRating = matchedRatings.Average(r => r.Rating);

                // Debug: Log the calculated average
                Console.WriteLine($"Average Rating for '{movie.Title}': {movie.AverageRating}");
            }
            else
            {
                // Debug: Log movies with no ratings
                Console.WriteLine($"No Ratings Found for Movie: {movie.Title} (ShowId: {movie.ShowId})");

                // Default to 0 if no ratings are found
                movie.AverageRating = 0;
            }
        }

        // Filter top-rated movies
        var topRatedMovies = allMovies
            .Where(m => m.Type == "Movie" && m.AverageRating > 0)
            .OrderByDescending(m => m.AverageRating)
            .Take(9)
            .ToList();

        // Debug: Log top-rated movies
        Console.WriteLine("Top Rated Movies:");
        foreach (var movie in topRatedMovies)
        {
            Console.WriteLine($"Movie: {movie.Title}, Average Rating: {movie.AverageRating}");
        }

        // Filter top-rated TV shows
        var topRatedTvShows = allMovies
            .Where(m => m.Type == "TV Show" && m.AverageRating > 0)
            .OrderByDescending(m => m.AverageRating)
            .Take(9)
            .ToList();

        // Debug: Log top-rated TV shows
        Console.WriteLine("Top Rated TV Shows:");
        foreach (var show in topRatedTvShows)
        {
            Console.WriteLine($"TV Show: {show.Title}, Average Rating: {show.AverageRating}");
        }

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

    public async Task<IActionResult> Filter(string searchTitle = "", string type = "", int? year = null, string rating = "", int page = 1, int pageSize = 30)
    {
        var moviesResponse = await _supabaseClient.From<Movies>().Get();
        var movies = moviesResponse.Models;

        if (!string.IsNullOrEmpty(searchTitle))
        {
            movies = movies.Where(m => m.Title != null && m.Title.ToLower().Contains(searchTitle.ToLower())).ToList();
        }
        if (!string.IsNullOrEmpty(type))
        {
            movies = movies.Where(m => m.Type == type).ToList();
        }
        if (year.HasValue)
        {
            movies = movies.Where(m => m.ReleaseYear == year.Value).ToList();
        }
        if (!string.IsNullOrEmpty(rating))
        {
            movies = movies.Where(m => m.Rating == rating).ToList();
        }

        ViewBag.Types = movies.Select(m => m.Type).Distinct().ToList();
        ViewBag.Years = movies.Select(m => m.ReleaseYear).Distinct().OrderBy(y => y).ToList();
        ViewBag.Ratings = movies.Select(m => m.Rating).Distinct().OrderBy(r => r).ToList();

        var totalMovies = movies.Count;
        var totalPages = (int)Math.Ceiling((double)totalMovies / pageSize);
        ViewBag.TotalPages = totalPages;
        ViewBag.CurrentPage = page;

        var pagedMovies = movies
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(m => new MovieWithRatingsViewModel
            {
                Movie = m,
                Ratings = new List<Ratings>() // Can fetch ratings if needed
            })
            .ToList();

        return View(pagedMovies);
    }


}
