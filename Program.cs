var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Healthcare Portal API");
        options.RoutePrefix = "swagger";
    });

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
