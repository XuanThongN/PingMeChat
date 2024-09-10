using AutoMapper;
using PingMeChat.CMS.Application.App.IRepositories;
using PingMeChat.CMS.Application.Feature.Indentity.Auth.Dto;
using PingMeChat.CMS.Application.Service.IRepositories;
using PingMeChat.CMS.Entities;

namespace PingMeChat.CMS.Application.Feature.Indentity.Auth
{
    public interface ITokenService
    {
        Task<bool> RevokeRefreshTokenAsync(string refreshTokenId);
        Task<bool> RevokeAllRefreshTokensForUserAsync(string userId, string email);
        Task<RefreshTokenDto?> GetSavedRefreshToken(string userId);
        Task<bool> FindByTokenValue(string tokenValue);
        Task<RefreshTokenDto> CheckValidRefreshtoken(string tokenValue);
        //Task<TokenRefreshResult> RefreshTokenAsync(string accessToken, string refreshToken);
    }

    public class TokenService : ITokenService
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IUserSessionRepository _userSessionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TokenService(IRefreshTokenRepository refreshTokenRepository,
            IUserSessionRepository userSessionRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _userSessionRepository = userSessionRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<bool> RevokeRefreshTokenAsync(string refreshTokenId)
        {
            var refreshToken = await _refreshTokenRepository.FindById(refreshTokenId);
            if (refreshToken != null)
            {
                refreshToken.IsRevoked = true;
                await _refreshTokenRepository.Update(refreshToken);
                await _unitOfWork.SaveChangeAsync();
                return true;
            }
            return false;
        }
        public async Task<bool> RevokeAllRefreshTokensForUserAsync(string userId, string email)
        {
            var refreshTokens = await _refreshTokenRepository.FindAll(x => x.AccountId == userId);
            foreach (var token in refreshTokens)
            {
                token.IsRevoked = true;
                token.UpdatedBy = email;
            }
            await _refreshTokenRepository.UpdateRange(refreshTokens);

            // Optionally, you might want to end all sessions for this user
            await _userSessionRepository.LogoutAllSessionsForUserAsync(userId);

            return true;
        }

        public async Task<RefreshTokenDto?> GetSavedRefreshToken(string userId)
        {
            var refreshToken = await _refreshTokenRepository.Find(match: x => x.AccountId == userId);

            if (refreshToken == null)
            {
                return null;
            }
            var refreshTokenDto = _mapper.Map<RefreshTokenDto>(refreshToken);
            return refreshTokenDto;
        }
        
        public async Task<bool> FindByTokenValue(string tokenValue)
        {
            var refreshToken = await _refreshTokenRepository.Find(match: x => x.TokenValue == tokenValue);
            if(refreshToken == null) return false;
            return true;
        }

        public async Task<RefreshTokenDto> CheckValidRefreshtoken(string tokenValue)
        {
            var refreshToken = await _refreshTokenRepository.Find(match: x => x.TokenValue == tokenValue);
            if (refreshToken == null) return new RefreshTokenDto();

            // kiểm tra token còn hạn không
            if (refreshToken.ExpiryDate < DateTime.Now)  return new RefreshTokenDto();
            return _mapper.Map<RefreshTokenDto>(refreshToken);
        }
        //public async Task<TokenRefreshResult> RefreshTokenAsync(string accessToken, string refreshToken)
        //{
        //    var principal = GetPrincipalFromExpiredToken(accessToken);
        //    var username = principal.Identity.Name;

        //    var storedRefreshToken = await _refreshTokenRepository.GetByTokenValueAsync(refreshToken);

        //    if (storedRefreshToken == null || storedRefreshToken.ExpiryDate < DateTime.UtcNow || storedRefreshToken.IsRevoked)
        //    {
        //        throw new UnauthorizedException("Invalid refresh token");
        //    }

        //    var newAccessToken = GenerateAccessToken(storedRefreshToken.Account);
        //    var newRefreshToken = GenerateRefreshToken();

        //    storedRefreshToken.IsRevoked = true;
        //    await _refreshTokenRepository.UpdateAsync(storedRefreshToken);

        //    var newRefreshTokenEntity = new RefreshToken
        //    {
        //        TokenValue = newRefreshToken,
        //        DeviceInfo = storedRefreshToken.DeviceInfo,
        //        IPAddress = storedRefreshToken.IPAddress,
        //        ExpiryDate = DateTime.UtcNow.AddDays(7),
        //        AccountId = storedRefreshToken.AccountId
        //    };

        //    await _refreshTokenRepository.AddAsync(newRefreshTokenEntity);

        //    var userSession = await _userSessionRepository.GetByRefreshTokenIdAsync(storedRefreshToken.Id);
        //    userSession.RefreshTokenId = newRefreshTokenEntity.Id;
        //    userSession.AccessToken = newAccessToken;
        //    userSession.LastActivityTime = DateTime.UtcNow;

        //    await _userSessionRepository.UpdateAsync(userSession);

        //    return new TokenRefreshResult
        //    {
        //        AccessToken = newAccessToken,
        //        RefreshToken = newRefreshToken
        //    };
        //}
    }

}
