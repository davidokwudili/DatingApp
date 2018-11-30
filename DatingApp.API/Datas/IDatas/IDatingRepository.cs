using System.Collections.Generic;
using System.Threading.Tasks;
using DatingApp.Api.helpers;
using DatingApp.API.helpers;
using DatingApp.Models;

namespace DatingApp.API.Datas.IDatas
{
    public interface IDatingRepository
    {
        void Add<T>(T entity) where T : class;
        void Delete<T>(T entity) where T : class;
        Task<User> GetUser(int id, bool isCurrentUser);
        Task<PagedList<User>> GetUsers(ParamsUser param);
        Task<bool> SaveAll();
        Task<Photo> GetPhoto(int id);
        Task<Like> GetLike(int userId, int recipientId);
        Task<Message> GetMessage(int id);
        Task<PagedList<Message>> GetMessagesForUser(ParamsMessage paramsMessage);
        Task<IEnumerable<Message>> GetMessageThread(int userId, int recipientId);
    }
}