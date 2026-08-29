using CleanAuthDemo.Application;
using CleanAuthDemo.Infrastructure;
using CleanAuthDemo.WebApi.Extensions;
using Scalar.AspNetCore;
using CleanAuthDemo.WebApi.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddPermissionAuthorization();
builder.Services.AddCurrentUser();
builder.Services.AddControllers();
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<
        BearerSecuritySchemeTransformer>();
});

var app = builder.Build();
await app.Services.SeedIdentityAsync();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();