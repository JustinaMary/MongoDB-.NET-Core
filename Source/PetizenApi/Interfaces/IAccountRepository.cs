using GraphQL.Language.AST;
using PetizenApi.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PetizenApi.Interfaces
{
    public interface IAccountRepository
    {

        Task<UserMaster> InsertUpdateUserMasterAsync(UserMaster userMaster, CancellationToken cancellationToken);
        Task<List<UserMaster>> GetUserMasterAsync(int Id, string UserId, string Email, int UserType, CancellationToken cancellationToken);

        Task<List<UserDetails>> GetUserDetailsAsync(string UserId, IDictionary<string, Field> fields, CancellationToken cancellationToken);
        Task<List<MyUserRole>> GetUserDetailsRolWiseAsync(string UserId, IDictionary<string, Field> fields, CancellationToken cancellationToken);

        Task<bool> IsEmailExistAsync(string Email, CancellationToken cancellationToken);
        Task<List<RoleMaster>> GetUserRolesAsync(int RoleType, CancellationToken cancellationToken);
        Task<bool> DeleteUserAsync(int Id, CancellationToken cancellationToken);

        Task<LoginResponse> GetLoginResponseAsync(string emailId, string ipAddress, UserMaster userModel, CancellationToken cancellationToken);

        bool UpdateUserRoles(string UserId, string DeletedRoles, string AddedRoles);

        Task<List<PasswordHistory>> GetPasswordHistoryAsync(string Email);
        Task<bool> InsertPasswordHistoryAsync(PasswordHistory passwordHistory);

    }
}
