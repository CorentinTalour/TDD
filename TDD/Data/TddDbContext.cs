using Microsoft.EntityFrameworkCore;
using TDD.objects;

namespace TDD;

public class TddDbContext : DbContext
{
    public DbSet<Book> Books { get; set; }

    public TddDbContext(DbContextOptions<TddDbContext> options) : base(options)
    {
    }
}