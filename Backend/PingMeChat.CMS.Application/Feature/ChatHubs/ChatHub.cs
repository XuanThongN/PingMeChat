using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using PingMeChat.CMS.Application.Common.Attributes;
using PingMeChat.CMS.Application.Common.Exceptions;
using PingMeChat.CMS.Application.Feature.Service.Chats;
using PingMeChat.CMS.Application.Feature.Service.Chats.Dto;
using PingMeChat.CMS.Application.Feature.Service.Messages;
using PingMeChat.CMS.Application.Feature.Service.Messages.Dto;
using PingMeChat.CMS.Entities.Feature;
using PingMeChat.Shared.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace PingMeChat.CMS.Application.Feature.ChatHubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IChatHubService _chatHubService;
        private readonly IChatService _chatService;
        private readonly IMessageService _messageService;
        private readonly IUserConnectionManager _userConnectionManager; // Service quản lý connect ws của user
        public ChatHub(IChatHubService chatHubService, IChatService chatService, IMessageService messageService, IUserConnectionManager userConnectionManager)
        {
            _chatHubService = chatHubService;
            _chatService = chatService;
            _messageService = messageService;
            _userConnectionManager = userConnectionManager;
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var userId = httpContext?.User?.FindFirstValue("UserId");

            if (!string.IsNullOrEmpty(userId))
            {
                _userConnectionManager.AddConnection(userId, Context.ConnectionId); // Thêm ConnectionId vào Nhóm UserId để quản lý toàn bộ connection thuộc user đó

                // Thêm người dùng vào nhóm cá nhân để hỗ trợ tin nhắn 1-1
                await Groups.AddToGroupAsync(Context.ConnectionId, userId);

                // Lấy danh sách tất cả các cuộc trò chuyện mà người dùng tham gia (bao gồm cả chat private và chat nhóm)
                var userChats = await _chatService.GetChatListAsync(userId);

                // Thêm người dùng vào tất cả các nhóm chat
                foreach (var chat in userChats)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, chat.Id);
                }

                // Thông báo cho các người dùng khác về việc người dùng này online
                await Clients.All.SendAsync("ReceiveMessage", $"{userId} has joined");
            }

            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var httpContext = Context.GetHttpContext();
            var userId = httpContext?.User?.FindFirstValue("UserId");

            if (!string.IsNullOrEmpty(userId))
            {
                _userConnectionManager.RemoveConnection(userId, Context.ConnectionId);
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task StartNewChatAsync(ChatCreateDto chatCreateDto)
        {
            // Tạo ValidationContext từ DTO
            var context = new ValidationContext(chatCreateDto, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();

            // Thực hiện kiểm tra validation
            bool isValid = Validator.TryValidateObject(chatCreateDto, context, validationResults, validateAllProperties: true);

            if (!isValid)
            {
                // Ném lỗi nếu không hợp lệ, trả về thông báo lỗi cho người dùng
                var errors = string.Join(", ", validationResults.Select(vr => vr.ErrorMessage));
                throw new HubException($"Validation failed: {errors}");
            }

            var currentUserId = Context.User.FindFirstValue("UserId");
            // Tạo cuộc trò chuyện mới trong db
            var newChat = await _chatService.CreateChatAsync(chatCreateDto, currentUserId);
            // Thêm các người dùng vào nhóm SignalR với chatId mới
            foreach (var user in newChat.UserChats)
            {
                _userConnectionManager.GetConnections(user.UserId)?.ToList().ForEach(async connectionId =>
                {
                    await Groups.AddToGroupAsync(connectionId, newChat.Id);
                });
            }

            // Kiểm tra chỉ tạo mới cuộc trò chuyện nhóm thì mới thông báo tới những người thành viên
            if (chatCreateDto.IsGroup)
            {
                await Clients.Groups(newChat.Id).SendAsync("NewGroupChat", newChat);
            }
            else
            {
                await Clients.Caller.SendAsync("NewPrivateChat", newChat);
            }
        }

        public async Task SendMessage(MessageCreateDto messageCreateDto)
        {
            var userId = Context.User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                throw new AppException("User not authenticated");
            }
            if (!await HasChatAccess(messageCreateDto.ChatId))
            {
                throw new AppException("Access denied");
            }
            // Gửi realtime tới những người tham gia đoạn chat
            //await _chatHubService.SendMessageAsync(chatId, userId, message, DateTime.UtcNow);
            // Gán người gửi vào dto
            messageCreateDto.SenderId = userId;
            // Lưu tin nhắn vào database
            await _messageService.SendMessageAsync(messageCreateDto);
        }

        public async Task JoinChat(string chatId)
        {
            var userId = Context.User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                throw new AppException("User not authenticated");
            }
            await _chatHubService.JoinGroupAsync(Context.ConnectionId, chatId);
        }

        // Hiển thị người gõ tin nhắn
        public async Task UserIsTyping(string chatId)
        {
            var userId = Context.User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId) || !await HasChatAccess(chatId))
            {
                throw new HubException("Access denied");
            }

            await Clients.Group(chatId).SendAsync("UserTyping", userId);
        }

        //Tắt hiển thị người gõ tin nhắn
        public async Task UserStoppedTyping(string chatId)
        {
            var userId = Context.User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId) || !await HasChatAccess(chatId))
            {
                throw new HubException("Access denied");
            }

            await Clients.Group(chatId).SendAsync("UserStoppedTyping", userId);
        }

        private async Task<bool> HasChatAccess(string chatId)
        {
            var userId = Context.User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return false;
            }
            return await _chatService.CanUserAccessChat(chatId, userId);
        }


        //Call audio/Video
        // Hàm này được gọi khi một người dùng muốn bắt đầu một cuộc gọi
        // chatId: ID của cuộc trò chuyện
        // isVideo: true nếu là cuộc gọi video, false nếu là cuộc gọi âm thanh
        public async Task InitiateCall(string chatId, bool isVideo)
        {
            var callerUserId = Context.User.FindFirstValue("UserId");
            // Gửi thông báo "IncomingCall" đến tất cả người dùng trong nhóm chat
            await Clients.Group(chatId).SendAsync("IncomingCall", callerUserId, chatId, isVideo);
        }

        // Hàm này được gọi khi người nhận cuộc gọi trả lời
        // chatId: ID của cuộc trò chuyện
        // accept: true nếu người nhận chấp nhận cuộc gọi, false nếu từ chối
        public async Task AnswerCall(string chatId, bool accept)
        {
            var answeringUserId = Context.User.FindFirstValue("UserId");
            // Gửi thông báo "CallAnswered" đến tất cả người dùng trong nhóm chat
            await Clients.Group(chatId).SendAsync("CallAnswered", answeringUserId, chatId, accept);
        }

        // Hàm này được gọi khi có một ICE candidate mới được tạo ra trong quá trình thiết lập kết nối
        // chatId: ID của cuộc trò chuyện
        // candidate: Thông tin về ICE candidate
        public async Task IceCandidate(string chatId, string candidate)
        {
            var userId = Context.User.FindFirstValue("UserId");
            // Gửi thông báo "IceCandidate" đến tất cả người dùng khác trong nhóm chat
            await Clients.OthersInGroup(chatId).SendAsync("IceCandidate", userId, candidate);
        }

        // Hàm này được gọi khi người gọi tạo ra một offer SDP
        // chatId: ID của cuộc trò chuyện
        // sdp: Session Description Protocol, chứa thông tin về cấu hình media
        public async Task Offer(string chatId, string sdp)
        {
            var userId = Context.User.FindFirstValue("UserId");
            // Gửi thông báo "Offer" đến tất cả người dùng khác trong nhóm chat
            await Clients.OthersInGroup(chatId).SendAsync("Offer", userId, sdp);
        }

        // Hàm này được gọi khi người nhận tạo ra một answer SDP
        // chatId: ID của cuộc trò chuyện
        // sdp: Session Description Protocol, chứa thông tin về cấu hình media
        public async Task Answer(string chatId, string sdp)
        {
            var userId = Context.User.FindFirstValue("UserId");
            // Gửi thông báo "Answer" đến tất cả người dùng khác trong nhóm chat
            await Clients.OthersInGroup(chatId).SendAsync("Answer", userId, sdp);
        }

        // Hàm này được gọi khi một người dùng kết thúc cuộc gọi
        // chatId: ID của cuộc trò chuyện
        public async Task EndCall(string chatId)
        {
            var userId = Context.User.FindFirstValue("UserId");
            // Gửi thông báo "CallEnded" đến tất cả người dùng trong nhóm chat
            await Clients.Group(chatId).SendAsync("CallEnded", userId);
        }
    }

}

