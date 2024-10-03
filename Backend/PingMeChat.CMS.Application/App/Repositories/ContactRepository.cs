using Microsoft.EntityFrameworkCore;
using PingMeChat.CMS.Application.App.IRepositories;
using PingMeChat.CMS.Entities;
using PingMeChat.CMS.Entities.Feature;
using PingMeChat.CMS.EntityFrameworkCore.EntityFrameworkCore;
using PingMeChat.Shared.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.App.Repositories
{
    public class ContactRepository : Repository<Contact>, IContactRepository
    {
        private readonly AppDBContext _dbContext;
        public ContactRepository(AppDBContext context) : base(context)
        {
            _dbContext = context;
        }
        // override lạii phương thức add
        public override async Task<Contact> Add(Contact entity)
        {
            // Nếu chưa tồn tại liên hệ, thêm mới
            await base.Add(entity);
            await _dbContext.Entry(entity).Reference(c => c.User).LoadAsync();
            await _dbContext.Entry(entity).Reference(c => c.ContactUser).LoadAsync();

            return entity;
        }
        public async Task<IEnumerable<Contact>> GetContactsByUserIdAsync(string userId)
        {
            return await _dbContext.Set<Contact>()
                .Include(c => c.User) // Bao gồm thông tin về người dùng
                .Include(c => c.ContactUser) // Bao gồm thông tin về liên hệ của người dùng
                .Where(c => (c.UserId == userId || c.ContactUserId == userId) // Lấy các liên hệ (đã gửi, đã chấp nhận, đang chờ)
                                && (c.Status == ContactStatus.Accepted
                                || c.Status == ContactStatus.Pending
                                || c.Status == ContactStatus.Requested)
                ) // Tìm tất cả liên hệ của userId
                .ToListAsync();
        }




    }
}
