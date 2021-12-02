using Microsoft.AspNetCore.Authorization;

namespace FinDoxDocumentsAPI
{
    public class RoleAuthorizeAttribute : AuthorizeAttribute
    {
        public RoleAuthorizeAttribute(params string[] roles)
        {
            Roles = string.Join(",", roles);
        }
    }
}
