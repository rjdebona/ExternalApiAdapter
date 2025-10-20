var builder = WebApplication.CreateBuilder(args);

 
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

 
var baseUrl = builder.Configuration["ExternalApi:BaseUrl"] ?? "https://api.example.com";
var apiKey = builder.Configuration["ExternalApi:ApiKey"] ?? "YOUR_API_KEY_HERE";

builder.Services.AddHttpClient<DebtCollectionPortal.Infrastructure.ApiRepositories.AccountRepositoryApi>(client =>
{
    client.BaseAddress = new Uri(baseUrl);
    client.DefaultRequestHeaders.Add("X-API-Key", apiKey);
    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
});

builder.Services.AddHttpClient<DebtCollectionPortal.Infrastructure.ApiRepositories.UserRepositoryApi>(client =>
{
    client.BaseAddress = new Uri(baseUrl);
    client.DefaultRequestHeaders.Add("X-API-Key", apiKey);
    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
});

builder.Services.AddHttpClient<DebtCollectionPortal.Infrastructure.ApiRepositories.ClientRepositoryApi>(client =>
{
    client.BaseAddress = new Uri(baseUrl);
    client.DefaultRequestHeaders.Add("X-API-Key", apiKey);
    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
});

 
builder.Services.AddScoped<DebtCollectionPortal.Application.IUserService, DebtCollectionPortal.Application.UserService>();
builder.Services.AddScoped<DebtCollectionPortal.Application.IAccountService, DebtCollectionPortal.Application.AccountService>();
builder.Services.AddScoped<DebtCollectionPortal.Application.IClientService, DebtCollectionPortal.Application.ClientService>();

 
builder.Services.AddScoped<DebtCollectionPortal.Domain.IAccountRepository>(sp => sp.GetRequiredService<DebtCollectionPortal.Infrastructure.ApiRepositories.AccountRepositoryApi>());
builder.Services.AddScoped<DebtCollectionPortal.Domain.IUserRepository>(sp => sp.GetRequiredService<DebtCollectionPortal.Infrastructure.ApiRepositories.UserRepositoryApi>());
builder.Services.AddScoped<DebtCollectionPortal.Domain.IClientRepository>(sp => sp.GetRequiredService<DebtCollectionPortal.Infrastructure.ApiRepositories.ClientRepositoryApi>());

 
builder.Services.AddHttpClient();

 
builder.Services.AddControllers();

 
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

var app = builder.Build();

 
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

 
app.UseCors();
app.UseSession(); // For simple login state if needed
app.UseStaticFiles(); // Serve wwwroot files (HTML, CSS, JS)

 
app.MapControllers();

 
app.MapFallbackToFile("index.html");

app.Run();
