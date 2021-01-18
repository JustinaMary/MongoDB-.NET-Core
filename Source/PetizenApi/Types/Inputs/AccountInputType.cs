using GraphQL.Types;

namespace PetizenApi.Types.Inputs
{
    public class LoginViewModelInput : InputObjectGraphType
    {
        public LoginViewModelInput()
        {
            Name = "loginViewModelInput";
            Field<NonNullGraphType<StringGraphType>>("email");
            Field<NonNullGraphType<StringGraphType>>("password");
            Field<BooleanGraphType>("rememberMe");
        }
    }

    public class UserMasterInput : InputObjectGraphType
    {
        public UserMasterInput()
        {
            Name = "userMasterInput";
            Field<NonNullGraphType<IdGraphType>>("id");
            Field<StringGraphType>("name");
            Field<StringGraphType>("emailId");
            Field<StringGraphType>("contactNo");
            Field<StringGraphType>("location");
            Field<StringGraphType>("latitude");
            Field<StringGraphType>("longitude");
            Field<StringGraphType>("profilePicture");
            Field<StringGraphType>("about");
            Field<StringGraphType>("insertedBy");
            //Field<StringGraphType>("role");
            Field<StringGraphType>("password");
            Field<BooleanGraphType>("fromBackend");
            Field<BooleanGraphType>("isFrontUser");
            Field<ListGraphType<StringGraphType>>("roleList");


        }
    }

    public class ExternalLoginInput : InputObjectGraphType
    {
        public ExternalLoginInput()
        {
            Name = "externalLoginInput";
            Field<StringGraphType>("email");
            Field<StringGraphType>("displayName");
            Field<StringGraphType>("loginProvider");
            Field<StringGraphType>("providerKey");
            Field<StringGraphType>("isPersistent");
            Field<StringGraphType>("bypassTwoFactor");
        }
    }

    public class ExternalRegisterInput : InputObjectGraphType
    {
        public ExternalRegisterInput()
        {
            Name = "externalRegisterInput";
            Field<ExternalLoginInput>("externalLogin");
            Field<UserMasterInput>("userMaster");

        }
    }

    public class ResetPasswordInput : InputObjectGraphType
    {
        public ResetPasswordInput()
        {
            Name = "resetPasswordInput";
            Field<StringGraphType>("userId");
            Field<StringGraphType>("password");
            Field<StringGraphType>("code");

        }
    }

    public class ManagePasswordInput : InputObjectGraphType
    {
        public ManagePasswordInput()
        {
            Name = "managePasswordInput";
            Field<StringGraphType>("userId");
            Field<StringGraphType>("oldPassword");
            Field<StringGraphType>("newPassword");

        }
    }
}
