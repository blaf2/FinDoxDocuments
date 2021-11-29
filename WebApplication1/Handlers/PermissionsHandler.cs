using FinDoxDocumentsAPI.Models;
using System;
using System.Security.Claims;

namespace FinDoxDocumentsAPI.Handlers
{
    public static class PermissionsHandler
    {
        public const string UserPermissionError = "User does not have permission for this";
        public const string UserDoesNotExistError = "User does not exist";

        public static bool HasPermission(ClaimsIdentity claimsIdentity, UserTypes permissionType)
        {
            if (!Enum.TryParse(claimsIdentity?.FindFirst(nameof(Models.User.UserType))?.Value, out UserTypes type))
                return false;

            if ((permissionType == UserTypes.Manager && type == UserTypes.Regular) || (permissionType == UserTypes.Admin && type != UserTypes.Admin))
                return false;

            return true;
        }

        public static User GetUser(ClaimsIdentity claimsIdentity)
        {
            if (!int.TryParse(claimsIdentity?.FindFirst(nameof(Models.User.UserId))?.Value, out int userId))
                return null;

            var userName = claimsIdentity?.FindFirst(nameof(Models.User.UserName))?.Value;
            if (string.IsNullOrEmpty(userName))
                return null;

            if (!Enum.TryParse(claimsIdentity?.FindFirst(nameof(Models.User.UserType))?.Value, out UserTypes userType))
                return null;

            return new User { UserId = userId, UserName = userName, UserType = userType};
        }
    }
}
