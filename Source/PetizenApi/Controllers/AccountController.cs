using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using PetizenApi.Interfaces;
using PetizenApi.Models;
using PetizenApi.Providers;
using Serilog;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace PetizenApi.Controllers
{
    [Produces("application/json")]
    [Route("api")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountRepository _accountRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IStringLocalizer<AccountController> _localizer;
        private readonly IEmailRepository _emailRepository;
        private readonly ApplicationUrl _appurl;

        public AccountController([FromServices] UserManager<ApplicationUser> userManager, [FromServices] SignInManager<ApplicationUser> signInManager,
            IAccountRepository accountRepository, IStringLocalizer<AccountController> localizer, IEmailRepository emailRepository, IOptions<ApplicationUrl> appurl)
        {
            _accountRepository = accountRepository;
            _userManager = userManager;
            _signInManager = signInManager;
            _localizer = localizer;
            _emailRepository = emailRepository;
            if (appurl == null) throw new ArgumentNullException(nameof(appurl));
            _appurl = appurl.Value;
        }

        [Route("login")]
        [HttpPost]
        public async Task<object> LoginAsync([FromBody] LoginViewModel loginView, CancellationToken cancellationToken)
        {
            //var response= await _accountRepository.LoginAsync(loginView, cancellationToken).ConfigureAwait(false);

            try
            {
                var loginResponse = new LoginResponse();

                if (loginView == null) throw new ArgumentNullException(nameof(loginView));

                var result = await _signInManager.PasswordSignInAsync(loginView.Email, loginView.Password, loginView.RememberMe,
                    lockoutOnFailure: false).ConfigureAwait(false);


                if (result.Succeeded)
                {
                    loginResponse = await _accountRepository.GetLoginResponseAsync(loginView.Email, loginView.IpAddress, null, cancellationToken).ConfigureAwait(false);
                }
                else if (result.RequiresTwoFactor)
                {
                    loginResponse.Message = _localizer["TwoFactor"];

                }
                else if (result.IsLockedOut)
                {
                    loginResponse.Message = _localizer["AccountLocked"];

                }
                else
                {

                    var user = await _userManager.FindByEmailAsync(loginView.Email).ConfigureAwait(false);
                    if (user != null)
                    {
                        if (!await _userManager.IsEmailConfirmedAsync(user).ConfigureAwait(false))
                        {
                            loginResponse.Message = _localizer["EmailNotConfirmed"];
                        }
                        else
                        {
                            loginResponse.Message = _localizer["InvalidLogin"];
                        }
                    }
                    else
                    {
                        loginResponse.Message = _localizer["NoUser"];
                    }

                }

                if (!string.IsNullOrEmpty(loginResponse.Message))
                {
                    return Ok(loginResponse);
                }
                else
                {
                    return StatusCode(204);
                }

            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception");
                throw;
            }

        }




        //for user who is registering directly
        //RegisterType=1
        [Route("registeruser")]
        [HttpPost]
        public async Task<object> RegisterUserAsync([FromBody] UserMaster userMaster, CancellationToken cancellationToken)
        {
            //var response= await _accountRepository.RegisterUserAsync(userMaster, cancellationToken).ConfigureAwait(false);
            try
            {
                if (userMaster == null) throw new ArgumentNullException(nameof(userMaster));
                var loginResponse = new LoginResponse();

                var user = new ApplicationUser { UserName = userMaster.EmailId, Email = userMaster.EmailId };
                var result = await _userManager.CreateAsync(user, userMaster.Password).ConfigureAwait(false);

                if (result.Succeeded)
                {
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user).ConfigureAwait(false);
                    await _userManager.AddToRoleAsync(user, userMaster.RoleList[0]).ConfigureAwait(false);
                    Guid UserId = user.Id;

                    userMaster.UserId = UserId;

                    // var UserRegister = await InsertUpdateUserMasterAsync(userMaster, cancellationToken).ConfigureAwait(false);
                    userMaster.InsertedBy = !userMaster.fromBackend ? UserId : userMaster.InsertedBy;

                    var UserRegister = await _accountRepository.InsertUpdateUserMasterAsync(userMaster, cancellationToken).ConfigureAwait(false);
                    UserRegister.RoleList.Add(userMaster.RoleList[0]);//only 1 role while register

                    if (UserRegister.Id > 0)
                    {

                        loginResponse = await _accountRepository.GetLoginResponseAsync(userMaster.EmailId, userMaster.IpAddress, UserRegister, cancellationToken).ConfigureAwait(false);

                        code = code.Replace("/", "~", StringComparison.CurrentCulture);
                        var Link = "";
                        if (!userMaster.fromBackend)  //registration from front-end solution
                        {
                            Link = _appurl.WebUrl + "confirmemail/" + code + "/" + UserRegister.EmailId;
                            Uri ConfirmUrl = new Uri(Link, UriKind.Absolute);
                            await _emailRepository.ConfirmatonEmailAsync(UserRegister.FirstName + " " + UserRegister.LastName, UserRegister.EmailId, ConfirmUrl.AbsoluteUri).ConfigureAwait(false);
                        }
                        else
                        {
                            if (userMaster.isFrontUser)
                            {
                                Link = _appurl.WebUrl + "confirmemail/" + code + "/" + UserRegister.EmailId;
                            }
                            else
                            {
                                Link = _appurl.BackOfficeUrl + "/accounts/confirmemail/" + UserRegister.EmailId + "/" + HttpUtility.UrlEncode(code);
                            }
                            Uri ConfirmUrl = new Uri(Link, UriKind.Absolute);
                            await _emailRepository.ConfirmEmailFromBackOfficeAsync(UserRegister.FirstName + " " + UserRegister.LastName, UserRegister.EmailId, userMaster.Password,
                                ConfirmUrl.AbsoluteUri).ConfigureAwait(false);
                        }

                    }
                    else
                    {
                        loginResponse.Message = _localizer["Failed"];
                    }

                }

                if (!string.IsNullOrEmpty(loginResponse.Message))
                {
                    return Ok(loginResponse);
                }
                else
                {
                    loginResponse.Message = _localizer["Failed"];
                    return Ok(loginResponse);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception");
                throw;
            }

        }

        [Route("updateuserprofile")]
        [HttpPost]
        public async Task<object> UpdateUserProfileAsync([FromBody] UserMaster userMaster, CancellationToken cancellationToken)
        {
            try
            {
                if (userMaster == null) throw new ArgumentNullException(nameof(userMaster));

                var userMasterRoleList = userMaster.RoleList;
                var response = await _accountRepository.InsertUpdateUserMasterAsync(userMaster, cancellationToken).ConfigureAwait(false);
                response.RoleList.AddRange(userMaster.RoleList);
                //to update roles
                ApplicationUser user = await _userManager.FindByEmailAsync(userMaster.EmailId).ConfigureAwait(false);

                var existingRoles = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
                var rolesToBeAdded = userMasterRoleList.Except(existingRoles).ToList();
                var rolesToBeDeleted = existingRoles.Except(userMasterRoleList).ToList();

                if (rolesToBeAdded.Count > 0 || rolesToBeDeleted.Count > 0)
                {
                    _accountRepository.UpdateUserRoles(response.UserId.ToString(), string.Join(",", rolesToBeDeleted), string.Join(",", rolesToBeAdded));
                    //userMaster.RoleList.AddRange(userMasterRoleList);

                }

                if (response.Id > 0)
                {
                    return Ok(response);
                }
                else
                {
                    return StatusCode(204);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception");
                throw;
            }
        }

        [Route("externallogin")]
        [HttpPost]
        public async Task<object> ExternalLoginAsync([FromBody] ExternalLogin externalLogin, CancellationToken cancellationToken)
        {
            try
            {
                //var response= await _accountRepository.ExternalLoginAsync(externalLogin, cancellationToken).ConfigureAwait(false);

                if (externalLogin == null) throw new ArgumentNullException(nameof(externalLogin));

                var loginResponse = new LoginResponse();
                // Sign in the user with this external login provider if the user already has a login.
                var result = await _signInManager.ExternalLoginSignInAsync(externalLogin.LoginProvider, externalLogin.ProviderKey, isPersistent: externalLogin.IsPersistent,
                    bypassTwoFactor: externalLogin.BypassTwoFactor).ConfigureAwait(false);
                if (result.Succeeded)
                {
                    loginResponse = await _accountRepository.GetLoginResponseAsync(externalLogin.Email, externalLogin.IpAddress, null, cancellationToken).ConfigureAwait(false);
                }
                else if (result.RequiresTwoFactor)
                {
                    loginResponse.Message = _localizer["TwoFactor"];

                }
                else if (result.IsLockedOut)
                {
                    loginResponse.Message = _localizer["AccountLocked"];

                }
                else
                {
                    loginResponse.Message = _localizer["NewAccount"];
                }

                if (!string.IsNullOrEmpty(loginResponse.Message))
                {
                    return Ok(loginResponse);
                }
                else
                {
                    return StatusCode(204);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception");
                throw;
            }
        }

        //RegisterType=2
        [Route("externalregister")]
        [HttpPost]
        public async Task<object> ExternalRegisterAsync([FromBody] UserMaster userMaster, CancellationToken cancellationToken)
        {
            try
            {
                //var response= await _accountRepository.ExternalRegisterAsync(externalRegister, cancellationToken).ConfigureAwait(false);


                if (userMaster == null) throw new ArgumentNullException(nameof(userMaster));

                var loginResponse = new LoginResponse();
                var user = new ApplicationUser { UserName = userMaster.EmailId, Email = userMaster.EmailId };
                var usercreate = await _userManager.CreateAsync(user).ConfigureAwait(false);

                if (usercreate.Succeeded)
                {
                    var logininfo = new UserLoginInfo(userMaster.LoginProvider, userMaster.ProviderKey,
                        userMaster.DisplayName);
                    usercreate = await _userManager.AddLoginAsync(user, logininfo).ConfigureAwait(false);

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user).ConfigureAwait(false);
                    await _userManager.ConfirmEmailAsync(user, code).ConfigureAwait(false);
                    await _userManager.AddToRoleAsync(user, userMaster.RoleList[0]).ConfigureAwait(false);

                    await _signInManager.SignInAsync(user, isPersistent: false).ConfigureAwait(false);
                    Guid UserId = user.Id;

                    userMaster.UserId = UserId;
                    userMaster.InsertedBy = !userMaster.fromBackend ? UserId : userMaster.InsertedBy;
                    var UserRegister = await _accountRepository.InsertUpdateUserMasterAsync(userMaster, cancellationToken).ConfigureAwait(false);
                    UserRegister.RoleList.Add(userMaster.RoleList[0]);//only 1 role while register


                    if (UserRegister.Id > 0)
                    {
                        loginResponse = await _accountRepository.GetLoginResponseAsync(userMaster.EmailId, userMaster.IpAddress, UserRegister, cancellationToken).ConfigureAwait(false);
                    }
                    else
                    {
                        loginResponse.Message = _localizer["Failed"];
                    }
                }




                if (!string.IsNullOrEmpty(loginResponse.Message))
                {
                    return Ok(loginResponse);
                }
                else
                {
                    return StatusCode(204);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception");
                throw;
            }
        }

        [Route("confirmemail")]
        [HttpPost]
        public async Task<object> ConfirmEmailAsync([FromBody] ConfirmEmailViewModel confirmEmail)
        {
            try
            {
                //var response = await _accountRepository.ConfirmEmailAsync(Email, Token, cancellationToken).ConfigureAwait(false);

                var response = new SuccessResponse();

                if (confirmEmail == null)
                {
                    throw new ArgumentNullException(nameof(confirmEmail));
                }

                //var user = new ApplicationUser { UserName = Email, Email = Email };
                var User = await _userManager.FindByEmailAsync(confirmEmail.Email).ConfigureAwait(false);
                if (User == null)
                {
                    response.Message = _localizer["NoUser"];

                    response.isSuccess = false;
                }
                else
                {
                    if (!string.IsNullOrEmpty(confirmEmail.Token))
                    {
                        try
                        {
                            confirmEmail.Token = confirmEmail.Token.Replace("~", "/", StringComparison.CurrentCulture);

                            IdentityResult result = await _userManager.ConfirmEmailAsync(User, confirmEmail.Token).ConfigureAwait(false);

                            if (result.Succeeded)
                            {
                                response.Message = _localizer["EmailConfirmed"];
                                response.isSuccess = true;
                            }
                            else
                            {
                                response.Message = _localizer["Token"];
                                response.isSuccess = false;
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex, "Exception");
                            throw;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(response.Message))
                {
                    return Ok(response);
                }
                else
                {
                    return StatusCode(204);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception");
                throw;
            }
        }

        [Route("forgotpassword")]
        [HttpPost]
        public async Task<object> ForgotPasswordAsync(string Email, CancellationToken cancellationToken)
        {
            try
            {
                // var response = await _accountRepository.ForgotPasswordAsync(Email, cancellationToken).ConfigureAwait(false);

                var response = new SuccessResponse();
                ApplicationUser user = await _userManager.FindByEmailAsync(Email).ConfigureAwait(false);

                if (user == null)
                {
                    response.Message = _localizer["NotRegistered"];
                    response.isSuccess = false;
                    //return response;
                }
                else if (!(await _userManager.IsEmailConfirmedAsync(user).ConfigureAwait(false)))
                {
                    response.Message = _localizer["Unauthorized"];
                    response.isSuccess = false;
                    //return response;
                }
                else
                {

                    var code = await _userManager.GeneratePasswordResetTokenAsync(user).ConfigureAwait(false);
                    code = code.Replace("/", "~", StringComparison.CurrentCulture);
                    var UserDetails = await _accountRepository.GetUserMasterAsync(0, "", Email, 0, cancellationToken).ConfigureAwait(false);
                    var UserName = UserDetails[0].FirstName + " " + UserDetails[0].LastName;
                    var Link = "";

                    if (UserDetails[0].isFrontUser)
                    {
                        Link = _appurl.WebUrl + "resetpassword/" + user.Id + "/" + code;
                    }
                    else
                    {
                        Link = _appurl.BackOfficeUrl + "/accounts/resetpw/" + user.Id + "/" + code;
                    }

                    Uri ConfirmUrl = new Uri(Link, UriKind.Absolute);

                    try
                    {
                        await _emailRepository.ResetPasswordEmailAsync(UserName, Email, ConfirmUrl.AbsoluteUri).ConfigureAwait(false);

                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Exception");
                        throw;
                    }
                    response.Message = _localizer["ResetMailSent"];

                    response.isSuccess = true;

                    //return response;

                }


                if (!string.IsNullOrEmpty(response.Message))
                {
                    return Ok(response);
                }
                else
                {
                    return StatusCode(204);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception");
                throw;
            }
        }

        [Route("resetpassword")]
        [HttpPost]
        public async Task<object> ResetPasswordAsync([FromBody] ResetPasswordViewModel resetPassword)
        {
            try
            {
                //var response = await _accountRepository.ResetPasswordAsync(resetPassword, cancellationToken).ConfigureAwait(false);

                if (resetPassword == null) throw new ArgumentNullException(nameof(resetPassword));
                var response = new SuccessResponse();

                var user = await _userManager.FindByIdAsync(resetPassword.UserId).ConfigureAwait(false);

                if (user == null)
                {
                    response.Message = _localizer["NoUser"];
                    response.isSuccess = false;
                }
                else
                {
                    var passwordHistory = await _accountRepository.GetPasswordHistoryAsync(user.Email).ConfigureAwait(false);
                    bool isMatchedWithHistory = false;
                    if (passwordHistory.Any())
                    {
                        foreach (var item in passwordHistory)
                        {
                            PasswordVerificationResult passresult = _userManager.PasswordHasher.VerifyHashedPassword(user, item.HashedPassword, resetPassword.Password);
                            if (passresult.ToString() == "Success")
                            {
                                isMatchedWithHistory = true;
                                break;
                            }

                        }
                    }

                    if (!isMatchedWithHistory)
                    {

                        resetPassword.Code = resetPassword.Code.Replace("~", "/", StringComparison.CurrentCulture);

                        var result = await _userManager.ResetPasswordAsync(user, resetPassword.Code, resetPassword.Password).ConfigureAwait(false);
                        if (result.Succeeded)
                        {
                            PasswordHistory history = new PasswordHistory();
                            history.Email = user.Email;
                            string hasPassword = _userManager.PasswordHasher.HashPassword(user, resetPassword.Password);
                            history.HashedPassword = hasPassword;
                            await _accountRepository.InsertPasswordHistoryAsync(history).ConfigureAwait(false);

                            response.Message = _localizer["PasswordResetSuccess"];
                            response.isSuccess = true;
                        }
                        else
                        {
                            response.Message = _localizer["PasswordResetFailed"];
                            response.isSuccess = false;

                        }

                    }
                    else
                    {
                        response.Message = _localizer["PasswordResetSameLast3"];
                        response.isSuccess = false;
                    }

                }

                if (!string.IsNullOrEmpty(response.Message))
                {
                    return Ok(response);
                }
                else
                {
                    return StatusCode(204);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception");
                throw;
            }
        }

        [Route("managepassword")]
        [HttpPost]
        public async Task<object> ManagePasswordAsync([FromBody] ManagePasswordViewModel managePassword)
        {
            try
            {
                //var response = await _accountRepository.ManagePasswordAsync(managePassword, cancellationToken).ConfigureAwait(false);

                if (managePassword == null) throw new ArgumentNullException(nameof(managePassword));

                var response = new SuccessResponse();

                var user = await _userManager.FindByIdAsync(managePassword.UserId).ConfigureAwait(false);

                if (user == null)
                {
                    response.Message = _localizer["NoUser"];
                    response.isSuccess = false;
                }
                else
                {
                    var passwordHistory = await _accountRepository.GetPasswordHistoryAsync(user.Email).ConfigureAwait(false);
                    bool isMatchedWithHistory = false;
                    if (passwordHistory.Any())
                    {
                        foreach (var item in passwordHistory)
                        {
                            PasswordVerificationResult passresult = _userManager.PasswordHasher.VerifyHashedPassword(user, item.HashedPassword, managePassword.NewPassword);
                            if (passresult.ToString() == "Success")
                            {
                                isMatchedWithHistory = true;
                                break;
                            }

                        }
                    }
                    if (!isMatchedWithHistory)
                    {
                        //var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                        var result = await _userManager.ChangePasswordAsync(user, managePassword.OldPassword, managePassword.NewPassword).ConfigureAwait(false);

                        if (result.Succeeded)
                        {
                            PasswordHistory history = new PasswordHistory();
                            history.Email = user.Email;
                            string hasPassword = _userManager.PasswordHasher.HashPassword(user, managePassword.NewPassword);
                            history.HashedPassword = hasPassword;
                            await _accountRepository.InsertPasswordHistoryAsync(history).ConfigureAwait(false);

                            response.Message = _localizer["PasswordChanged"];
                            response.isSuccess = true;
                        }
                        else
                        {
                            response.Message = _localizer["PasswordChangeFailed"];
                            response.isSuccess = false;

                        }
                    }
                    else
                    {
                        response.Message = _localizer["PasswordResetSameLast3"];
                        response.isSuccess = false;
                    }


                }

                if (!string.IsNullOrEmpty(response.Message))
                {
                    return Ok(response);
                }
                else
                {
                    return StatusCode(204);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception");
                throw;
            }
        }


    }
}