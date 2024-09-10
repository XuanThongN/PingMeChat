using AutoMapper;
using PingMeChat.CMS.Application.App.IRepositories;
using PingMeChat.CMS.Application.Feature.Service.BidaTables.Dto;
using PingMeChat.CMS.Application.Lib;
using PingMeChat.CMS.Application.Service.IRepositories;
using PingMeChat.CMS.Entities.Feature;
using MathNet.Numerics.Statistics.Mcmc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using NPOI.SS.Formula.Functions;
using System.Linq.Expressions;
using System.Linq;
using PingMeChat.CMS.Application.Common.Dto;
using PingMeChat.CMS.Application.Common.Pagination;
using PingMeChat.CMS.Application.Common.Exceptions;
using PingMeChat.Shared;

namespace PingMeChat.CMS.Application.Feature.Service
{
    public interface IBidaTableService : IServiceBase<BidaTable, BidaTableCreateDto, BidaTableUpdateDto, BidaTableDto, IBidaTableRepository>
    {
        Task<BidaTableDto> Find(Expression<Func<BidaTable, bool>> match, Func<IQueryable<BidaTable>, IIncludableQueryable<BidaTable, object>>? include = null);
        Task<BidaTableDto> ChangeStatus(string id, string email);
        Task<bool> StartPlay(string id, string email);
        Task<bool> StartPlayAfterChangeTable(string id, string orderParrentId, string email);
        Task<bool> StartPlayAfterChangeHour(string id, string orderParrentId, string email);
        Task<int> GetTotalProgress();
        Task<int> GetTotalEmpty();
        Task<Dictionary<string,int>> GetTotalWithStatus();
        Task<List<BidaTableDto>> GetEmptyTables();
        Task<bool> Delete(string id, string email);
        //Task<List<BidaTableDto>> FindAll(Expression<Func<BidaTable, bool>> predicate, Func<IQueryable<BidaTable>, IIncludableQueryable<BidaTable, object>> include = null);
        //Task<bool> StartSessionAsync(string tableId);
        //Task<bool> PauseSessionAsync(string tableId);
        //Task<bool> ResumeSessionAsync(string tableId);
        //Task<bool> EndSessionAsync(string tableId);
    }
    public class BidaTableService : ServiceBase<BidaTable, BidaTableCreateDto, BidaTableUpdateDto, BidaTableDto, IBidaTableRepository>, IBidaTableService
    {
        private readonly IBidaTableSessionRepository _bidaTableSessionRepository;
        public BidaTableService(IBidaTableRepository repository, IUnitOfWork unitOfWork, IMapper mapper, IBidaTableSessionRepository bidaTableSessionRepository, IUriService uriService) : base(repository, unitOfWork, mapper, uriService)
        {
            _bidaTableSessionRepository = bidaTableSessionRepository;
        }

        public override async Task<BidaTableDto> Add(BidaTableCreateDto dto)
        {
            var entity = await _repository.Find(x => x.Code == dto.Code);
            if (entity != null) throw new AppException("Mã bàn đã tồn tại trong hệ thống", null ,400);

            var bidaTable = _mapper.Map<BidaTable>(dto);
            //entity.BidaTableStatus = BidaTableStatus.Available;
            await _repository.Add(bidaTable);
            await _unitOfWork.SaveChangeAsync();
            return _mapper.Map<BidaTableDto>(bidaTable);
        }

        public async Task<BidaTableDto> ChangeStatus(string id, string email)
        {
            var entity = await _repository.FindById(id);
            if (entity == null) throw new ApplicationException("Table not found");
            entity.BidaTableStatus = BidaTableStatus.Available;
            entity.OrderParrentId = null;
            entity.UpdatedBy = email;

            await _repository.Update(entity);
            await _unitOfWork.SaveChangeAsync();

            return _mapper.Map<BidaTableDto>(entity);

        }

        public async Task<bool> StartPlay(string id, string email)
        {
            var executionStrategy = _unitOfWork.CreateExecutionStrategy();

            return await executionStrategy.ExecuteAsync(async () =>
            {
                using (var transaction = await _unitOfWork.BeginTransactionAsync())
                {
                    try
                    {
                        var bidaTable = await _repository.FindById(id);
                        if (bidaTable == null || bidaTable.BidaTableStatus != BidaTableStatus.Available)
                        {
                            return false;
                        }

                        bidaTable.BidaTableStatus = BidaTableStatus.Playing;
                        await _repository.Update(bidaTable);

                        // tạo 1 phiên chơi mới
                        BidaTableSession bidaTableSession = new BidaTableSession
                        {
                            StartTime = DateTime.Now.RoundToMinute(), // Chỉ tính đến phút
                            SessionStatus = SessionStatus.Playing,
                            BidaTableId = bidaTable.Id,
                            CreatedBy = email,
                            UpdatedBy = email
                        };
                        await _bidaTableSessionRepository.Add(bidaTableSession);
                        await _unitOfWork.SaveChangeAsync();
                        await transaction.CommitAsync();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        // Log exception
                        //      _logger.LogError(ex, "Error occurred while starting play for table {TableId}", tableId);
                        return false;
                    }
                }
            });
        }

        public async Task<bool> StartPlayAfterChangeTable(string id, string orderParrentId, string email)
        {
            //var executionStrategy = _unitOfWork.CreateExecutionStrategy();

            //return await executionStrategy.ExecuteAsync(async () =>
            //{
            //    using (var transaction = await _unitOfWork.BeginTransactionAsync())
            //    {
            //        try
            //        {
            //            var bidaTable = await _repository.FindById(id);
            //            if (bidaTable == null || bidaTable.BidaTableStatus != BidaTableStatus.Available)
            //            {
            //                return false;
            //            }
            //            // chuyển sang chế độ chuyển bàn - tách giừo
            //            bidaTable.BidaTableStatus = BidaTableStatus.Maintenance;
            //            bidaTable.OrderParrentId = orderParrentId;
            //            await _repository.Update(bidaTable);

            //            // tạo 1 phiên chơi mới
            //            BidaTableSession bidaTableSession = new BidaTableSession
            //            {
            //                StartTime = DateTime.Now,
            //                SessionStatus = SessionStatus.Playing,
            //                BidaTableId = bidaTable.Id,
            //                CreatedBy = email,
            //                UpdatedBy = email
            //            };
            //            await _bidaTableSessionRepository.Add(bidaTableSession);
            //            await _unitOfWork.SaveChangeAsync();
            //            await transaction.CommitAsync();
            //            return true;
            //        }
            //        catch (Exception ex)
            //        {
            //            await transaction.RollbackAsync();
            //            // Log exception
            //            //      _logger.LogError(ex, "Error occurred while starting play for table {TableId}", tableId);
            //            return false;
            //        }
            //    }
            //});

            var bidaTable = await _repository.FindById(id);
            if (bidaTable == null || bidaTable.BidaTableStatus != BidaTableStatus.Available)
            {
                return false;
            }
            // chuyển sang chế độ chuyển bàn
            bidaTable.BidaTableStatus = BidaTableStatus.ChangeTable;
            bidaTable.OrderParrentId = orderParrentId;
            await _repository.Update(bidaTable);
            await _unitOfWork.SaveChangeAsync();

            // tạo 1 phiên chơi mới
            BidaTableSession bidaTableSession = new BidaTableSession
            {
                StartTime = DateTime.Now.RoundToMinute(),
                SessionStatus = SessionStatus.Playing,
                BidaTableId = bidaTable.Id,
                CreatedBy = email,
                UpdatedBy = email
            };
            await _bidaTableSessionRepository.Add(bidaTableSession);
            await _unitOfWork.SaveChangeAsync();
            return true;
        }
        public async Task<bool> StartPlayAfterChangeHour(string id, string orderParrentId, string email)
        {
            var bidaTable = await _repository.FindById(id);
            if (bidaTable == null || bidaTable.BidaTableStatus != BidaTableStatus.Available)
            {
                return false;
            }
            // chuyển sang chế độ tách giờ
            bidaTable.BidaTableStatus = BidaTableStatus.SplitHour;
            bidaTable.OrderParrentId = orderParrentId;
            await _repository.Update(bidaTable);
            await _unitOfWork.SaveChangeAsync();

            // tạo 1 phiên chơi mới
            BidaTableSession bidaTableSession = new BidaTableSession
            {
                StartTime = DateTime.Now.RoundToMinute(),
                SessionStatus = SessionStatus.Playing,
                BidaTableId = bidaTable.Id,
                CreatedBy = email,
                UpdatedBy = email
            };
            await _bidaTableSessionRepository.Add(bidaTableSession);
            await _unitOfWork.SaveChangeAsync();
            return true;
        }

        public async Task<int> GetTotalProgress()
        {
            var list = await _repository.FindAll(x => x.BidaTableStatus == BidaTableStatus.Playing);
            return list.Count();
        }

        public async Task<int> GetTotalEmpty()
        {
            var list = await _repository.FindAll(x => x.BidaTableStatus == BidaTableStatus.Available);
            return list.Count();
        }

        public async Task<List<BidaTableDto>> GetEmptyTables()
        {
            var data = await _repository.FindAll(match: x => x.BidaTableStatus == BidaTableStatus.Available,
                include: incl => incl.Include(x => x.BidaTableType));
            var result = _mapper.Map<List<BidaTableDto>>(data);
            return result;
        }

        public async Task<BidaTableDto> Find(Expression<Func<BidaTable, bool>> match, Func<IQueryable<BidaTable>, IIncludableQueryable<BidaTable, object>>? include = null)
        {
            var data = await _repository.Find(match: match, include: include);
            if (data == null) throw new ApplicationException("Table not found");
            var result = _mapper.Map<BidaTableDto>(data);
            return result;

        }
        //public async Task<bool> EndSessionAsync(string tableId)
        //{
        //    var table = await _repository.FindById(tableId);
        //    if (table == null || table.BidaTableSessionId == null) throw new ApplicationException("Table not found");

        //    var session = await _bidaTableSessionRepository.Find(x => x.Id == table.BidaTableSessionId);
        //    if (session == null) throw new ApplicationException("Session not found");

        //    session.EndTime = DateTime.Now;
        //    session.SessionStatus = SessionStatus.Active;
        //    session.TotalTime = (int)(session.EndTime.Value - session.StartTime).TotalSeconds - session.TotalPausedTime;
        //    await _bidaTableSessionRepository.Update(session);

        //    table.BidaTableStatus = BidaTableStatus.Available;
        //    table.BidaTableSessionId = null;
        //    await _repository.Update(table);

        //    await _unitOfWork.SaveChangesAsync();
        //    return true;

        //}
        //public async Task<bool> PauseSessionAsync(string tableId)
        //{
        //    var table = await _repository.FindById(tableId);
        //    if (table == null || table.BidaTableSessionId == null) throw new ApplicationException("Table not found");

        //    var session = await _bidaTableSessionRepository.Find( x => x.Id == table.BidaTableSessionId);
        //    if (session == null) throw new ApplicationException("Session not found");

        //    if (session.SessionStatus != SessionStatus.Playing) throw new ApplicationException("Session is not playing");

        //    session.SessionStatus = SessionStatus.Paused;
        //    session.PausedTime = DateTime.Now;
        //    await _bidaTableSessionRepository.Update(session);

        //    table.BidaTableStatus = BidaTableStatus.Paused;
        //    await _repository.Update(table);

        //    await _unitOfWork.SaveChangesAsync();

        //    return true;

        //}

        //public async Task<bool> ResumeSessionAsync(string tableId)
        //{
        //    var table = await _repository.FindById(tableId);
        //    if (table == null || table.BidaTableSessionId == null) throw new ApplicationException("Table not found");

        //    var session = await _bidaTableSessionRepository.Find(x => x.Id == table.BidaTableSessionId);
        //    if (session == null) throw new ApplicationException("Session not found");

        //    session.SessionStatus = SessionStatus.Playing;
        //    session.TotalPausedTime += (int)(DateTime.Now - session.PausedTime.Value).TotalSeconds;
        //    session.PausedTime = null;
        //    await _bidaTableSessionRepository.Update(session);

        //    table.BidaTableStatus = BidaTableStatus.Playing;

        //    await _repository.Update(table);
        //    await _unitOfWork.SaveChangesAsync();

        //    return true;

        //}

        //public async Task<bool> StartSessionAsync(string tableId)
        //{
        //    var table = await _repository.FindById(tableId);
        //    if (table == null) throw new ApplicationException("Table not found");

        //    if (table.BidaTableStatus != BidaTableStatus.Available) throw new ApplicationException("Table is not available");

        //    var session = new BidaTableSession
        //    {
        //        BidaTableId = tableId,
        //        StartTime = DateTime.Now,
        //        SessionStatus = SessionStatus.Playing
        //    };

        //    await _bidaTableSessionRepository.Add(session);
        //    table.BidaTableStatus = BidaTableStatus.Playing;

        //    await _repository.Update(table);
        //    await _unitOfWork.CommitTransaction();

        //    return true;
        //}

        public async Task<bool> Delete(string id, string email)
        {
            var entity = await _repository.FindById(id) ?? throw new ApplicationException("Không thể tìm thấy bàn để xoá");
            if (entity.BidaTableStatus != BidaTableStatus.Available)
                throw new AppException("Không thể xoá bàn đang chơi");
            entity.UpdatedBy = email;
            entity.IsDeleted = true;
            entity.DeletedDate = DateTime.Now;
            await _repository.Update(entity);
            await _unitOfWork.SaveChangeAsync();
            return true;

        }

        public async Task<Dictionary<string, int>> GetTotalWithStatus()
        {
            var items = await _repository.GetAll();
            var result = items.GroupBy(x => x.BidaTableStatus)
                                .ToDictionary(x => x.Key.ToString(),
                                            x => x.Count());
            return result;
        }
    }
}