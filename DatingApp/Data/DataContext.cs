using DatingApp.Models;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        //Add reference of Db Context to Startup/ConfigureServices
        public DbSet<Value> Values { get; set; }

    }
}
