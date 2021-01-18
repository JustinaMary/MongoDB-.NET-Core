using GraphQL.Types;

namespace PetizenApi.Types.Inputs
{
    public class MenuMasterInput : InputObjectGraphType
    {
        public MenuMasterInput()
        {
            Name = "menuMasterInput";
            Field<NonNullGraphType<StringGraphType>>("menuId");
            Field<NonNullGraphType<IntGraphType>>("menuType");
            Field<StringGraphType>("menuName");
            Field<StringGraphType>("parentId");
            Field<StringGraphType>("menuIcon");
            Field<StringGraphType>("routeLink");
            Field<StringGraphType>("insertedBy");
            Field<BooleanGraphType>("isChild");
        }

    }

    public class MenuAccessInput : InputObjectGraphType
    {
        public MenuAccessInput()
        {
            Name = "menuAccessInput";
            Field<NonNullGraphType<StringGraphType>>("accessId");
            Field<NonNullGraphType<StringGraphType>>("menuId");
            Field<StringGraphType>("roleId");
            Field<StringGraphType>("insertedBy");
        }
    }
}
