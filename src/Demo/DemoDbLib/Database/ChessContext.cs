using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace DemoDbLib.Database
{
    public class ChessContext : DbContext
    {
        private string _sqlConnectionString;

        public DbSet<Player> Players { get; set; }
        public DbSet<RatingSource> RatingSources { get; set; }
        public DbSet<Rating> Ratings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseSqlServer(_sqlConnectionString);
        }

        public ChessContext(string sqlConnectionString)
        {
            _sqlConnectionString = sqlConnectionString;
        }
    }
}
