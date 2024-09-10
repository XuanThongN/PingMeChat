using PingMeChat.CMS.Application.App.IRepositories;
using PingMeChat.CMS.Entities;
using PingMeChat.CMS.EntityFrameworkCore.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.App.Repositories
{
    public class UserSessionRepository : Repository<UserSession>, IUserSessionRepository
    {
        private readonly AppDBContext _context;
        public UserSessionRepository(AppDBContext context) : base(context)
        {
            _context = context;
        }

        public async Task LogoutAllSessionsForUserAsync(string userId)
        {
            var currentTime = DateTime.UtcNow;

            var activeSessions = await _context.UserSessions
                .Where(s => s.AccountId == userId && s.LogoutTime == null)
                .ToListAsync();

            foreach (var session in activeSessions)
            {
                session.LogoutTime = currentTime;
                session.LastActivityTime = currentTime;
            }

            await _context.SaveChangesAsync();

            // Optionally, you might want to revoke all refresh tokens for this user as well
            var refreshTokens = await _context.RefreshTokens
                .Where(rt => rt.AccountId == userId && !rt.IsRevoked)
                .ToListAsync();

            foreach (var token in refreshTokens)
            {
                token.IsRevoked = true;
            }

            await _context.SaveChangesAsync();
        }
    }
}
