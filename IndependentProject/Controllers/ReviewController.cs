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

        // Initialize the Ratings model with the movie ID
        var model = new Ratings
        {
            ShowId = movieId
        };

        return View(model);  // Pass the model to the view
    }


    // POST action to submit the review
    [HttpPost]
    public async Task<IActionResult> Create(Ratings ratings)
    {
        try
        {
            Console.WriteLine($"ShowId: {ratings.ShowId}, UserId: {ratings.UserId}, Rating: {ratings.Rating}, Recommend: {ratings.Recommend}, Description: {ratings.Description}");
            // Ensure ShowId and UserId are valid
            if (string.IsNullOrEmpty(ratings.ShowId) || string.IsNullOrEmpty(ratings.UserId))
            {
                ViewBag.ErrorMessage = "Movie ID or User ID is missing.";
                return View("Error");
            }

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

