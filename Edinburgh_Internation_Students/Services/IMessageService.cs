using Edinburgh_Internation_Students.DTOs.Common;
using Edinburgh_Internation_Students.DTOs.Messages;

namespace Edinburgh_Internation_Students.Services;

public interface IMessageService
{
    Task<ApiResponse<List<MessageDto>>> GetGroupMessagesAsync(Guid groupId, int userId, int limit = 50, int offset = 0);
    Task<ApiResponse<MessageDto>> SendMessageAsync(Guid groupId, int userId, SendMessageRequest request);
    Task<ApiResponse<bool>> AddReactionAsync(Guid groupId, Guid messageId, int userId, string emoji);
    Task<ApiResponse<bool>> RemoveReactionAsync(Guid groupId, Guid messageId, int userId, string emoji);
    Task<ApiResponse<bool>> DeleteMessageAsync(Guid groupId, Guid messageId, int userId);
}
