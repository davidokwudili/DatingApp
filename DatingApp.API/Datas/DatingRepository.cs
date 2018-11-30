using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.Api.helpers;
using DatingApp.API.Datas.IDatas;
using DatingApp.API.helpers;
using DatingApp.Datas;
using DatingApp.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Datas
{
    public class DatingRepository : IDatingRepository
    {

        private readonly DataContext _context;


        public DatingRepository(DataContext context)
        {
            _context = context;
        }


        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public async Task<Like> GetLike(int userId, int recipientId)
        {
            return await _context.Likes.FirstOrDefaultAsync(u =>
               u.LikerId == userId && u.LikeeId == recipientId);
        }

        public async Task<Photo> GetPhoto(int id)
        {
            return await _context.Photos.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<User> GetUser(int id, bool isCurrentUser)
        {
            var query = _context.Users.Include(p => p.Photos).AsQueryable();

            if (isCurrentUser)
                query = query.IgnoreQueryFilters();

            var user = await query.FirstOrDefaultAsync(u => u.Id == id);

            return user;
        }

        public async Task<PagedList<User>> GetUsers(ParamsUser param)
        {
            //get the users, & include their photos
            var users = _context.Users.Include(p => p.Photos)
                                //orderby the last active date, desc, as as to show from most recent
                                .OrderByDescending(u => u.LastActive).AsQueryable();

            //get all users without adding the current logged in user among the lis
            users = users.Where(u => u.Id != param.UserId);

            //also get based on opposite gender
            users = users.Where(u => u.Gender == param.Gender);


            if (param.Likers)
            {
                var userLikers = await GetUserLikes(param.UserId, param.Likers);
                users = users.Where(u => userLikers.Contains(u.Id));
            }
            if (param.Likees)
            {
                var userLikees = await GetUserLikes(param.UserId, param.Likers);
                users = users.Where(u => userLikees.Contains(u.Id));
            }


            //check if MinAge Or MaxAge Params where passed
            if (param.MinAge != 18 || param.MaxAge != 99)
            {
                //get the Min Date Of Birth by adding the years of max to today exp (-99 - 1)
                var minDob = DateTime.Today.AddYears(-param.MaxAge - 1);
                //get the Max Date Of Birth by adding the years of min to today exp (-18 - 1)
                var maxDob = DateTime.Today.AddYears(-param.MinAge);

                //get users where DOB greater than minDob && lesser or equall MaxDob
                users = users.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);
            }

            //check if an orderBy was specified
            if (!string.IsNullOrEmpty(param.OrderBy))
            {
                //switch the orderBy parameter specified
                switch (param.OrderBy)
                {
                    case "created":
                        users = users.OrderByDescending(u => u.Created);
                        break;
                    default:
                        users = users.OrderByDescending(u => u.LastActive);
                        break;
                }
            }


            //get the queried page list
            return await PagedList<User>.QueryAsync(users, param.PageNumber, param.PageSize);
        }


        private async Task<IEnumerable<int>> GetUserLikes(int id, bool likers)
        {
            var user = await _context.Users
                .Include(x => x.Likers)
                .Include(x => x.Likees)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (likers)
            {
                return user.Likers.Where(u => u.LikeeId == id).Select(i => i.LikerId);
            }
            else
            {
                return user.Likees.Where(u => u.LikerId == id).Select(i => i.LikeeId);
            }
        }



        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }



        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<PagedList<Message>> GetMessagesForUser(ParamsMessage paramsMessage)
        {
            //get the messages
            var messages = _context.Messages
                //include the sender object, and then include the photo of the sender
                .Include(u => u.Sender).ThenInclude(p => p.Photos)
                //include the Recipient object, and then include the photo of the Recipient
                .Include(u => u.Recipient).ThenInclude(p => p.Photos)
                //make it quaryable, so as not to cause error
                .AsQueryable();

            //check the passed in parameter
            switch (paramsMessage.MessageContainer)
            {
                case "Inbox":
                    //get messages based on inboxe, based on RecipientID who sent me messages, & where it's not deleted
                    messages = messages.Where(u => u.RecipientId == paramsMessage.UserId
                        && u.RecipientDeleted == false);
                    break;
                case "Outbox":
                    //get messages based on outbox, based on who i sent messages, & where it's not deleted
                    messages = messages.Where(u => u.SenderId == paramsMessage.UserId
                        && u.SenderDeleted == false);
                    break;
                default:
                    //get the default, which is "UnRead"
                    messages = messages.Where(u => u.RecipientId == paramsMessage.UserId
                        && u.RecipientDeleted == false && u.IsRead == false);
                    break;
            }

            //order by decending, so as to show from the latest message
            messages = messages.OrderByDescending(d => d.MessageSent);

            //query and get the number of messages based on the page size and it number
            return await PagedList<Message>.QueryAsync(messages, paramsMessage.PageNumber, paramsMessage.PageSize);
        }

        public async Task<IEnumerable<Message>> GetMessageThread(int userId, int recipientId)
        {
            //get messages
            var messages = await _context.Messages
                    //get the messages from the sender with user photos
                    .Include(u => u.Sender).ThenInclude(p => p.Photos)
                    //get the messages from the recipient with user photos
                    .Include(u => u.Recipient).ThenInclude(p => p.Photos)
                    //get messages i sent and messages a user has sent to me
                    .Where(m =>
                        m.RecipientId == userId && m.RecipientDeleted == false && m.SenderId == recipientId
                        ||
                        m.RecipientId == recipientId && m.SenderId == userId && m.SenderDeleted == false)
                    //order by the date sent desc to show it latest
                    .OrderByDescending(m => m.MessageSent)
                    .ToListAsync();

            return messages;
        }
    }
}