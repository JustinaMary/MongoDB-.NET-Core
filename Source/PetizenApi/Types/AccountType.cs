using GraphQL.Types;
using PetizenApi.Models;

namespace PetizenApi.Types
{
    public class LoginResponseType : ObjectGraphType<LoginResponse>
    {
        public LoginResponseType()
        {
            Field(x => x.Id, nullable: true);
            Field(x => x.UserId, nullable: true, type: typeof(IdGraphType)).Description("UserId for login");
            Field(x => x.Username, nullable: true);
            Field(x => x.ProfileImg, nullable: true);
            Field(x => x.Rolenames, nullable: true);
            Field(x => x.Latitude, nullable: true);
            Field(x => x.Longitude, nullable: true);
            Field(x => x.AccessToken, nullable: true);
            Field(x => x.RefreshToken, nullable: true);
            Field(x => x.TokenExpiry, nullable: true);
            Field(x => x.Message);

        }
    }

    public class UserMasterType : ObjectGraphType<UserMaster>
    {
        public UserMasterType()
        {
            Field(x => x.Id);
            Field(x => x.UserId, type: typeof(IdGraphType)).Description("UserId of the User");
            Field(x => x.FirstName);
            Field(x => x.LastName);
            Field(x => x.EmailId);
            Field(x => x.ContactNo);
            Field(x => x.Location);
            Field(x => x.Latitude);
            Field(x => x.Longitude);
            Field(x => x.ProfilePicture);
            Field(x => x.About);
            Field(x => x.AllRolesStr);
            Field(x => x.RoleList);
        }
    }

    public class SuccessResponseType : ObjectGraphType<SuccessResponse>
    {
        public SuccessResponseType()
        {
            Field(x => x.isSuccess);
            Field(x => x.Message);

        }
    }

    public class RoleMasterType : ObjectGraphType<RoleMaster>
    {
        public RoleMasterType()
        {
            Field(x => x.RoleId, type: typeof(IdGraphType)).Description("Id of the Role");
            Field(x => x.RoleName);

        }
    }


    public class UserDetailsType : ObjectGraphType<UserDetails>
    {
        public UserDetailsType()
        {
            Field(x => x.Id);
            Field(x => x.UserId, type: typeof(IdGraphType));
            Field(x => x.FirstName);
            Field(x => x.LastName);
            Field(x => x.EmailId);
            Field(x => x.ContactNo);
            Field(x => x.About);
            Field(x => x.AllRolesStr);
            Field(x => x.ProfilePic);
            Field(x => x.UserImagesList, type: typeof(ListGraphType<MultipleImagesType>));
            Field(x => x.UserRoleList, type: typeof(ListGraphType<MyUserRoleType>));
            Field(x => x.DogCoursesList, type: typeof(ListGraphType<DogCoursesType>));
            Field(x => x.DogList, type: typeof(ListGraphType<DogMasterType>));
        }
    }


    public class MultipleImagesType : ObjectGraphType<MultipleImages>
    {
        public MultipleImagesType()
        {
            Field(x => x.MediaTitle);
            Field(x => x.MediaPath);
            Field(x => x.IsProfilePic);

        }
    }

    public class MyUserRoleType : ObjectGraphType<MyUserRole>
    {
        public MyUserRoleType()
        {
            Field(x => x.RoleId);
            Field(x => x.RoleName);
            Field(x => x.LocationList, type: typeof(ListGraphType<MyLocationType>));

        }
    }



    public class MyLocationType : ObjectGraphType<MyLocation>
    {
        public MyLocationType()
        {
            Field(x => x.Name);
            Field(x => x.Location);
            Field(x => x.ServicesList, type: typeof(ListGraphType<DogServicesType>));
            Field(x => x.LocationImagesList, type: typeof(ListGraphType<MultipleImagesType>));

        }
    }


}
