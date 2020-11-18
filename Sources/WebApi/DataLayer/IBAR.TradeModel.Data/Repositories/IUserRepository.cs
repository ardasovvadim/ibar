using IBAR.TradeModel.Data.Entities;
using System.Data.Entity;
using System.Linq;

namespace IBAR.TradeModel.Data.Repositories
{
    public interface IUserRepository
    {
        User GetByEmail(string email);
        void Save(User user);
        User GetById(long id);
        bool IsExists(User user);
        IQueryable<User> Query();
        User Add(User dto);
        void Delete(long id);
        User Update(User dto);
        User GetEntry(User user);
    }

    public class UserRepository : IUserRepository
    {
        private readonly TradeModelContext _dbContext;

        public UserRepository(TradeModelContext dbContext)
        {
            _dbContext = dbContext;
        }

        public User GetByEmail(string email)
        {
            return _dbContext
                    .Users
                    .Include(u => u.Roles)
                    .FirstOrDefault(u => u.Email == email && !u.Deleted);
        }

        public void Save(User user)
        {
            _dbContext.Entry(user).State = EntityState.Modified;
            _dbContext.SaveChanges();
        }

        public User GetById(long id)
        {
            return _dbContext.Users.FirstOrDefault(u => u.Id == id && !u.Deleted);
        }

        public bool IsExists(User user)
        {
            return _dbContext
                    .Users
                    .Any(u => (u.Id == user.Id || u.Email == user.Email || u.Phone == user.Phone)
                              && !u.Deleted);
        }

        public IQueryable<User> Query()
        {
            return _dbContext.Users.Where(u => !u.Deleted);
        }

        public User Add(User dto)
        {
            var user = _dbContext.Users.Add(dto);
            _dbContext.SaveChanges();
            return user;
        }

        public void Delete(long id)
        {
            var user = GetById(id);
            user.Deleted = true;
            _dbContext.SaveChanges();
        }

        public User Update(User dto)
        {
            var user = GetById(dto.Id);
            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.Email = dto.Email;
            user.Phone = dto.Phone;
            _dbContext.SaveChanges();
            return user;
        }

        public User GetEntry(User user)
        {
            return _dbContext
                    .Users
                    .FirstOrDefault(u => !u.Deleted && 
                                         (u.Id == user.Id || u.Email == user.Email || u.Phone == user.Phone));
        }
    }
}