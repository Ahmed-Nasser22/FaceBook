using api.DTOs;
using api.Entities;
using api.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace api.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _Mapper;

        public UserRepository(DataContext context, IMapper Mapper)
        {
            _context = context;
            _Mapper = Mapper;
        }

        public async Task<MemberDto> GetMemberAsync(string UserName)
        {
            return await _context.Users
                .Where(s => s.UserName == UserName)
                .ProjectTo<MemberDto>(_Mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();
        }



        public async Task<IEnumerable<MemberDto>> GetMembersAsync()
        {
            return await _context.Users
                  .ProjectTo<MemberDto>(_Mapper.ConfigurationProvider).ToListAsync();
        }

        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<AppUser> GetUserByUsernameAsync(string UserName)
        {
            return await _context.Users.Include(x => x.Photos)
                .SingleOrDefaultAsync(s => s.UserName == UserName);
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await _context.Users.Include(p => p.Photos).ToListAsync();
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public void Update(AppUser User)
        {
            _context.Entry(User).State = EntityState.Modified;
        }
    }
}
