using Microsoft.EntityFrameworkCore;
using TDD.Models;

namespace TDD.Data;

public class TddDbContext : DbContext
{
    public DbSet<Book> Books { get; set; }

    public TddDbContext(DbContextOptions<TddDbContext> options) : base(options)
    {
    }
}