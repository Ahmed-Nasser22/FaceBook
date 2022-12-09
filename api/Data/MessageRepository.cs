using api.DTOs;
using api.Entities;
using api.Helpers;
using api.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace api.Data
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext _conext;
        private readonly IMapper _mapper;

        public MessageRepository(DataContext conext, IMapper mapper)
        {
            _conext = conext;
            _mapper = mapper;
        }

        public void AddGroup(Group group)
        {
            _conext.Groups.Add(group);
        }

        public void AddMessage(Message message)
        {
            _conext.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            _conext.Messages.Remove(message);
        }

        public async Task<Connection> GetConnection(string connectionId)
        {
           return await _conext.Connections.FindAsync(connectionId);
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _conext.Messages.FindAsync(id);
        }

        public async Task<Group> GetMessageGroup(string groupName)
        {
            return await _conext.Groups
                .Include(c => c.Connections)
                .FirstOrDefaultAsync(g => g.Name == groupName);
        }

        public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
        {
            var query = _conext.Messages
                .OrderByDescending(m => m.MessageSent)
                .AsQueryable();

            query = messageParams.Container switch
            {
                "Inbox" => query.Where(u => u.RecipientUsername == messageParams.Username && u.RecipientDeleted == false),
                "Outbox" => query.Where(u => u.SenderUsername == messageParams.Username && u.SenderDeleted == false),
                _ => query.Where(u => u.RecipientUsername == messageParams.Username && u.DateRead == null && u.RecipientDeleted == false)
            };

            var message = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);

            return await PagedList<MessageDto>
                .CreateAsync(message, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUserName, string recipientUserName)
        {
            var query =  _conext.Messages
       
                .Where(
                        m => m.RecipientUsername == currentUserName && m.RecipientDeleted == false &&
                        m.SenderUsername == recipientUserName ||
                        m.RecipientUsername == recipientUserName && m.SenderDeleted == false &&
                        m.SenderUsername == currentUserName
                      ).OrderBy(m=>m.MessageSent).AsQueryable();

            var unreadMessages = query
                .Where(m => m.DateRead == null && currentUserName == m.RecipientUsername)
                .ToList();
            if (unreadMessages.Any())
            {
                foreach (var message in unreadMessages)
                {
                    message.DateRead = DateTime.UtcNow;
                }
            }

            return await query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider).ToListAsync();
        }

        public void RemoveConnection(Connection connection)
        {
            _conext.Connections.Remove(connection);
        }

    
    }
}
