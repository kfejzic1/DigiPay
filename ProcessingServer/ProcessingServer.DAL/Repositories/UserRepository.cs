using Microsoft.EntityFrameworkCore;
using ProcessingServer.DAL.Entities;
using ProcessingServer.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingServer.DAL.Repositories
{
    public class UserRepository : IUserRepository
    {
        private AppDbContext _dbContext;

        public UserRepository(AppDbContext appDbContext)
        {
            _dbContext = appDbContext;
        }

        public async Task<User> CreateUser(User user)
        {
            await _dbContext.Users.AddAsync(user);
            return user;
        }

        public async Task<User> GetUserById(string id)
        {
            var result = await _dbContext.Users.FirstOrDefaultAsync(user => user.UserId.Equals(id));
            return result;
        }
    }
}
