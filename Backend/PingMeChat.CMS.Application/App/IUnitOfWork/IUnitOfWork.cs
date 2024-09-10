using Microsoft.EntityFrameworkCore;
using PingMeChat.CMS.Application.App.IRepositories;
using PingMeChat.CMS.Application.App.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;

namespace PingMeChat.CMS.Application.Service.IRepositories
{
    public interface IUnitOfWork
    {
        IExecutionStrategy CreateExecutionStrategy();
        Task CommitAsync();
        Task RollBackAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task SaveChangeAsync();
    }
}
