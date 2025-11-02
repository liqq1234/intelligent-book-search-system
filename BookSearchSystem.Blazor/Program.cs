using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BookSearchSystem.Blazor;
using BookSearchSystem.Blazor.Services;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configure HttpClient with API base address and timeout
builder.Services.AddScoped(sp => new HttpClient 
{ 
    BaseAddress = new Uri("http://localhost:5001"),
    Timeout = TimeSpan.FromMinutes(5) // 增加超时时间到5分钟
});

// Add MudBlazor services
builder.Services.AddMudServices();

// Register application services
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IBookService, BookService>();

await builder.Build().RunAsync();
