using Edinburgh_Internation_Students.DTOs.Common;
using Edinburgh_Internation_Students.DTOs.Groups;

namespace Edinburgh_Internation_Students.Services;

public interface IGroupService
{
    Task<ApiResponse<GroupDto>> CreateGroupAsync(string name, string? description = null);
    Task<ApiResponse<GroupDto>> GetGroupByIdAsync(Guid groupId);
    Task<ApiResponse<List<GroupDto>>> GetAllGroupsAsync();
    Task<ApiResponse<GroupDto>> UpdateGroupAsync(Guid groupId, UpdateGroupRequest request);
    Task<ApiResponse<bool>> DeleteGroupAsync(Guid groupId);
    Task<ApiResponse<GroupMemberDto>> AddMemberToGroupAsync(Guid groupId, int userId);
    Task<ApiResponse<bool>> RemoveMemberFromGroupAsync(Guid groupId, int userId);
    Task<ApiResponse<List<GroupMemberDto>>> GetGroupMembersAsync(Guid groupId);
    Task<ApiResponse<UserGroupsResponse>> GetUserGroupsAsync(int userId);
    Task<Guid> FindOrCreateAvailableGroupAsync();
    Task<int> GetGroupMemberCountAsync(Guid groupId);
    Task<List<GroupDto>> GetActiveGroupsWithMembersAsync();
    Task ShuffleGroupMembersAsync();
}
