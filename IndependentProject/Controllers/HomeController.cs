using Microsoft.AspNetCore.Mvc;
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

    // Method to display all data from the NetflixShow table with filtering
    /*public async Task<IActionResult> Index(string searchTitle = "", int? releaseYear = null, string rating = "", string type = "", int pageNumber = 1, int pageSize = 20)
    {
        try
        {
            // Step 1: Retrieve the movies based on search criteria
            var response = await _supabaseClient.From<Movies>().Get();
            var movies = response.Models?.AsQueryable() ?? Enumerable.Empty<Movies>().AsQueryable();

            // Apply filters if necessary
            if (!string.IsNullOrEmpty(searchTitle))
            {
                movies = movies.Where(m => m.Title.Contains(searchTitle, StringComparison.OrdinalIgnoreCase));
            }

            if (releaseYear.HasValue)
            {
                movies = movies.Where(m => m.ReleaseYear == releaseYear.Value);
            }

            if (!string.IsNullOrEmpty(rating))
            {
                movies = movies.Where(m => m.Rating.Equals(rating, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(type))
            {
                movies = movies.Where(m => m.Type.Equals(type, StringComparison.OrdinalIgnoreCase));
            }

            // Get the total count of filtered movies for pagination
            int totalMovies = movies.Count();

            // Apply pagination
            movies = movies.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            // Convert movies to a list so we can work with it
            var movieList = movies.ToList();

            // Step 2: Retrieve all ratings, then filter them in-memory based on ShowId
            var allRatingsResponse = await _supabaseClient.From<Ratings>().Get();
            var ratingsList = allRatingsResponse.Models;

            // Get the list of show IDs from the movie list
            var showIds = movieList.Select(m => m.ShowId).Distinct().ToList();

            // Step 3: Group ratings by ShowId for easier lookup
            var ratingsByShowId = ratingsList
                .Where(r => showIds.Contains(r.ShowId)) // Filter in-memory
                .GroupBy(r => r.ShowId)
                .ToDictionary(g => g.Key, g => g.ToList());

            // Step 4: Create a list of ViewModel objects
            var movieWithRatingsList = movieList.Select(movie => new MovieWithRatingsViewModel
            {
                Movie = movie,
                Ratings = ratingsByShowId.ContainsKey(movie.ShowId) ? ratingsByShowId[movie.ShowId] : new List<Ratings>()
            }).ToList();

            // Calculate total pages for pagination
            int totalPages = (int)Math.Ceiling(totalMovies / (double)pageSize);

            // Pass data to the view
            ViewBag.Years = response.Models?.Select(m => m.ReleaseYear).Distinct().OrderBy(y => y).ToList();
            ViewBag.Ratings = response.Models?.Select(m => m.Rating).Distinct().OrderBy(r => r).ToList();
            ViewBag.Types = response.Models?.Select(m => m.Type).Distinct().OrderBy(t => t).ToList();
            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = totalPages;

            return View(movieWithRatingsList);  // Pass the ViewModel list to the view
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = ex.Message;
            return View("Error");
        }
    }*/

    public async Task<IActionResult> Index()
    {
        // Fetch latest movies
        var moviesResponse = await _supabaseClient.From<Movies>()
            .Filter("type", Operator.Equals, "Movie")
            .Order("release_year", Ordering.Descending)
            .Limit(10) // Adjust to show the number of latest movies you want
            .Get();

        var latestMovies = moviesResponse.Models;

        // Fetch latest TV shows
        var tvShowsResponse = await _supabaseClient.From<Movies>()
            .Filter("type", Operator.Equals, "TV Show")
            .Order("release_year", Ordering.Descending)
            .Limit(10) // Adjust to show the number of latest TV shows you want
            .Get();

        var latestTvShows = tvShowsResponse.Models;

        // Pass the lists to the view
        var viewModel = new LatestContentViewModel
        {
            LatestMovies = latestMovies,
            LatestTvShows = latestTvShows
        };

        return View(viewModel);
    }



    public async Task<IActionResult> Details(string id)
    {
        // Retrieve the movie based on the ShowId
        var movieResponse = await _supabaseClient
            .From<Movies>()
            .Filter("show_id", Operator.Equals, id)
            .Get();

        var movie = movieResponse.Models.FirstOrDefault();

        if (movie == null)
        {
            return NotFound("Movie not found.");
        }

        // Retrieve reviews for this movie
        var reviewResponse = await _supabaseClient
            .From<Ratings>()
            .Filter("Show_Id", Operator.Equals, id)
            .Get();

        var reviews = reviewResponse.Models;

        // Pass the movie and reviews data to the view
        var viewModel = new MovieWithRatingsViewModel
        {
            Movie = movie,
            Ratings = reviews
        };

        return View(viewModel);
    }



}
