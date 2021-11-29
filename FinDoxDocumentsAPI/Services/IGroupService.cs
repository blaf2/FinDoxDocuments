using FinDoxDocumentsAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinDoxDocumentsAPI.Services
{
    public interface IGroupService
    {
        Task<IEnumerable<UserGroup>> GetGroupsAsync();
        Task<UserGroup> GetGroupAsync(int id);
        Task<UserGroup> CreateGroupAsync(CreateUserGroupRequest request);
        Task<UserGroup> UpdateGroupAsync(int id, UpdateUserGroupRequest request);
        Task DeleteGroupAsync(int id);
    }
}
