using Microsoft.AspNetCore.Mvc;
using Supabase;
using System.Threading.Tasks;

public class HomeController : Controller
{
    private readonly Client _supabaseClient;

    public HomeController(Client supabaseClient)
    {
        _supabaseClient = supabaseClient;
    }

    // Test method to check Supabase connection
    public async Task<IActionResult> TestSupabase()
    {
        try
        {
            // Fetch a small amount of data from the Account table
            var response = await _supabaseClient.From<Account>().Get();

            // Check if we received any data
            if (response.Models != null && response.Models.Count > 0)
            {
                // Return data as JSON for simplicity
                return Json(new { success = true, data = response.Models });
            }
            else
            {
                // No data found, but connection works
                return Json(new { success = true, message = "Connected, but no data found." });
            }
        }
        catch (Exception ex)
        {
            // If something went wrong, return the error message
            return Json(new { success = false, message = ex.Message });
        }
    }
}
