using Imdb.Infrastructure.Entities;
using Infrastructure.Repositories.Entities;
using Microsoft.EntityFrameworkCore;

namespace Imdb.Infrastructure
{
    public class DataContext : DbContext
    {

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<User> User { get; set; }
        public DbSet<ListImdb> List { get; set; }
        public DbSet<Film> Films { get; set; }
        public DbSet<Film_Genre> Film_FilmCategory { get; set; }
        public DbSet<Genre> FilmCategory { get; set; }
        public DbSet<Film_List> Film_List { get; set; }
        public DbSet<Person> Person { get; set; }
        public DbSet<Person_Film> Person_Film { get; set; }
        public DbSet<Person_Type> Person_PersonCategory { get; set; }
        public DbSet<Entities.Type> PersonCategory { get; set; }
        public DbSet<Review> Review { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<Reaction> Reaction { get; set; }
        public DbSet<Comment> Comment { get; set; }

        public DbSet<RefreshToken> RefreshToken { get; set; }


    }
}
