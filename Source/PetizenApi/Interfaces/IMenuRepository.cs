using GraphQL.Language.AST;
using PetizenApi.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PetizenApi.Interfaces
{
    public interface IMenuRepository
    {
        //menu master
        Task<List<MenuMaster>> GetMenuMastersAsync(string menuId, string parentId, int menuType, IDictionary<string, Field> fields, CancellationToken cancellationToken);


        Task<MenuMaster> InsUpdMenuMasterAsync(MenuMaster menuMaster, CancellationToken cancellationToken);

        Task<bool> DeleteMenuMasterAsync(string menuId, CancellationToken cancellationToken);

        //Menu Access

        Task<List<MenuAccess>> GetMenuAccessAsync(string accessId, string menuId, string roleId, CancellationToken cancellationToken);

        Task<bool> InsUpdMenuAccessAsync(List<MenuAccess> menuAccess, CancellationToken cancellationToken);

        Task<bool> DeleteMenuAccessAsync(string accessId, CancellationToken cancellationToken);

        Task<List<GetRoleWiseMenu>> GetRoleWiseMenuAsync(string ParentId, CancellationToken cancellationToken);

        Task<List<MenuAction>> GetRoleWiseActionAsync(string ParentId, string RoleId, CancellationToken cancellationToken);

        Task<List<GetLoginWiseMenu>> GetLoginWiseMenuAsync(string UserId, CancellationToken cancellationToken);

        Task<List<SubMenuList>> GetSubMenusAsync(string ParentId, string RoleId, CancellationToken cancellationToken);

        Task<List<ChildMenuList>> GetChildMenusAsync(string ParentId, string RoleId, CancellationToken cancellationToken);

        Task<List<MenuMaster>> GetSubMenusTestAsync(string ParentId, string RoleId, IDictionary<string, Field> fields, CancellationToken cancellationToken);



    }
}
