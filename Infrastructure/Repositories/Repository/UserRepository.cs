using Imdb.Infrastructure.Entities;
using Imdb.Infrastructure.IRepository;
using Infrastructure.Repositories.Entities;
using Infrastructure.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Imdb.Infrastructure.Repository
{
    public record RegistrationDTO([Required] string FirstName, [Required] string LastName,
    [Required] string UserName, [Required] string Password, [Required] string Age, [Required] string Email);

    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(DataContext dataContext) : base(dataContext)
        {
        }

        public async Task<User?> GetByUserNameAsync(string userName)
        {
            return await _context.User.Where(user => user.UserName == userName).SingleOrDefaultAsync();
        }
        public IQueryable<User> QueryRole(int usertier)
        {
            return base.Query().Where(x => x.Role.Tier < usertier).AsQueryable();
        }
    }
}
