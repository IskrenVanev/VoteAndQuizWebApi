using Microsoft.EntityFrameworkCore;
using VoteAndQuizWebApi.Data;
using VoteAndQuizWebApi.Repository.IRepository;
using VoteAndQuizWebApi.Repository;
using Microsoft.AspNetCore.Identity;
using VoteAndQuizWebApi.Models;
using VoteAndQuizWebApi.Utility;
using System.Security.Claims;
using VoteAndQuizWebApi.Services;
using VoteAndQuizWebApi.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
        .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

builder.Services.AddAuthorization();
builder.Services.AddScoped<UserManager<User>>();
builder.Services.AddIdentityApiEndpoints<User>()
    .AddUserManager<UserManager<User>>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Change to None for local development
    options.Cookie.SameSite = SameSiteMode.None; // Allow cross-origin
});

builder.Services.AddRazorPages();
builder.Services.AddScoped<IDbInitializer, DbInitializer>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IQuizRepository, QuizRepository>();
builder.Services.AddScoped<IVoteRepository, VoteRepository>();
builder.Services.AddScoped<IVoteOptionRepository, VoteOptionRepository>();
builder.Services.AddScoped<IQuizzesService, QuizzesService>();
builder.Services.AddScoped<IVotesService, VotesService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:7056") // URL of your React app
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapIdentityApi<User>();

app.UseCors("AllowReactApp");

app.MapPost("/logout", async (SignInManager<User> signInManager) =>
{
    await signInManager.SignOutAsync();
    return Results.Ok();
}).RequireAuthorization();

app.MapGet("/pingauth", (ClaimsPrincipal user) =>
{
    var email = user.FindFirstValue(ClaimTypes.Email); // get the user's email from the claim
    return Results.Json(new { Email = email }); ; // return the email as a plain text response
}).RequireAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.MapFallbackToFile("/index.html");

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
var dbContext = services.GetRequiredService<ApplicationDbContext>();
var dbInitializer = services.GetRequiredService<IDbInitializer>();
SeedDatabase();
app.MapControllers();

app.Run();

void SeedDatabase()
{
    using (var scope = app.Services.CreateScope())
    {
        var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
        dbInitializer.SeedData();
    }
}