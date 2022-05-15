using ChatApp.Api.Filters;
using ChatApp.Api.Mappers;
using ChatApp.Api.Registers;
using ChatApp.DataAccess.Context;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// todo: JWT Config

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", corsBuilder =>
        corsBuilder.SetIsOriginAllowed(_ => true)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

builder.Services.AddDbContext<ChatAppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("ChatAppConnection:Development"));
});

builder.Services.AddControllers();
builder.Services.AddServices();

builder.Services.AddSignalR(options =>
{
    options.AddFilter<HubAuthFilter>();
});

builder.Services.AddFilters();
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddHubs();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("CorsPolicy");

app.UseAuthorization();

app.MapControllers();
app.MapSignalRHubs();

app.Run();
