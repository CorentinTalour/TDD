using Microsoft.EntityFrameworkCore;
using TDD.Repositories;
using TDD.Repositories.Implementations;
using TDD.Repositories.Interfaces;
using TDD.services;

var builder = WebApplication.CreateBuilder(args);

// Ajouter le DbContext et le repository
builder.Services.AddDbContext<DbContext>(options =>
    options.UseSqlServer(
        builder.Configuration
            .GetConnectionString("DefaultConnection"))); // Assure-toi d'avoir la bonne configuration de base de données

builder.Services.AddHttpClient<IBookWebService, BookWebServiceClient>();

builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<BookService>();
builder.Services.AddScoped<IBookWebService, BookWebServiceClient>();
builder.Services.AddScoped<BookWebService>();

// Add services to the container.
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

app.Run();