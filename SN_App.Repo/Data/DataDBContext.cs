using SN_App.Repo.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace SN_App.Repo.Data
{
    public class DataDBContext : DbContext
    {
        public DataDBContext(DbContextOptions<DataDBContext> options) : base(options) {}

        public DbSet<Value> Values { get; set; }
    }

    public class DataDBContextFactory : IDesignTimeDbContextFactory<DataDBContext>
    {
        public DataDBContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DataDBContext>();
            optionsBuilder.UseSqlServer("Server=.\\SQLEXPRESS;Database=NenoDbb;Trusted_Connection=True;");

            return new DataDBContext(optionsBuilder.Options);
        }
    }
}