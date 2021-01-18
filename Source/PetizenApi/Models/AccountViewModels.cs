using Microsoft.AspNetCore.Mvc.Rendering;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.GeoJsonObjectModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PetizenApi.Models
{
    public class UserMaster
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailId { get; set; }
        public string ContactNo { get; set; }
        public string ProfilePicture { get; set; }
        public string About { get; set; }
        public string Location { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public Guid InsertedBy { get; set; }
        public DateTime InsertedDate { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        // for registration

        public string AllRolesStr { get; set; }
        public int RoleType { get; set; }
        public string Password { get; set; }
        public bool fromBackend { get; set; } = false;
        public bool isFrontUser { get; set; } = false;

        private readonly List<string> items = new List<string>();
        public List<string> RoleList { get { return items; } }
        public string IpAddress { get; set; }

        //external registraion
        public string DisplayName { get; set; }
        public string LoginProvider { get; set; }
        public string ProviderKey { get; set; }

    }



    public class ExternalLogin
    {
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string LoginProvider { get; set; }
        public string ProviderKey { get; set; }
        public bool IsPersistent { get; set; }
        public bool BypassTwoFactor { get; set; }
        public string IpAddress { get; set; }
    }

    public class LoginViewModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
        public string IpAddress { get; set; }
    }

    public class LoginResponse
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public string ProfileImg { get; set; }
        public string Rolenames { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime TokenExpiry { get; set; }
        public string Message { get; set; }
    }

    public class ExternalLoginConfirmationViewModel
    {
        public string Email { get; set; }
    }

    public class ConfirmEmailViewModel
    {
        public string Email { get; set; }
        public string Token { get; set; }
    }


    public class ResetPasswordViewModel
    {

        public string UserId { get; set; }
        public string Password { get; set; }
        public string Code { get; set; }
    }

    public class ManagePasswordViewModel
    {

        public string UserId { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<SelectListItem> Providers { get; }
        public System.Uri ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class UseRecoveryCodeViewModel
    {
        [Required]
        public string Code { get; set; }
        public System.Uri ReturnUrl { get; set; }
    }

    public class VerifyAuthenticatorCodeViewModel
    {
        public string Code { get; set; }
        public System.Uri ReturnUrl { get; set; }
        public bool RememberBrowser { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        public string Provider { get; set; }
        public string Code { get; set; }
        public System.Uri ReturnUrl { get; set; }
        public bool RememberBrowser { get; set; }
        public bool RememberMe { get; set; }
    }

    public class SuccessResponse
    {

        public bool isSuccess { get; set; }
        public string Message { get; set; }

    }

    public class RoleMaster
    {
        public Guid RoleId { get; set; }
        public string RoleName { get; set; }

    }


    public class PasswordHistory
    {
        public int HistoryId { get; set; }
        public string Email { get; set; }
        public string HashedPassword { get; set; }
        public string CreatedDate { get; set; }
    }

    #region UserProfile


    public class UserDetails
    {
        public int Id { get; set; }

        public Guid UserId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string EmailId { get; set; }

        public string ContactNo { get; set; }

        public string About { get; set; }

        public string ProfilePic { get; set; }

        public string AllRolesStr { get; set; }

        public List<MultipleImages> UserImagesList { get; set; }

        public List<MyUserRole> UserRoleList { get; set; }

        public List<DogCourses> DogCoursesList { get; set; }//trainer

        public List<DogMaster> DogList { get; set; }//owner


    }



    public class MyUserRole
    {
        public string RoleId { get; set; }

        public string RoleName { get; set; }

        public List<MyLocation> LocationList { get; set; }

    }

    public class MyLocation
    {

        public string Name { get; set; } = "";  //like clinic name or SPA brand name

        public string Location { get; set; } = "";

        public List<DogServices> ServicesList { get; set; }

        public List<MultipleImages> LocationImagesList { get; set; } //For only Vet and spa

    }



    public class MultipleImages
    {
        public string MediaTitle { get; set; }

        public string MediaPath { get; set; }

        public bool IsProfilePic { get; set; }

    }

    //public class MyDogCourses
    //{
    //    public string CourseId { get; set; }

    //    public string Title { get; set; } = "";

    //    public string Description { get; set; } = "";

    //    public long Amount { get; set; }

    //    public string ImagePath { get; set; } = "";

    //}


    //public class MyDog
    //{
    //    public string DogId { get; set; }

    //    public string Name { get; set; } = "";

    //    public string Lineage { get; set; } = "";

    //    public string breedName { get; set; } = "";

    //    public string Gender { get; set; } = "";

    //    public DateTime DOB { get; set; }

    //    public List<MyDogMedia> DogMediaList { get; set; }

    //}

    //public class MyDogMedia
    //{
    //    public int MediaType { get; set; }

    //    public int FileType { get; set; }

    //    public string MediaUrl { get; set; } = "";

    //}



    #endregion


    public class UserAddress
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string AddressId { get; set; }
        public Guid UserId { get; set; }
        public string Location { get; set; } = "";
        public GeoJsonPoint<GeoJson2DGeographicCoordinates> Coordinates { get; set; }
        public string InsertedBy { get; set; } = "";
        public DateTime InsertedDate { get; set; } = DateTime.Now;
    }


}
