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
            var response = await _supabaseClient.From<Movies>().Get();

            // Filter results based on search criteria
            var movies = response.Models?.AsQueryable() ?? Enumerable.Empty<Movies>().AsQueryable();

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

            // Get unique years and ratings for the dropdowns
            var years = response.Models?.Select(m => m.ReleaseYear).Distinct().OrderBy(y => y).ToList();
            var ratings = response.Models?.Select(m => m.Rating).Distinct().OrderBy(r => r).ToList();
            var types = response.Models?.Select(m => m.Type).Distinct().OrderBy(t => t).ToList();

            ViewBag.Years = years;
            ViewBag.Ratings = ratings;
            ViewBag.Types = types;

            return View(movies.ToList()); // Pass the filtered list of movies to the view
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = ex.Message;
            return View("Error");
        }
    }
}
