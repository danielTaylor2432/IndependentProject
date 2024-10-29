using Microsoft.AspNetCore.Mvc;
using Supabase;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class HomeController : Controller
{
    private readonly Client _supabaseClient;

    public HomeController(Client supabaseClient)
    {
        _supabaseClient = supabaseClient;
    }

    // Method to display all data from the NetflixShow table with filtering
    public async Task<IActionResult> Index(string searchTitle = "", int? releaseYear = null, string rating = "", string type = "")
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

            // Pass the ViewModel list to the view
            ViewBag.Years = response.Models?.Select(m => m.ReleaseYear).Distinct().OrderBy(y => y).ToList();
            ViewBag.Ratings = response.Models?.Select(m => m.Rating).Distinct().OrderBy(r => r).ToList();
            ViewBag.Types = response.Models?.Select(m => m.Type).Distinct().OrderBy(t => t).ToList();

            return View(movieWithRatingsList);  // Pass the ViewModel list to the view
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = ex.Message;
            return View("Error");
        }
    }



}
