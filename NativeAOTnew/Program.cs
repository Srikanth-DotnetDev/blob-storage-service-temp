using Microsoft.Extensions.Options;

var builder = WebApplication.CreateSlimBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure JSON logging to the console.
builder.Logging.AddJsonConsole();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", () =>
{
    var numbers = from n in Enumerable.Range(100, 50)
                  select (Number: n, OddEven: n % 2 == 0 ? "Even" : "Odd");


    foreach (var n in numbers)
    {
        Console.WriteLine("The number {0} is {1}.", n.Number, n.OddEven);
    }
    return Results.Ok();
}).WithName("dsfdsf").WithSummary("dsfiudsf");



app.Run();