using Supabase;
using Newtonsoft.Json;
using YourProjectNamespace.Services; // Ensure this matches the namespace where RiotApiService is located

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Access the Supabase configuration from appsettings.json
var supabaseUrl = builder.Configuration["Supabase:Url"];
var supabaseKey = builder.Configuration["Supabase:Key"];

// Initialize Supabase client
var supabaseClient = new Client(supabaseUrl, supabaseKey);

// Register the Supabase client as a singleton service
builder.Services.AddSingleton(supabaseClient);

// Access the Riot API key from appsettings.json
var riotApiKey = builder.Configuration["RiotApi:ApiKey"];

// Register RiotApiService with Dependency Injection, passing the configuration
builder.Services.AddSingleton<RiotApiService>(serviceProvider =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    return new RiotApiService(configuration);
});

builder.Services.AddControllers().AddNewtonsoftJson();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
