using FinDoxDocumentsAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinDoxDocumentsAPI.Repositories
{
    public interface IGroupRepository
    {
        Task<IEnumerable<UserGroup>> GetGroupsAsync();
        Task<UserGroup> GetGroupAsync(int id);
        Task<UserGroup> CreateGroupAsync(CreateUserGroupRequest request);
        Task<UserGroup> UpdateGroupAsync(UpdateUserGroupRequest request);
        Task DeleteGroupAsync(int id);
    }
}
