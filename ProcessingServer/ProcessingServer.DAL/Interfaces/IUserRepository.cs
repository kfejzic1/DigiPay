using ProcessingServer.DAL.Entities;
using ProcessingServer.DAL.Other;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingServer.DAL.Interfaces
{
    public interface IUserRepository
    {
        Task<User> CreateUser(User user);
        Task<User> GetUserById(string id);
    }
}
