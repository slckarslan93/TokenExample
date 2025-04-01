var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient<TokenService>();
builder.Services.AddSingleton<OrderService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

var orderService = app.Services.GetRequiredService<OrderService>();
var timer = new Timer(async _ => await orderService.FetchOrdersAsync(), null, TimeSpan.Zero, TimeSpan.FromMinutes(5));

app.Run();
