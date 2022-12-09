using api.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace api.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;

        public UnitOfWork(  IMapper mapper , DataContext context)
        {
            _mapper = mapper;
            _context = context;
        }
        public IUserRepository UserRepository => new UserRepository(_context , _mapper);

        public IMessageRepository MessageRepository => new MessageRepository(_context, _mapper);

        public ILikesRepository likesRepository => new LikesRepository(_context , _mapper);

        public async Task<bool> Complete()
        {
          return  await _context.SaveChangesAsync() > 0;
        }

        public bool HasChanges()
        {
            return _context.ChangeTracker.HasChanges();
        }
    }
}
