using Dapper;
using GraphQL.Language.AST;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.GeoJsonObjectModel;
using PetizenApi.Database;
using PetizenApi.Interfaces;
using PetizenApi.Models;
using PetizenApi.Providers;
using PetizenApi.Services;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Threading;
using System.Threading.Tasks;

[assembly: NeutralResourcesLanguageAttribute("en-US")]
namespace PetizenApi.Repositories
{

    public class AccountRepository : IAccountRepository
    {
        //private readonly UserManager<ApplicationUser> _userManager;
        //private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConnectionFactory _connectionFactory;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly ApplicationUrl _appurl;
        private readonly IEmailRepository _emailRepository;
        private readonly ICommonRepository _commonRepository;
        private readonly ILocationServiceRepository _locationService;
        private readonly IDogRepository _dogRepository;
        private readonly MongoConnection context = null;

        private readonly string MediaUrl = "";
        private readonly IStringLocalizer<AccountRepository> _localizer;

        public AccountRepository(//UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
            IConnectionFactory connectionFactory, ITokenGenerator tokenGenerator, IOptions<ApplicationUrl> appurl, IEmailRepository emailRepository,
            IOptions<ApplicationUrl> webUrl, IOptions<MongoSettings> settings, IStringLocalizer<AccountRepository> localizer, ILocationServiceRepository locationService, IDogRepository dogRepository,
            ICommonRepository commonRepository)
        {
            //_userManager = userManager;
            //_signInManager = signInManager;

            _connectionFactory = connectionFactory;
            _tokenGenerator = tokenGenerator;
            if (appurl == null) throw new ArgumentNullException(nameof(appurl));
            _appurl = appurl.Value;
            _emailRepository = emailRepository;

            context = new MongoConnection(settings);
            _locationService = locationService;
            _dogRepository = dogRepository;
            _commonRepository = commonRepository;

            if (webUrl == null) throw new ArgumentNullException(nameof(webUrl));
            MediaUrl = webUrl.Value.MediaUrl.ToString();
            _localizer = localizer;

        }



        public async Task<List<UserMaster>> GetUserMasterAsync(int Id, string UserId, string Email, int UserType, CancellationToken cancellationToken)
        {
            try
            {
                var userMaster = Enumerable.Empty<UserMaster>();

                var procedureName = "usp_UserMasterGet";
                var param = new Dapper.DynamicParameters();

                Guid ValidUserId = Guid.Empty;

                param.Add("@Id", Id);
                param.Add("@UserId", Guid.TryParse(UserId, out ValidUserId) ? ValidUserId : Guid.Empty);
                param.Add("@Email", Email == null ? "" : Email);
                param.Add("@UserType", UserType);
                //_connectionFactory.CloseConnection();
                try
                {
                    using (var multiResult = await SqlMapper.QueryMultipleAsync(_connectionFactory.GetConnection,
                    procedureName, param, commandType: CommandType.StoredProcedure).ConfigureAwait(false))
                    {
                        userMaster = await multiResult.ReadAsync<UserMaster>().ConfigureAwait(false);
                        //userMaster.Select(c => { c.ProfilePicture = string.IsNullOrEmpty(c.ProfilePicture) ? c.ProfilePicture : MediaUrl + "/" + c.ProfilePicture; return c; }).ToList();

                    }
                }
                finally
                {
                    _connectionFactory.CloseConnection();
                }


                //togetprofilepic

                var Resultprofilpic = await context.UserLocationMedia.Find(Builders<UserLocationMedia>.Filter.Eq(d => d.IsProfilePic, true)).ToListAsync().ConfigureAwait(false);
                userMaster = userMaster.Select(c => { c.ProfilePicture = string.IsNullOrEmpty(Resultprofilpic.Where(x => x.UserId == c.UserId.ToString()).Select(y => y.MediaPath).FirstOrDefault()) ? "" : MediaUrl + Resultprofilpic.Where(x => x.UserId == c.UserId.ToString()).Select(y => y.MediaPath).FirstOrDefault(); return c; }).ToList();
                userMaster = userMaster.Select(c => { c.Location = ""; return c; }).ToList();
                userMaster = userMaster.Select(c => { c.Latitude = 0; return c; }).ToList();
                userMaster = userMaster.Select(c => { c.Longitude = 0; return c; }).ToList();





                if (Id != 0 || ValidUserId != Guid.Empty || !string.IsNullOrEmpty(Email))//for single user
                {

                    //togetuseraddress
                    var UserAddress = await context.UserAddress.Find(t => t.UserId == userMaster.ToList()[0].UserId).FirstOrDefaultAsync().ConfigureAwait(false);
                    if (UserAddress != null)
                    {
                        userMaster.ToList()[0].Location = UserAddress.Location;
                        userMaster.ToList()[0].Latitude = UserAddress.Coordinates.Coordinates.Latitude;
                        userMaster.ToList()[0].Longitude = UserAddress.Coordinates.Coordinates.Longitude;

                    }


                    var userrolesList = await GetMyRolesArrayAsync(Id, UserId).ConfigureAwait(false);
                    userMaster.ToList()[0].RoleList.AddRange(userrolesList.Select(x => x.RoleName).ToList());

                }


                return userMaster.ToList();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception");
                throw;
            }
        }


        public async Task<List<RoleMaster>> GetMyRolesArrayAsync(int Id, string UserId)
        {
            try
            {
                List<RoleMaster> roles = new List<RoleMaster>();

                var procedureName = "usp_AspnetUserRolesGet";
                var param = new DynamicParameters();
                if (Id != 0)
                {
                    param.Add("@Id", Id);
                }
                if (!string.IsNullOrEmpty(UserId))
                {
                    param.Add("@UserId", UserId);
                }
                param.Add("@IsRoleId", true);


                try
                {
                    using (var multiResult = await SqlMapper.QueryMultipleAsync(_connectionFactory.GetConnection,
                    procedureName, param, commandType: CommandType.StoredProcedure).ConfigureAwait(false))
                    {
                        roles = (await multiResult.ReadAsync<RoleMaster>().ConfigureAwait(false)).ToList();
                    }
                }
                finally
                {
                    _connectionFactory.CloseConnection();
                }




                return roles;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception");
                throw;
            }
        }





        public async Task<List<UserDetails>> GetUserDetailsAsync(string UserId, IDictionary<string, Field> fields, CancellationToken cancellationToken)
        {
            try
            {
                var userMaster = new List<UserMaster>();
                var userDetails = new List<UserDetails>();
                var userDetailSingle = new UserDetails();
                userDetailSingle.UserImagesList = new List<MultipleImages>();
                userDetailSingle.UserRoleList = new List<MyUserRole>();
                userDetailSingle.DogCoursesList = new List<DogCourses>();
                userDetailSingle.DogList = new List<DogMaster>();

                var fieldsName = _commonRepository.GetFieldsName(fields);

                if (fieldsName.Any(str => str.Contains("firstName", StringComparison.CurrentCulture)))
                {
                    var procedureName = "usp_UserMasterGet";
                    var param = new DynamicParameters();

                    Guid ValidUserId = Guid.Empty;

                    param.Add("@UserId", Guid.TryParse(UserId, out ValidUserId) ? ValidUserId : Guid.Empty);

                    try
                    {
                        using (var multiResult = await SqlMapper.QueryMultipleAsync(_connectionFactory.GetConnection,
                        procedureName, param, commandType: CommandType.StoredProcedure).ConfigureAwait(false))
                        {
                            userMaster = (await multiResult.ReadAsync<UserMaster>().ConfigureAwait(false)).ToList();
                            userMaster = userMaster.Select(c => { c.ProfilePicture = string.IsNullOrEmpty(c.ProfilePicture) ? c.ProfilePicture : MediaUrl + c.ProfilePicture; return c; }).ToList();

                        }

                        userDetailSingle.Id = userMaster[0].Id;
                        userDetailSingle.UserId = userMaster[0].UserId;
                        userDetailSingle.FirstName = userMaster[0].FirstName;
                        userDetailSingle.LastName = userMaster[0].LastName;
                        userDetailSingle.EmailId = userMaster[0].EmailId;
                        userDetailSingle.ContactNo = userMaster[0].ContactNo;
                        userDetailSingle.About = userMaster[0].About;
                        userDetailSingle.AllRolesStr = userMaster[0].AllRolesStr;

                        userDetailSingle.ProfilePic = "";


                        var UserImagesList = await _locationService.GetUserLocationMediaAsync("", "", userMaster[0].UserId.ToString(), cancellationToken).ConfigureAwait(false);
                        if (UserImagesList.Count > 0)
                        {
                            userDetailSingle.UserImagesList = UserImagesList.Select(x =>
                                new MultipleImages
                                {
                                    MediaTitle = x.MediaTitle,
                                    MediaPath = x.MediaPath,
                                    IsProfilePic = x.IsProfilePic,

                                }).ToList();

                            var profPic = UserImagesList.Where(x => x.IsProfilePic).Select(y => y.MediaPath).FirstOrDefault();
                            if (profPic == null)
                            {
                                profPic = UserImagesList.Select(y => y.MediaPath).FirstOrDefault();
                            }
                            if (profPic == null)
                            {
                                profPic = "";
                            }
                            userDetailSingle.ProfilePic = profPic;
                        }

                    }
                    finally
                    {
                        _connectionFactory.CloseConnection();
                    }

                }


                #region DogCoursesList
                if (fieldsName.Any(str => str.Contains("dogCoursesList", StringComparison.CurrentCulture)) && userMaster[0].AllRolesStr.ToLower(CultureInfo.CurrentCulture).IndexOf("trainer", StringComparison.CurrentCulture) > -1)
                {
                    List<DogCourses> dogcourseslist = new List<DogCourses>();
                    dogcourseslist = await _dogRepository.GetDogCoursesAsync("", userMaster[0].UserId.ToString(), null, cancellationToken).ConfigureAwait(false);
                    userDetailSingle.DogCoursesList = dogcourseslist;
                }
                #endregion

                #region DogList
                if (fieldsName.Any(str => str.Contains("dogList", StringComparison.CurrentCulture)) && userMaster[0].AllRolesStr.ToLower(CultureInfo.CurrentCulture).IndexOf("parent", StringComparison.CurrentCulture) > -1)
                {
                    List<DogMaster> doglist = new List<DogMaster>();
                    doglist = await _dogRepository.GetDogForDetailsAsync(userMaster[0].UserId.ToString(), cancellationToken).ConfigureAwait(false);
                    userDetailSingle.DogList = doglist;
                }
                #endregion






                userDetails.Add(userDetailSingle);

                return userDetails;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception");
                throw;
            }
        }

        public async Task<List<MyUserRole>> GetUserDetailsRolWiseAsync(string UserId, IDictionary<string, Field> fields, CancellationToken cancellationToken)
        {
            try
            {
                var UserRoleList = new List<MyUserRole>();

                var fieldsName = _commonRepository.GetFieldsName(fields);

                try
                {

                    //Userroles
                    var roles = await GetMyRolesArrayAsync(0, UserId).ConfigureAwait(false);

                    var builder = Builders<UserLocation>.Filter;
                    var filterDefine = FilterDefinition<UserLocation>.Empty;

                    if (!string.IsNullOrEmpty(UserId))
                    {
                        filterDefine = filterDefine & builder.Eq(d => d.UserId, UserId);
                    }

                    var ResultUserLocation = await context.UserLocation.Find(filterDefine).ToListAsync().ConfigureAwait(false);

                    foreach (var role in roles)
                    {
                        MyUserRole userrole = new MyUserRole();

                        userrole.RoleId = role.RoleId.ToString();
                        userrole.RoleName = role.RoleName;
                        userrole.LocationList = new List<MyLocation>();

                        #region location
                        var rolewiseLoc = ResultUserLocation.Where(x => x.RoleId.ToLower(CultureInfo.CurrentCulture) == role.RoleId.ToString().ToLower(CultureInfo.CurrentCulture)).ToList();
                        if (rolewiseLoc.Count > 0)
                        {
                            if (role.RoleName == "Dog Walker" || role.RoleName == "Trainer")
                            {
                                foreach (var item1 in rolewiseLoc[0].LocationTwo)
                                {
                                    userrole.LocationList.Add(new MyLocation()
                                    {
                                        Name = "",
                                        Location = item1,
                                        LocationImagesList = new List<MultipleImages>(),
                                        ServicesList = new List<DogServices>()
                                    });
                                }


                            }
                            else
                            {

                                if (fieldsName.Any(str => str.Contains("locationList", StringComparison.CurrentCulture)))
                                {
                                    foreach (var item1 in rolewiseLoc)
                                    {
                                        var loc = new MyLocation();
                                        loc.Name = item1.Name;
                                        loc.Location = item1.LocationOne;
                                        loc.ServicesList = new List<DogServices>();
                                        loc.LocationImagesList = new List<MultipleImages>();
                                        if (fieldsName.Any(str => str.Contains("servicesList", StringComparison.CurrentCulture)))
                                        {
                                            var serviceList = await _locationService.GetDogServicesAsync("", item1.LocationId, "", "", null, cancellationToken).ConfigureAwait(false);
                                            if (serviceList.Count > 0)
                                            {
                                                loc.ServicesList = serviceList;
                                            }
                                        }
                                        if (role.RoleName == "Vet" && fieldsName.Any(str => str.Contains("locationImagesList", StringComparison.CurrentCulture)))
                                        {
                                            var userLocationMedia = await _locationService.GetUserLocationMediaAsync("", item1.LocationId, "", cancellationToken).ConfigureAwait(false);
                                            if (userLocationMedia.Count > 0)
                                            {
                                                loc.LocationImagesList = userLocationMedia.Select(x =>
                                                    new MultipleImages
                                                    {
                                                        MediaTitle = x.MediaTitle,
                                                        MediaPath = x.MediaPath,
                                                        IsProfilePic = x.IsProfilePic,
                                                    }).ToList();
                                            }

                                        }

                                        userrole.LocationList.Add(loc);
                                    }
                                }
                            }
                        }
                        #endregion

                        UserRoleList.Add(userrole);
                    }

                    //userroles end
                }
                finally
                {
                    _connectionFactory.CloseConnection();
                }

                return UserRoleList;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception");
                throw;
            }
        }



        public async Task<bool> IsEmailExistAsync(string Email, CancellationToken cancellationToken)
        {
            try
            {
                var EmailId = "";
                var isExist = false;
                var procedureName = "usp_IsEmailExist";
                var param = new DynamicParameters();
                param.Add("@Email", Email);

                try
                {
                    using (var multiResult = await SqlMapper.QueryMultipleAsync(_connectionFactory.GetConnection,
                    procedureName, param, commandType: CommandType.StoredProcedure).ConfigureAwait(false))
                    {
                        EmailId = await multiResult.ReadFirstOrDefaultAsync<string>().ConfigureAwait(false);

                    }
                }
                finally
                {
                    _connectionFactory.CloseConnection();
                }
                if (!string.IsNullOrEmpty(EmailId))
                {
                    isExist = true;
                }
                return isExist;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception");
                throw;
            }
        }
        //Role type 1 for front-end user and 2 for back-office users
        public async Task<List<RoleMaster>> GetUserRolesAsync(int RoleType, CancellationToken cancellationToken)
        {

            try
            {
                var roles = Enumerable.Empty<RoleMaster>();
                var procedureName = "usp_AspnetRolesGet";
                var param = new Dapper.DynamicParameters();
                param.Add("@RoleType", RoleType);
                try
                {
                    using (var multiResult = await SqlMapper.QueryMultipleAsync(_connectionFactory.GetConnection,
                    procedureName, param, commandType: CommandType.StoredProcedure).ConfigureAwait(false))
                    {
                        roles = await multiResult.ReadAsync<RoleMaster>().ConfigureAwait(false);

                    }
                }
                finally
                {
                    _connectionFactory.CloseConnection();
                }
                return roles.ToList();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception");
                throw;
            }
        }


        public async Task<bool> DeleteUserAsync(int Id, CancellationToken cancellationToken)
        {
            try
            {

                var isDeleted = false;
                var procedureName = "usp_DeleteUserMaster";
                var param = new DynamicParameters();
                param.Add("@Id", Id);
                var newid = 0;

                try
                {
                    newid = await _connectionFactory.GetConnection.ExecuteAsync(procedureName, param, commandType: CommandType.StoredProcedure)
                        .ConfigureAwait(false);
                    //returns no of rows affected
                }
                finally
                {
                    _connectionFactory.CloseConnection();
                }

                if (newid > 0)
                {
                    isDeleted = true;
                }
                return isDeleted;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception");
                throw;
            }
        }

        public bool UpdateUserRoles(string UserId, string DeletedRoles, string AddedRoles)
        {
            try
            {

                var isUpdated = false;
                var procedureName = "usp_InsUpdUserRoles";
                var param = new DynamicParameters();
                param.Add("@deletedroles", DeletedRoles);
                param.Add("@addedroles", AddedRoles);
                param.Add("@userid", UserId);

                var newid = 0;

                try
                {
                    newid = _connectionFactory.GetConnection.Execute(procedureName, param, commandType: CommandType.StoredProcedure);
                    //returns no of rows affected

                }
                finally
                {
                    _connectionFactory.CloseConnection();
                }

                if (newid > 0)
                {
                    isUpdated = true;
                }
                return isUpdated;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception");
                throw;
            }
        }


        //to insert the user in usermaster
        public async Task<UserMaster> InsertUpdateUserMasterAsync(UserMaster userMaster, CancellationToken cancellationToken)
        {
            try
            {
                var oldUserMaster = userMaster;
                if (userMaster == null) throw new ArgumentNullException(nameof(userMaster));
                var location = new { userMaster.Location, userMaster.Longitude, userMaster.Latitude };
                //var userMasterRoleList = userMaster.RoleList;
                var procedureName = "usp_UserMasterInsUpd";
                var param = new DynamicParameters();
                param.Add("@Id", userMaster.Id);
                param.Add("@FirstName", userMaster.FirstName);
                param.Add("@LastName", userMaster.LastName);
                param.Add("@ContactNo", userMaster.ContactNo);
                param.Add("@About", !string.IsNullOrEmpty(userMaster.About) ? userMaster.About : null);
                param.Add("@InsertedBy", userMaster.InsertedBy);

                if (userMaster.Id == 0)
                {
                    param.Add("@UserId", userMaster.UserId);
                    param.Add("@EmailId", userMaster.EmailId);
                }
                try
                {
                    using (var multiResult = await SqlMapper.QueryMultipleAsync(_connectionFactory.GetConnection,
                    procedureName, param, commandType: CommandType.StoredProcedure).ConfigureAwait(false))
                    {
                        userMaster = await multiResult.ReadFirstOrDefaultAsync<UserMaster>().ConfigureAwait(false);

                    }

                }
                finally
                {
                    _connectionFactory.CloseConnection();
                }

                if (!string.IsNullOrEmpty(location.Location))
                {
                    var userAddress = new UserAddress();
                    var AddressExist = await context.UserAddress.Find(t => t.UserId == userMaster.UserId)
                        .SingleOrDefaultAsync().ConfigureAwait(false);
                    if (AddressExist != null)  //update
                    {
                        var update = Builders<UserAddress>.Update
                            .Set(x => x.Location, location.Location)
                            //.Set(x => x.InsertedBy, userMaster.InsertedBy.ToString())
                            .Set(x => x.Coordinates, location.Latitude > 0 && location.Longitude > 0 ? new GeoJsonPoint<GeoJson2DGeographicCoordinates>(new GeoJson2DGeographicCoordinates(location.Longitude, location.Latitude)) : AddressExist.Coordinates);
                        await context.UserAddress.UpdateOneAsync(t => t.AddressId == AddressExist.AddressId, update).ConfigureAwait(false);

                    }
                    else  // insert
                    {
                        userAddress.Coordinates = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(new GeoJson2DGeographicCoordinates(location.Longitude, location.Latitude));
                        userAddress.UserId = userMaster.UserId;
                        userAddress.Location = location.Location;
                        userAddress.InsertedBy = oldUserMaster.InsertedBy.ToString();
                        await context.UserAddress.InsertOneAsync(userAddress).ConfigureAwait(false);
                    }

                }

                return userMaster;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception");
                throw;
            }
        }


        public async Task<LoginResponse> GetLoginResponseAsync(string emailId, string ipAddress, UserMaster userModel, CancellationToken cancellationToken)
        {
            try
            {
                var loginResponse = new LoginResponse();
                List<UserMaster> UserResponse = new List<UserMaster>();
                if (userModel == null)
                {
                    UserResponse = await GetUserMasterAsync(0, "", emailId, 0, cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    UserResponse.Add(userModel);
                }




                if (UserResponse[0].Id > 0)
                {

                    var ProfilePicResult = await context.UserLocationMedia.Find(
                        Builders<UserLocationMedia>.Filter.Eq(d => d.UserId, UserResponse[0].UserId.ToString()) &
                        Builders<UserLocationMedia>.Filter.Eq(d => d.IsProfilePic, true)
                    ).ToListAsync().ConfigureAwait(false);


                    var tokenRequest = new TokenRequest();
                    tokenRequest.Email = UserResponse[0].EmailId;
                    tokenRequest.Roles.AddRange(UserResponse[0].RoleList);
                    tokenRequest.IpAddress = ipAddress;
                    tokenRequest.UserId = UserResponse[0].UserId;

                    var TokenResponse = await _tokenGenerator.GetTokenResponseAsync(tokenRequest).ConfigureAwait(false);
                    loginResponse.Id = UserResponse[0].Id;
                    loginResponse.UserId = UserResponse[0].UserId;
                    //loginResponse.Latitude = UserResponse[0].Latitude;
                    //loginResponse.Longitude = UserResponse[0].Longitude;
                    loginResponse.Message = _localizer["Success"];
                    loginResponse.ProfileImg = "";// UserResponse[0].ProfilePicture;
                    loginResponse.Rolenames = "";
                    loginResponse.Username = UserResponse[0].FirstName + " " + UserResponse[0].LastName;
                    loginResponse.AccessToken = TokenResponse.AccessToken;
                    loginResponse.RefreshToken = TokenResponse.RefreshToken;
                    loginResponse.TokenExpiry = TokenResponse.Expires;

                    if (ProfilePicResult.Count > 0)
                    {
                        loginResponse.ProfileImg = ProfilePicResult[0].MediaPath;
                    }

                }
                else
                {
                    loginResponse.Message = _localizer["Failed"];
                }

                return loginResponse;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception");
                throw;
            }
        }


        public async Task<List<PasswordHistory>> GetPasswordHistoryAsync(string Email)
        {
            try
            {
                var historyList = Enumerable.Empty<PasswordHistory>();

                var procedureName = "spGetPasswordHistory";
                var param = new DynamicParameters();
                param.Add("@Email", Email);

                try
                {
                    using (var multiResult = await SqlMapper.QueryMultipleAsync(_connectionFactory.GetConnection,
                        procedureName, param, commandType: CommandType.StoredProcedure).ConfigureAwait(false))
                    {
                        historyList = await multiResult.ReadAsync<PasswordHistory>().ConfigureAwait(false);
                    }

                }
                finally
                {
                    _connectionFactory.CloseConnection();
                }

                return historyList.ToList();

            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception");
                throw;
            }
        }

        public async Task<bool> InsertPasswordHistoryAsync(PasswordHistory passwordHistory)
        {
            try
            {
                if (passwordHistory == null) throw new ArgumentNullException(nameof(passwordHistory));
                bool IsSuccess = false;
                var procedureName = "spPasswordHistoryIns";
                var param = new DynamicParameters();
                param.Add("@HistoryId", passwordHistory.HistoryId);
                param.Add("@Email", passwordHistory.Email);
                param.Add("@HashedPassword", passwordHistory.HashedPassword);
                try
                {
                    var rowsAffected = await SqlMapper.ExecuteAsync(_connectionFactory.GetConnection,
                    procedureName, param, commandType: CommandType.StoredProcedure).ConfigureAwait(false);
                    if (rowsAffected > 0)
                    {
                        IsSuccess = true;
                    }

                }
                finally
                {
                    _connectionFactory.CloseConnection();
                }

                return IsSuccess;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception");
                throw;
            }

        }



    }
}


////to update roles
//ApplicationUser user = await _userManager.FindByEmailAsync(userMaster.EmailId).ConfigureAwait(false);

//var existingRoles = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
//var rolesToBeAdded = userMasterRoleList.Except(existingRoles).ToList();
//var rolesToBeDeleted = existingRoles.Except(userMasterRoleList).ToList();

//if (rolesToBeAdded.Count > 0 || rolesToBeDeleted.Count > 0)
//{
//    UpdateUserRoles(userMaster.UserId.ToString(), string.Join(",", rolesToBeDeleted), string.Join(",", rolesToBeAdded));
//}