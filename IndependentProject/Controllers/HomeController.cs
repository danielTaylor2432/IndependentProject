using Microsoft.AspNetCore.Mvc;
using Supabase;
using System.Threading.Tasks;
using System.Collections.Generic;

public class HomeController : Controller
{
    private readonly Client _supabaseClient;

    public HomeController(Client supabaseClient)
    {
        _supabaseClient = supabaseClient;
    }

    // Method to display all data from the Account table
    public async Task<IActionResult> Index()
    {
        try
        {
            // Fetch all data from the Account table
            var response = await _supabaseClient.From<Account>().Get();

            // Return the data to the view
            return View(response.Models);
        }
        catch (Exception ex)
        {
            // If something went wrong, return an error view or message
            ViewBag.ErrorMessage = ex.Message;
            return View("Error");
        }
    }

    // Method to display the form to add a new Account
    public IActionResult Create()
    {
        return View();
    }

    // Method to handle the submission of the new Account
    [HttpPost]
    public async Task<IActionResult> Create(Account account)
    {
        if (ModelState.IsValid)
        {
            try
            {
                // Insert the new account into the database
                var response = await _supabaseClient.From<Account>().Insert(account);

                // After inserting, redirect to the Index action to show all data
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // If something went wrong, return an error view or message
                ViewBag.ErrorMessage = ex.Message;
                return View("Error");
            }
        }
        return View(account);
    }
}
