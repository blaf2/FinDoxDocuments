using FinDoxDocumentsAPI.Models;
using FinDoxDocumentsAPI.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinDoxDocumentsAPI.Services
{
    public class GroupService : IGroupService
    {
        private readonly IGroupRepository _groupRepository;

        public GroupService(IGroupRepository groupRepository)
        {
            _groupRepository = groupRepository;
        }

        public async Task<UserGroup> CreateGroupAsync(CreateUserGroupRequest request)
        {
            return await _groupRepository.CreateGroupAsync(request);
        }

        public async Task DeleteGroupAsync(int id)
        {
            await _groupRepository.DeleteGroupAsync(id);
        }

        public async Task<UserGroup> GetGroupAsync(int id)
        {
            return await _groupRepository.GetGroupAsync(id);
        }

        public async Task<IEnumerable<UserGroup>> GetGroupsAsync()
        {
            return await _groupRepository.GetGroupsAsync();
        }

        public async Task<UserGroup> UpdateGroupAsync(UpdateUserGroupRequest request)
        {
            return await _groupRepository.UpdateGroupAsync(request);
        }
    }
}
