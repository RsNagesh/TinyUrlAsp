using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

using UrlProject.Models;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<ShortUrl> Urls { get; set; }
}