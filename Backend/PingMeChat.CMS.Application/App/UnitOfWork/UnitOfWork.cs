using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PingMeChat.CMS.Application.App.IRepositories;
using PingMeChat.CMS.Application.Service.IRepositories;

namespace PingMeChat.CMS.EntityFrameworkCore.EntityFrameworkCore
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDBContext _dbContext;

        public UnitOfWork(AppDBContext context )
        {
            _dbContext = context;
        }
        // Thêm phương thức này vào UnitOfWork
        public IExecutionStrategy CreateExecutionStrategy()
        {
            return _dbContext.Database.CreateExecutionStrategy();
        }
        public async Task CommitAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public async Task RollBackAsync()
        {
            await _dbContext.DisposeAsync();
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _dbContext.Database.BeginTransactionAsync();
        }

        public async Task SaveChangeAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
