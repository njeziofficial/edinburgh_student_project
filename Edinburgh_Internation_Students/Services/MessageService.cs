using Edinburgh_Internation_Students.Data;
using Edinburgh_Internation_Students.DTOs.Common;
using Edinburgh_Internation_Students.DTOs.Messages;
using Edinburgh_Internation_Students.DTOs.Users;
using Edinburgh_Internation_Students.Models;
using Microsoft.EntityFrameworkCore;

namespace Edinburgh_Internation_Students.Services;

public class MessageService(ApplicationDbContext context) : IMessageService
{
    public async Task<ApiResponse<List<MessageDto>>> GetGroupMessagesAsync(Guid groupId, int userId, int limit = 50, int offset = 0)
    {
        try
        {
            // Verify group exists
            var groupExists = await context.Groups.AnyAsync(g => g.Id == groupId);
            if (!groupExists)
            {
                return ApiResponse<List<MessageDto>>.ErrorResponse(
                    "Group not found",
                    null,
                    404
                );
            }

            // Verify user is a member of the group
            var isMember = await context.GroupMembers
                .AnyAsync(gm => gm.GroupId == groupId && gm.UserId == userId);

            if (!isMember)
            {
                return ApiResponse<List<MessageDto>>.ErrorResponse(
                    "You must be a member of this group to view messages",
                    null,
                    403
                );
            }

            // Get messages with pagination
            var messages = await context.Messages
                .Where(m => m.GroupId == groupId)
                .Include(m => m.Sender)
                .Include(m => m.Reactions)
                    .ThenInclude(r => r.User)
                .OrderByDescending(m => m.CreatedAt)
                .Skip(offset)
                .Take(limit)
                .Select(m => new MessageDto
                {
                    Id = m.Id.ToString(),
                    Content = m.Content,
                    SenderId = m.SenderId.ToString(),
                    RecipientId = null,
                    GroupId = m.GroupId.ToString(),
                    IsRead = false,
                    ReadAt = null,
                    IsEdited = m.IsEdited,
                    EditedAt = null,
                    IsDeleted = false,
                    Sender = new UserSummaryDto
                    {
                        Id = m.Sender.Id.ToString(),
                        Name = $"{m.Sender.FirstName} {m.Sender.LastName}".Trim(),
                        AvatarUrl = null,
                        Country = null,
                        IsOnline = false
                    },
                    CreatedAt = m.CreatedAt
                })
                .ToListAsync();

            return ApiResponse<List<MessageDto>>.SuccessResponse(messages);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<MessageDto>>.ErrorResponse(
                "Failed to retrieve messages",
                new List<string> { ex.Message },
                500
            );
        }
    }

    public async Task<ApiResponse<MessageDto>> SendMessageAsync(Guid groupId, int userId, SendMessageRequest request)
    {
        try
        {
            // Verify group exists
            var groupExists = await context.Groups.AnyAsync(g => g.Id == groupId && g.IsActive);
            if (!groupExists)
            {
                return ApiResponse<MessageDto>.ErrorResponse(
                    "Group not found or inactive",
                    null,
                    404
                );
            }

            // Verify user is a member of the group
            var isMember = await context.GroupMembers
                .AnyAsync(gm => gm.GroupId == groupId && gm.UserId == userId);

            if (!isMember)
            {
                return ApiResponse<MessageDto>.ErrorResponse(
                    "You must be a member of this group to send messages",
                    null,
                    403
                );
            }

            // Create message
            var message = new Message
            {
                GroupId = groupId,
                SenderId = userId,
                Content = request.Content,
                IsEdited = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await context.Messages.AddAsync(message);
            await context.SaveChangesAsync();

            // Load sender info
            var sender = await context.Users.FindAsync(userId);

            var messageDto = new MessageDto
            {
                Id = message.Id.ToString(),
                Content = message.Content,
                SenderId = message.SenderId.ToString(),
                RecipientId = null,
                GroupId = message.GroupId.ToString(),
                IsRead = false,
                ReadAt = null,
                IsEdited = message.IsEdited,
                EditedAt = null,
                IsDeleted = false,
                Sender = new UserSummaryDto
                {
                    Id = sender!.Id.ToString(),
                    Name = $"{sender.FirstName} {sender.LastName}".Trim(),
                    AvatarUrl = null,
                    Country = null,
                    IsOnline = false
                },
                CreatedAt = message.CreatedAt
            };

            return ApiResponse<MessageDto>.SuccessResponse(
                messageDto,
                "Message sent successfully",
                201
            );
        }
        catch (Exception ex)
        {
            return ApiResponse<MessageDto>.ErrorResponse(
                "Failed to send message",
                new List<string> { ex.Message },
                500
            );
        }
    }

    public async Task<ApiResponse<bool>> AddReactionAsync(Guid groupId, Guid messageId, int userId, string emoji)
    {
        try
        {
            // Verify message exists and belongs to the group
            var message = await context.Messages
                .FirstOrDefaultAsync(m => m.Id == messageId && m.GroupId == groupId);

            if (message == null)
            {
                return ApiResponse<bool>.ErrorResponse(
                    "Message not found",
                    null,
                    404
                );
            }

            // Verify user is a member of the group
            var isMember = await context.GroupMembers
                .AnyAsync(gm => gm.GroupId == groupId && gm.UserId == userId);

            if (!isMember)
            {
                return ApiResponse<bool>.ErrorResponse(
                    "You must be a member of this group to add reactions",
                    null,
                    403
                );
            }

            // Check if reaction already exists
            var existingReaction = await context.MessageReactions
                .FirstOrDefaultAsync(r => r.MessageId == messageId && r.UserId == userId && r.Emoji == emoji);

            if (existingReaction != null)
            {
                return ApiResponse<bool>.SuccessResponse(
                    true,
                    "Reaction already exists"
                );
            }

            // Add reaction
            var reaction = new MessageReaction
            {
                MessageId = messageId,
                UserId = userId,
                Emoji = emoji,
                CreatedAt = DateTime.UtcNow
            };

            await context.MessageReactions.AddAsync(reaction);
            await context.SaveChangesAsync();

            return ApiResponse<bool>.SuccessResponse(
                true,
                "Reaction added successfully"
            );
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.ErrorResponse(
                "Failed to add reaction",
                new List<string> { ex.Message },
                500
            );
        }
    }

    public async Task<ApiResponse<bool>> RemoveReactionAsync(Guid groupId, Guid messageId, int userId, string emoji)
    {
        try
        {
            // Verify message exists and belongs to the group
            var messageExists = await context.Messages
                .AnyAsync(m => m.Id == messageId && m.GroupId == groupId);

            if (!messageExists)
            {
                return ApiResponse<bool>.ErrorResponse(
                    "Message not found",
                    null,
                    404
                );
            }

            // Find and remove reaction
            var reaction = await context.MessageReactions
                .FirstOrDefaultAsync(r => r.MessageId == messageId && r.UserId == userId && r.Emoji == emoji);

            if (reaction == null)
            {
                return ApiResponse<bool>.ErrorResponse(
                    "Reaction not found",
                    null,
                    404
                );
            }

            context.MessageReactions.Remove(reaction);
            await context.SaveChangesAsync();

            return ApiResponse<bool>.SuccessResponse(
                true,
                "Reaction removed successfully"
            );
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.ErrorResponse(
                "Failed to remove reaction",
                new List<string> { ex.Message },
                500
            );
        }
    }

    public async Task<ApiResponse<bool>> DeleteMessageAsync(Guid groupId, Guid messageId, int userId)
    {
        try
        {
            // Verify message exists and belongs to the group
            var message = await context.Messages
                .FirstOrDefaultAsync(m => m.Id == messageId && m.GroupId == groupId);

            if (message == null)
            {
                return ApiResponse<bool>.ErrorResponse(
                    "Message not found",
                    null,
                    404
                );
            }

            // Verify user is the sender
            if (message.SenderId != userId)
            {
                return ApiResponse<bool>.ErrorResponse(
                    "You can only delete your own messages",
                    null,
                    403
                );
            }

            context.Messages.Remove(message);
            await context.SaveChangesAsync();

            return ApiResponse<bool>.SuccessResponse(
                true,
                "Message deleted successfully"
            );
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.ErrorResponse(
                "Failed to delete message",
                new List<string> { ex.Message },
                500
            );
        }
    }
}
