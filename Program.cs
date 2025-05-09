using Microsoft.AspNetCore.Authentication;
using FinanceTest.Authentication;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(); // ⬅️ Add this for controllers

builder.Services.AddAuthentication("Admin")
    .AddScheme<AuthenticationSchemeOptions, SimpleTokenHandler>("Admin", null);

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();