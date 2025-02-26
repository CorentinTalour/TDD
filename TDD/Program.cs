using Microsoft.EntityFrameworkCore;
using TDD.Repository;
using TDD.services;

var builder = WebApplication.CreateBuilder(args);

// Ajouter le DbContext et le repository
builder.Services.AddDbContext<DbContext>(options =>
    options.UseSqlServer(
        builder.Configuration
            .GetConnectionString("DefaultConnection"))); // Assure-toi d'avoir la bonne configuration de base de données

builder.Services.AddScoped<IBookRepository, BookRepository>(); // Injection du BookRepository
builder.Services.AddScoped<BookService>(); // Injection du BookService

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