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

        // Initialize the Ratings model with the movie ID and other test values for debugging
        var model = new Ratings
        {
            ShowId = movieId,
            //UserId = "TestUser", // Set a default value for testing
            //Rating = 5,          // Set a default rating for testing
            //Recommend = "Yes",   // Set a recommendation value for testing
            //Description = "Sample description for debugging." // Add a description for testing
            //RatingID = 7
        };

        Console.WriteLine($"GET - ShowId: {model.ShowId}, UserId: {model.UserId}, Rating: {model.Rating}, Recommend: {model.Recommend}, Description: {model.Description}, RatingID: {model.RatingID}");

        return View(model);  // Pass the model to the view
    }



    // POST action to submit the review
    [HttpPost]
    public async Task<IActionResult> Create(
    int ratingID,
    string showId,
    string userId,
    int rating,
    string recommend,
    string description)
    {
        Console.WriteLine($"ShowId: {showId}, RatingID: {ratingID}, UserId: {userId}, Rating: {rating}, Recommend: {recommend}, Description: {description}");

        if (string.IsNullOrEmpty(showId) || string.IsNullOrEmpty(userId))
        {
            ViewBag.ErrorMessage = "Movie ID or User ID is missing.";
            return View("Error");
        }

        try
        {
            // Create a new Ratings instance with the parameters
            var ratings = new Ratings
            {
                ShowId = showId,
                UserId = userId,
                Rating = rating,
                Recommend = recommend,
                Description = description
            };

            // Insert the new review
            var response = await _supabaseClient.From<Ratings>().Insert(ratings);

            if (response != null && response.Models.Any())
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
            ViewBag.ErrorMessage = $"An error occurred: {ex.Message}";
            return View("Error");
        }
    }





}

