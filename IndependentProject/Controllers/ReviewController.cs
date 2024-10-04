using Microsoft.AspNetCore.Mvc;
using Supabase;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class ReviewController : Controller
{
    private readonly Client _supabaseClient;

    public ReviewController(Client supabaseClient)
    {
        _supabaseClient = supabaseClient;
    }

    // GET action to render the form
    [HttpGet]
    public IActionResult Create(string movieId)
    {
        if (string.IsNullOrEmpty(movieId))
        {
            ViewBag.ErrorMessage = "Movie ID is missing.";
            return View("Error");
        }

        // Hardcoded UserId and setting ShowId (movieId)
        /*var Ratings = new Ratings
        {
            ShowId = movieId,
            UserId = 1  // Hardcoded UserId for now

        };*/

        return View();  // Pass the initialized Ratings model to the view
    }

    // POST action to submit the review
    [HttpPost]
    public async Task<IActionResult> Create(string movieId, int rating, string recommend, string description)
    {
        try
        {
            // Create a new review
            var Ratings = new Ratings
            {
                //RatingID = 1,
                ShowId = movieId,
                UserId = 1,
                Rating = 3,
                Recommend = "hi",
                Description = "oii"
            };
            
            Console.WriteLine($"Inserting Review: RatingId={Ratings.RatingID},ShowId={Ratings.ShowId}, UserId={Ratings.UserId}, Rating={Ratings.Rating}, Recommend={Ratings.Recommend}, Description={Ratings.Description}");

            var response = await _supabaseClient.From<Ratings>().Insert(Ratings);

            if (response != null)
            {
                return RedirectToAction("Index", "Home"); 
            }
            else
            {
                ViewBag.ErrorMessage = "Error submitting review.";
                return View("Error");
            }
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = ex.Message;
            return View("Error");
        }
    }


}

