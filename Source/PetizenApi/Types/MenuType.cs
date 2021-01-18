using GraphQL.Types;
using PetizenApi.Models;
using PetizenApi.Providers;

namespace PetizenApi.Types
{
    public class MenuMasterType : ObjectGraphType<MenuMaster>
    {
        public MenuMasterType()
        {
            Field(x => x.MenuId);
            Field(x => x.MenuType);
            Field(x => x.MenuName);
            Field(x => x.ParentId);
            Field(x => x.MenuIcon);
            Field(x => x.RouteLink);
            Field(x => x.InsertedBy);
            Field(x => x.InsertedDate);
            Field(x => x.IsChild);
            Field(x => x.MenuAccess, type: typeof(ListGraphType<MenuAccessType>));
        }
    }


    public class MenuAccessType : ObjectGraphType<MenuAccess>
    {
        public MenuAccessType()
        {
            Field(x => x.AccessId);
            Field(x => x.MenuId);
            Field(x => x.RoleId);
            Field(x => x.InsertedBy);
            Field(x => x.InsertedDate);
        }
    }

    public class MenuActionType : ObjectGraphType<MenuAction>
    {
        public MenuActionType()
        {
            Field(x => x.MenuId);
            Field(x => x.MenuName);
            Field(x => x.isSelected);

        }
    }

    public class GetRoleWiseMenuType : ObjectGraphType<GetRoleWiseMenu>
    {
        public GetRoleWiseMenuType(ContextServiceLocator contextServiceLocator)
        {
            Field(x => x.RoleId);
            Field(x => x.RoleName);
            Field(x => x.MenuId);

            Field<ListGraphType<MenuActionType>>("menu",
                  arguments: new QueryArguments(new QueryArgument<StringGraphType> { Name = "roleId" }),
                  resolve: context => contextServiceLocator.MenuRepository.GetRoleWiseActionAsync(context.Source.MenuId, context.Source.RoleId, context.CancellationToken), description: "Role Id");

        }
    }




    public class GetLoginWiseMenuType : ObjectGraphType<GetLoginWiseMenu>
    {
        public GetLoginWiseMenuType(ContextServiceLocator contextServiceLocator)
        {
            Field(x => x.MenuId);
            Field(x => x.MenuName);
            Field(x => x.RouteLink);
            Field(x => x.RoleId);

            Field<ListGraphType<SubMenuType>>("subMenu",
                  arguments: new QueryArguments(new QueryArgument<StringGraphType> { Name = "roleId" }),//dont know use of this line
                  resolve: context => contextServiceLocator.MenuRepository.GetSubMenusAsync(context.Source.MenuId, context.Source.RoleId, context.CancellationToken), description: "Role Id");

        }
    }


    public class SubMenuType : ObjectGraphType<SubMenuList>
    {
        public SubMenuType(ContextServiceLocator contextServiceLocator)
        {
            Field(x => x.MenuId);
            Field(x => x.MenuName);
            Field(x => x.RouteLink);
            Field(x => x.RoleId);

            Field<ListGraphType<ChildMenuType>>("childMenu",
                 arguments: new QueryArguments(new QueryArgument<StringGraphType> { Name = "roleId" }),
                 resolve: context => contextServiceLocator.MenuRepository.GetChildMenusAsync(context.Source.MenuId, context.Source.RoleId, context.CancellationToken), description: "Role Id");

        }
    }

    public class ChildMenuType : ObjectGraphType<ChildMenuList>
    {
        public ChildMenuType()
        {
            Field(x => x.MenuId);
            Field(x => x.MenuName);
            Field(x => x.RouteLink);
        }
    }


}
