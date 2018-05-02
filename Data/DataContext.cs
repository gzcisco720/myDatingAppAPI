using Microsoft.EntityFrameworkCore;
using myDotnetApp.API.Model;

namespace myDotnetApp.API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options):base(options){}
        
        public DbSet<Values> Values { get; set; }
        public DbSet<User> Users { get; set; }
    }
}