using GraphQL.Language.AST;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using PetizenApi.Database;
using PetizenApi.Interfaces;
using PetizenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PetizenApi.Repositories
{
    public class MenuRepository : IMenuRepository
    {
        private readonly MongoConnection context = null;
        private readonly IMongoCollection<MenuAccess> _menuAccess;
        private readonly IMongoCollection<MenuMaster> _menuMaster;
        private readonly ICommonRepository _commonRepository;

        private readonly IAccountRepository _accountRepository;
        public MenuRepository(IOptions<MongoSettings> settings, IAccountRepository accountRepository,
            ICommonRepository commonRepository)
        {
            context = new MongoConnection(settings);
            var client = new MongoClient(settings.Value.ConnectionString);
            var database = client.GetDatabase(settings.Value.Database);
            _accountRepository = accountRepository;
            _menuAccess = database.GetCollection<MenuAccess>("MenuAccess");
            _menuMaster = database.GetCollection<MenuMaster>("MenuAccess");
            _commonRepository = commonRepository;
        }

        //menu master
        public async Task<List<MenuMaster>> GetMenuMastersAsync(string menuId, string parentId, int menuType, IDictionary<string, Field> fields, CancellationToken cancellationToken)
        {
            try
            {
                var builder = Builders<MenuMaster>.Filter;
                var filterDefine = FilterDefinition<MenuMaster>.Empty;

                if (!string.IsNullOrEmpty(menuId))
                {
                    filterDefine = builder.Eq(d => d.MenuId, menuId);
                }
                if (!string.IsNullOrEmpty(parentId))
                {
                    filterDefine = builder.Eq(d => d.ParentId, parentId);
                }
                if (menuType > 0)
                {
                    filterDefine = filterDefine & builder.Eq(d => d.MenuType, menuType);
                }
                if (string.IsNullOrEmpty(menuId) && string.IsNullOrEmpty(parentId) && menuType == 0)//iff all  null then it will return only master menu rows
                {
                    filterDefine = builder.Eq(d => d.ParentId, "");
                }

                var Result = await context.MenuMaster
                                  .Find(filterDefine)
                                  .ToListAsync().ConfigureAwait(false);

                return Result;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<MenuMaster> InsUpdMenuMasterAsync(MenuMaster menuMaster, CancellationToken cancellationToken)
        {
            try
            {
                if (menuMaster == null) throw new ArgumentNullException(nameof(menuMaster));
                if (string.IsNullOrEmpty(menuMaster.MenuId))
                {
                    await context.MenuMaster.InsertOneAsync(menuMaster).ConfigureAwait(false);
                }
                else
                {
                    var GetMenu = await GetMenuMastersAsync(menuMaster.MenuId, "", 0, null, cancellationToken).ConfigureAwait(false);

                    //if (GetMenu == null) throw new ArgumentNullException(nameof(GetMenu));


                    var update = Builders<MenuMaster>.Update
                                        .Set(x => x.MenuType, menuMaster.MenuType > 0 ? menuMaster.MenuType : GetMenu[0].MenuType)
                                        .Set(x => x.MenuName, !string.IsNullOrEmpty(menuMaster.MenuName) ? menuMaster.MenuName : GetMenu[0].MenuName)
                                        .Set(x => x.ParentId, !string.IsNullOrEmpty(menuMaster.ParentId) ? menuMaster.ParentId : GetMenu[0].ParentId)
                                        .Set(x => x.MenuIcon, !string.IsNullOrEmpty(menuMaster.MenuIcon) ? menuMaster.MenuIcon : GetMenu[0].MenuIcon)
                                        .Set(x => x.RouteLink, !string.IsNullOrEmpty(menuMaster.RouteLink) ? menuMaster.RouteLink : GetMenu[0].RouteLink);


                    var result = context.MenuMaster.UpdateOne(t => t.MenuId == menuMaster.MenuId, update);

                }
                return menuMaster;
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }
        }

        public async Task<bool> DeleteMenuMasterAsync(string menuId, CancellationToken cancellationToken)
        {
            try
            {
                var delMenu = await context.MenuMaster.FindOneAndDeleteAsync(
                         Builders<MenuMaster>.Filter.Eq("MenuId", menuId)).ConfigureAwait(false);

                return true;
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }
        }

        //Menu Access

        public async Task<List<MenuAccess>> GetMenuAccessAsync(string accessId, string menuId, string roleId, CancellationToken cancellationToken)
        {
            try
            {
                var builder = Builders<MenuAccess>.Filter;
                var filterDefine = FilterDefinition<MenuAccess>.Empty;

                if (!string.IsNullOrEmpty(accessId))
                {
                    filterDefine = builder.Eq(d => d.AccessId, accessId);
                }
                if (!string.IsNullOrEmpty(menuId))
                {
                    filterDefine = builder.Eq(d => d.MenuId, menuId);
                }
                if (!string.IsNullOrEmpty(roleId))
                {
                    filterDefine = builder.Eq(d => d.RoleId, roleId);
                }

                var Result = await context.MenuAccess
                                  .Find(filterDefine)
                                  .ToListAsync().ConfigureAwait(false);

                return Result;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> InsUpdMenuAccessAsync(List<MenuAccess> menuAccess, CancellationToken cancellationToken)
        {
            try
            {
                var getMenuMaster = await GetMenuMastersAsync(menuAccess.FirstOrDefault().MenuId, "", 0, null, cancellationToken).ConfigureAwait(false);//.FirstOrDefault().ParentId;

                var storedData = (from menu in context.MenuMaster.AsQueryable()
                                  join access in context.MenuAccess.AsQueryable() on menu.MenuId equals access.MenuId
                                  where menu.ParentId == getMenuMaster[0].ParentId
                                  select new MenuAccess
                                  {
                                      AccessId = access.AccessId,
                                      MenuId = access.MenuId,
                                      RoleId = access.RoleId
                                  }).ToList();

                var DataTobeStored = menuAccess.Except(storedData);
                var DataTobeDeleted = storedData.Except(menuAccess).Select(t => t.AccessId);

                var filterDefine = FilterDefinition<MenuAccess>.Empty;
                var builder = Builders<MenuAccess>.Filter;

                filterDefine = builder.Where(d => DataTobeDeleted.Contains(d.AccessId));
                await context.MenuAccess.InsertManyAsync(DataTobeStored).ConfigureAwait(false);
                await context.MenuAccess.DeleteManyAsync(filterDefine).ConfigureAwait(false);

                return true;

            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }
        }

        public async Task<bool> DeleteMenuAccessAsync(string accessId, CancellationToken cancellationToken)
        {
            var delmenuacc = await context.MenuAccess.FindOneAndDeleteAsync(
                        Builders<MenuAccess>.Filter.Eq("AccessId", accessId)).ConfigureAwait(false);

            return true;
        }

        public async Task<List<GetRoleWiseMenu>> GetRoleWiseMenuAsync(string ParentId, CancellationToken cancellationToken)
        {

            try
            {
                var getMenuMaster = await GetMenuMastersAsync("", ParentId, 0, null, cancellationToken).ConfigureAwait(false);

                if (getMenuMaster.Count > 0)
                {
                    var ParentType = getMenuMaster.FirstOrDefault().MenuType;

                    var roles = await _accountRepository.GetUserRolesAsync(ParentType, cancellationToken).ConfigureAwait(false);

                    return roles.Select(x => new GetRoleWiseMenu() { RoleId = x.RoleId.ToString(), RoleName = x.RoleName, MenuId = ParentId }).ToList();

                }
                else
                {
                    return new List<GetRoleWiseMenu>();
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<MenuAction>> GetRoleWiseActionAsync(string ParentId, string RoleId, CancellationToken cancellationToken)
        {
            try
            {
                //var AccessData = (from menu in context.MenuMaster.AsQueryable()
                //                 where menu.ParentId == ParentId
                //                 join access in context.MenuAccess.AsQueryable() on menu.MenuId equals access.MenuId
                //                 into accessData
                //                 from access in accessData.DefaultIfEmpty()
                //                     // access.RoleId.ToLower() == RoleId.ToLower()

                //                 select new MenuAction
                //                 {
                //                     MenuId = menu.MenuId,
                //                     MenuName = menu.MenuName,
                //                     isSelected = access.RoleId.ToLower() == RoleId.ToLower() ? true : false
                //                 })).Distinct();


                var GetMenu = await context.MenuMaster.Find(Builders<MenuMaster>.Filter.Eq(d => d.ParentId, ParentId)).ToListAsync().ConfigureAwait(false);
                var GetAccess = context.MenuAccess.Find(Builders<MenuAccess>.Filter.Eq(d => d.MenuId, ParentId) | Builders<MenuAccess>.Filter.Eq(d => d.RoleId, RoleId)).ToList();

                var AccessData = GetMenu.Select(x =>
                  new MenuAction
                  {
                      MenuId = x.MenuId,
                      MenuName = x.MenuName,
                      isSelected = GetAccess.Where(y => y.RoleId == RoleId && y.MenuId == x.MenuId).ToList().Count > 0 ? true : false
                  }).ToList();

                return AccessData.ToList();
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }
        }


        public async Task<List<GetLoginWiseMenu>> GetLoginWiseMenuAsync(string UserId, CancellationToken cancellationToken)
        {

            try
            {

                var userMaster = await _accountRepository.GetUserMasterAsync(0, UserId, "", 0, cancellationToken).ConfigureAwait(false);
                var menuData = new List<GetLoginWiseMenu>();
                if (userMaster[0].RoleType == 2)
                {
                    var GetMenu = context.MenuMaster.Find(Builders<MenuMaster>.Filter.Eq(d => d.ParentId, "") & Builders<MenuMaster>.Filter.Eq(d => d.MenuType, userMaster[0].RoleType)).ToList();

                    List<RoleMaster> allRoles = await _accountRepository.GetUserRolesAsync(userMaster[0].RoleType, cancellationToken).ConfigureAwait(false);

                    var userroleid = allRoles.Where(y => y.RoleName == userMaster[0].RoleList[0]).Select(x => x.RoleId).FirstOrDefault();

                    menuData = GetMenu.Select(x => new GetLoginWiseMenu()
                    {
                        MenuId = x.MenuId.ToString(),
                        MenuName = x.MenuName,
                        RouteLink = x.RouteLink,
                        RoleId = userroleid.ToString()
                    }).ToList();
                }

                return menuData.ToList();

            }
            catch (Exception)
            {

                throw;
            }
        }


        public async Task<List<SubMenuList>> GetSubMenusAsync(string ParentId, string RoleId, CancellationToken cancellationToken)
        {
            try
            {
                var GetMenu = await context.MenuMaster.Find(Builders<MenuMaster>.Filter.Eq(d => d.ParentId, ParentId)).ToListAsync().ConfigureAwait(false);

                var AccessData = GetMenu.Select(x =>
                  new SubMenuList
                  {
                      MenuId = x.MenuId,
                      MenuName = x.MenuName,
                      RouteLink = x.RouteLink,
                      RoleId = RoleId,
                  }).ToList();

                return AccessData.ToList();
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }
        }

        public async Task<List<MenuMaster>> GetSubMenusTestAsync(string ParentId, string RoleId, IDictionary<string, Field> fields, CancellationToken cancellationToken)
        {
            try
            {

                var GetMenu = new List<MenuMaster>();
                var fieldsName = _commonRepository.GetFieldsName(fields);
                var fieldsBuilder = Builders<MenuMaster>.Projection;
                var fieldsAdd = fieldsBuilder.Include(fieldsName[0]);
                foreach (var item in fieldsName.Skip(1))
                {
                    fieldsAdd = fieldsAdd.Include(item);
                }
                if (fieldsName.Any(str => str.Contains("menuAccess", StringComparison.CurrentCulture)))
                {
                    GetMenu = await context.MenuMaster.Aggregate()
                    .Match(Builders<MenuMaster>.Filter.Eq(d => d.ParentId, ParentId))
                    .Lookup(
                 context.MenuAccess,
                 m => m.MenuId,
                 c => c.MenuId,
                 (MenuMaster m) => m.MenuAccess)
                .Project<MenuMaster>(fieldsAdd)
                .ToListAsync().ConfigureAwait(false);
                }
                else
                {
                    GetMenu = await context.MenuMaster.Aggregate()
                    .Match(Builders<MenuMaster>.Filter.Eq(d => d.ParentId, ParentId))
                    .Project<MenuMaster>(fieldsAdd)
                    .ToListAsync().ConfigureAwait(false);
                }


                return GetMenu.ToList();
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }
        }

        public async Task<List<ChildMenuList>> GetChildMenusAsync(string ParentId, string RoleId, CancellationToken cancellationToken)
        {
            try
            {
                var GetMenu = await context.MenuMaster.Find(Builders<MenuMaster>.Filter.Eq(d => d.ParentId, ParentId)).ToListAsync().ConfigureAwait(false);
                var GetAccess = context.MenuAccess.Find(Builders<MenuAccess>.Filter.Eq(d => d.RoleId, RoleId)).ToList();

                var AllowedMenuActionIds = GetAccess.Select(a => a.MenuId).ToList();
                var AllowedMenuData = GetMenu.Where(a => AllowedMenuActionIds.Contains(a.MenuId)).ToList();

                var AccessData = AllowedMenuData.Select(x =>
                new ChildMenuList
                {
                    MenuId = x.MenuId,
                    MenuName = x.MenuName,
                    RouteLink = x.RouteLink,
                }).ToList();


                return AccessData.ToList();
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }
        }




    }
}
