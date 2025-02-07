﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PetizenApi.Resources.Repositories {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class AccountRepository {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal AccountRepository() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("PetizenApi.Resources.Repositories.AccountRepository", typeof(AccountRepository).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to User account locked out..
        /// </summary>
        public static string AccountLocked {
            get {
                return ResourceManager.GetString("AccountLocked", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Email confirmed successfully...
        /// </summary>
        public static string EmailConfirmed {
            get {
                return ResourceManager.GetString("EmailConfirmed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Your email Id is not Confirmed..
        /// </summary>
        public static string EmailNotConfirmed {
            get {
                return ResourceManager.GetString("EmailNotConfirmed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed.
        /// </summary>
        public static string Failed {
            get {
                return ResourceManager.GetString("Failed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid login attempt..
        /// </summary>
        public static string InvalidLogin {
            get {
                return ResourceManager.GetString("InvalidLogin", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Account does not exist kindly register.
        /// </summary>
        public static string NewAccount {
            get {
                return ResourceManager.GetString("NewAccount", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Not a Registered User.
        /// </summary>
        public static string NotRegistered {
            get {
                return ResourceManager.GetString("NotRegistered", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to User not exist with the provided email..
        /// </summary>
        public static string NoUser {
            get {
                return ResourceManager.GetString("NoUser", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Password changed successfully...
        /// </summary>
        public static string PasswordChanged {
            get {
                return ResourceManager.GetString("PasswordChanged", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed to change password...
        /// </summary>
        public static string PasswordChangeFailed {
            get {
                return ResourceManager.GetString("PasswordChangeFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed to reset password...
        /// </summary>
        public static string PasswordResetFailed {
            get {
                return ResourceManager.GetString("PasswordResetFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Password reset successfully...
        /// </summary>
        public static string PasswordResetSuccess {
            get {
                return ResourceManager.GetString("PasswordResetSuccess", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Instructions to reset password mail has sent.
        /// </summary>
        public static string ResetMailSent {
            get {
                return ResourceManager.GetString("ResetMailSent", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Success.
        /// </summary>
        public static string Success {
            get {
                return ResourceManager.GetString("Success", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid token...
        /// </summary>
        public static string Token {
            get {
                return ResourceManager.GetString("Token", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Require two factor.
        /// </summary>
        public static string TwoFactor {
            get {
                return ResourceManager.GetString("TwoFactor", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unauthorized.
        /// </summary>
        public static string Unauthorized {
            get {
                return ResourceManager.GetString("Unauthorized", resourceCulture);
            }
        }
    }
}
