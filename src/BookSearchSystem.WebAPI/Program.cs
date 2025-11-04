using Microsoft.SemanticKernel;
using BookSearchSystem.WebAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure CORS - 允许前端访问
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazor", policy =>
    {
        policy.WithOrigins("http://localhost:5000", "https://localhost:5001")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Register services
builder.Services.AddScoped<IDatabaseService, DatabaseService>();
builder.Services.AddScoped<IAdvancedDatabaseService, AdvancedDatabaseService>();
builder.Services.AddScoped<IKernelService, KernelService>();

// Configure Semantic Kernel
var ollamaConfig = builder.Configuration.GetSection("Ollama");
var ollamaEndpoint = ollamaConfig["Endpoint"] ?? "http://localhost:11434";
var kernel = Kernel.CreateBuilder()
    .AddOpenAIChatCompletion(
        modelId: ollamaConfig["ModelName"] ?? "qwen2.5:7b",
        endpoint: new Uri($"{ollamaEndpoint}/v1"),  // Ollama的OpenAI兼容端点
        apiKey: null)
    .Build();

builder.Services.AddSingleton(kernel);

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowBlazor");

app.UseAuthorization();

app.MapControllers();

app.Run();
