using GraphQL.Types;
using PetizenApi.Interfaces;
using PetizenApi.Models;
using PetizenApi.Types;
using System.Collections.Generic;


namespace PetizenApi.Schemas
{

    public class QueryObject : ObjectGraphType
    {
        public QueryObject(
           IDogRepository dogRepository,
            IMenuRepository menuRepository, ILocationServiceRepository locationService, IChatRepository chatRepository, ITicketingRepository ticketingRepository,
            IAccountRepository accountRepository)
        {



            #region Account

            FieldAsync<ListGraphType<UserMasterType>>("getUserMaster",
                       arguments: new QueryArguments(new List<QueryArgument>
                       {
                new QueryArgument<IntGraphType>
                     {
                        Name = "id"
                     },
                   new QueryArgument<StringGraphType>
                     {
                        Name = "email"
                     },
                    new QueryArgument<StringGraphType>
                     {
                        Name = "userId"
                     },
                     new QueryArgument<IntGraphType>
                     {
                        Name = "userType"
                     },
                       }),
                       resolve: async context =>
                       {

                           var Id = context.GetArgument<int>("id");
                           var Email = context.GetArgument<string>("email");
                           var UserId = context.GetArgument<string>("userId");
                           var UserType = context.GetArgument<int>("userType");
                           return await accountRepository.GetUserMasterAsync(Id, UserId, Email, UserType, context.CancellationToken).ConfigureAwait(false);
                       }
                   );




            FieldAsync<ListGraphType<UserDetailsType>>("getUserDetails",
                      arguments: new QueryArguments(new List<QueryArgument>
                      {
                            new QueryArgument<StringGraphType>
                             {
                                Name = "userId"
                             },
                      }),
                      resolve: async context =>
                      {

                          var UserId = context.GetArgument<string>("userId");
                          return await accountRepository.GetUserDetailsAsync(UserId, context.SubFields, context.CancellationToken).ConfigureAwait(false);
                      }
                  );

            FieldAsync<ListGraphType<MyUserRoleType>>("getUserDetailsRolWise",
                     arguments: new QueryArguments(new List<QueryArgument>
                     {
                            new QueryArgument<StringGraphType>
                             {
                                Name = "userId"
                             },
                     }),
                     resolve: async context =>
                     {
                         var UserId = context.GetArgument<string>("userId");
                         return await accountRepository.GetUserDetailsRolWiseAsync(UserId, context.SubFields, context.CancellationToken).ConfigureAwait(false);
                     }
                 );



            FieldAsync<BooleanGraphType>("isEmailExist",
                      arguments: new QueryArguments(new List<QueryArgument>
                      {

                   new QueryArgument<StringGraphType>
                     {
                        Name = "email"
                     },
                      }),
                      resolve: async context =>
                      {
                          var Email = context.GetArgument<string>("email");
                          return await accountRepository.IsEmailExistAsync(Email, context.CancellationToken).ConfigureAwait(false);
                      }
                  );

            FieldAsync<ListGraphType<RoleMasterType>>("getUserRoles",
                  arguments: new QueryArguments(new List<QueryArgument>
                      {

                   new QueryArgument<IntGraphType>
                     {
                        Name = "roleType"
                     },
                      }),
                     resolve: async context =>
                     {
                         var RoleType = context.GetArgument<int>("roleType");
                         return await accountRepository.GetUserRolesAsync(RoleType, context.CancellationToken).ConfigureAwait(false);
                     }
                 );



            FieldAsync<ListGraphType<UserMasterType>>("searchUser",
                       arguments: new QueryArguments(new List<QueryArgument>
                       {
                            new QueryArgument<StringGraphType>
                            {
                            Name = "role"
                            },
                            new QueryArgument<StringGraphType>
                            {
                            Name = "latitude"
                            },
                            new QueryArgument<StringGraphType>
                            {
                            Name = "longitude"
                            },
                            new QueryArgument<IntGraphType>
                            {
                            Name = "distance"
                            },
                       }),
                       resolve: async context =>
                       {

                           var role = context.GetArgument<string>("role");
                           var latitude = context.GetArgument<string>("latitude");
                           var longitude = context.GetArgument<string>("longitude");
                           var distance = context.GetArgument<int>("distance");
                           var fields = context.SubFields;
                           return await locationService.SearchUserAsync(role, latitude, longitude, distance, fields, context.CancellationToken).ConfigureAwait(false);

                       }
                   );




            #endregion


            #region Dog module



            FieldAsync<ListGraphType<DogBreedsType>>("getDogBreed",
                  arguments: new QueryArguments(new List<QueryArgument>
                      {

                   new QueryArgument<StringGraphType>
                     {
                        Name = "breedId"
                     },
                     new QueryArgument<StringGraphType>
                     {
                        Name = "groupName"
                     },
                      new QueryArgument<StringGraphType>
                     {
                        Name = "breedName"
                     },
                      }),

                   resolve: async context =>
                   {
                       var breedId = context.GetArgument<string>("breedId");
                       var groupName = context.GetArgument<string>("groupName");
                       var breedName = context.GetArgument<string>("breedName");
                       return await dogRepository.GetDogBreedAsync(breedId, groupName, breedName, context.SubFields, context.CancellationToken).ConfigureAwait(false);
                   }
               );//.AuthorizeWith(AuthorizationPolicyName.Authenticate)


            FieldAsync<ListGraphType<DogMasterType>>("getDogs",
                arguments: new QueryArguments(new List<QueryArgument>
                    {

                   new QueryArgument<StringGraphType>
                     {
                        Name = "dogId"
                     },
                     new QueryArgument<StringGraphType>
                     {
                        Name = "breedId"
                     },
                      new QueryArgument<StringGraphType>
                     {
                        Name = "ownerId"
                     },
                         new QueryArgument<StringGraphType>
                     {
                        Name = "gender"
                     },
                    }),

                 resolve: async context =>
                 {

                     var dogId = context.GetArgument<string>("dogId");
                     var breedId = context.GetArgument<string>("breedId");
                     var ownerId = context.GetArgument<string>("ownerId");
                     var gender = context.GetArgument<string>("gender");
                     return await dogRepository.GetDogAsync(dogId, breedId, ownerId, gender, context.SubFields, context.CancellationToken).ConfigureAwait(false);
                 }
             );

            FieldAsync<ListGraphType<DogMediaType>>("getDogsMedia",
                arguments: new QueryArguments(new List<QueryArgument>
                    {

                   new QueryArgument<StringGraphType>
                     {
                        Name = "dogId"
                     },
                    new QueryArgument<StringGraphType>
                     {
                        Name = "mediaId"
                     },
                    }),

                 resolve: async context =>
                 {

                     var dogId = context.GetArgument<string>("dogId");
                     var mediaId = context.GetArgument<string>("mediaId");
                     return await dogRepository.GetDogsMediaAsync(mediaId, dogId, context.CancellationToken).ConfigureAwait(false);
                 }
             );

            FieldAsync<ListGraphType<DogCharacteristicsType>>("getDogCharacter",
               arguments: new QueryArguments(new List<QueryArgument>
                   {

                   new QueryArgument<StringGraphType>
                     {
                        Name = "characterId"
                     },
                    new QueryArgument<StringGraphType>
                     {
                        Name = "description"
                     },
                   }),

                resolve: async context =>
                {
                    var characterId = context.GetArgument<string>("characterId");
                    var description = context.GetArgument<string>("description");
                    return await dogRepository.GetDogCharacteristicsAsync(characterId, description, context.CancellationToken).ConfigureAwait(false);
                }
            );


            FieldAsync<ListGraphType<FavouriteDogType>>("getFavDogs",
             arguments: new QueryArguments(new List<QueryArgument>
                 {

                   new QueryArgument<StringGraphType>
                     {
                        Name = "userId"
                     },
                     new QueryArgument<StringGraphType>
                     {
                        Name = "forDogId"
                     },
                 }),

              resolve: async context =>
              {

                  var userId = context.GetArgument<string>("userId");
                  var forDogId = context.GetArgument<string>("forDogId");
                  return await dogRepository.GetFavouriteAsync(userId, forDogId, context.CancellationToken).ConfigureAwait(false);
              }
          );


            FieldAsync<ListGraphType<DogCoursesType>>("getDogCourses",
             arguments: new QueryArguments(new List<QueryArgument>
                 {
                    new QueryArgument<StringGraphType>
                     {
                        Name = "courseId"
                     },
                   new QueryArgument<StringGraphType>
                     {
                        Name = "userId"
                     },

                 }),

              resolve: async context =>
              {
                  var courseId = context.GetArgument<string>("courseId");
                  var userId = context.GetArgument<string>("userId");
                  var fields = context.SubFields;
                  return await dogRepository.GetDogCoursesAsync(courseId, userId, fields, context.CancellationToken).ConfigureAwait(false);
              }
          );


            FieldAsync<ListGraphType<DogMasterType>>("searchDog",
               arguments: new QueryArguments(new List<QueryArgument>
                   {


                     new QueryArgument<StringGraphType>
                     {
                        Name = "breedId"
                     },
                     new QueryArgument<StringGraphType>
                     {
                        Name = "gender"
                     },
                         new QueryArgument<StringGraphType>
                     {
                        Name = "latitude"
                     },
                         new QueryArgument<StringGraphType>
                     {
                        Name = "longitude"
                     },
                         new QueryArgument<IntGraphType>
                     {
                        Name = "distance"
                     },
                   }),

                resolve: async context =>
                {

                    var breedId = context.GetArgument<string>("breedId");
                    var gender = context.GetArgument<string>("gender");
                    var latitude = context.GetArgument<string>("latitude");
                    var longitude = context.GetArgument<string>("longitude");
                    var distance = context.GetArgument<int>("distance");
                    var fields = context.SubFields;
                    return await dogRepository.SearchDogAsync(breedId, gender, latitude, longitude, distance,
                        fields, context.CancellationToken).ConfigureAwait(false);

                }
            );


            #endregion




            #region Menu Master


            FieldAsync<ListGraphType<MenuMasterType>>("getMenuMaster",
             arguments: new QueryArguments(new List<QueryArgument>
                 {

                   new QueryArgument<StringGraphType>
                     {
                        Name = "menuId"
                     },
                     new QueryArgument<StringGraphType>
                     {
                        Name = "parentId"
                     },
                      new QueryArgument<IntGraphType>
                     {
                        Name = "menuType"
                     },
                 }),

              resolve: async context =>
              {
                  var menuMaster = new List<MenuMaster>();

                  var menuId = context.GetArgument<string>("menuId");
                  var parentId = context.GetArgument<string>("parentId");
                  var menuType = context.GetArgument<int>("menuType");


                  menuMaster = await menuRepository.GetMenuMastersAsync(menuId, parentId, menuType, context.SubFields, context.CancellationToken).ConfigureAwait(false);
                  return menuMaster;
              }
          );

            FieldAsync<ListGraphType<MenuAccessType>>("getMenuAccess",
             arguments: new QueryArguments(new List<QueryArgument>
                 {

                   new QueryArgument<StringGraphType>
                     {
                        Name = "accessId"
                     },
                     new QueryArgument<StringGraphType>
                     {
                        Name = "menuId"
                     },
                      new QueryArgument<IntGraphType>
                     {
                        Name = "roleId"
                     },
                 }),

              resolve: async context =>
              {
                  var menuAccesses = new List<MenuAccess>();

                  var accessId = context.GetArgument<string>("accessId");
                  var menuId = context.GetArgument<string>("menuId");
                  var roleId = context.GetArgument<string>("roleId");


                  menuAccesses = await menuRepository.GetMenuAccessAsync(accessId, menuId, roleId, context.CancellationToken).ConfigureAwait(false);//tolist()
                  return menuAccesses;
              }
          );



            FieldAsync<ListGraphType<GetRoleWiseMenuType>>("getRoleWiseMenus",
            arguments: new QueryArguments(new List<QueryArgument>
                {

                   new QueryArgument<StringGraphType>
                     {
                        Name = "parentId"
                     },

                }),

             resolve: async context =>
             {
                 var menus = new List<GetRoleWiseMenu>();
                 var parentId = context.GetArgument<string>("parentId");
                 return await menuRepository.GetRoleWiseMenuAsync(parentId, context.CancellationToken).ConfigureAwait(false);
             }
         );



            FieldAsync<ListGraphType<GetLoginWiseMenuType>>("getLoginWiseMenu",
          arguments: new QueryArguments(new List<QueryArgument>
              {

                   new QueryArgument<StringGraphType>
                     {
                        Name = "userId"
                     },

              }),

           resolve: async context =>
           {
               var menus = new List<GetLoginWiseMenu>();
               var userId = context.GetArgument<string>("userId");
               return await menuRepository.GetLoginWiseMenuAsync(userId, context.CancellationToken).ConfigureAwait(false);
           }
       );


            FieldAsync<ListGraphType<MenuMasterType>>("getSubMenusTest",
           arguments: new QueryArguments(new List<QueryArgument>
               {

                   new QueryArgument<StringGraphType>
                     {
                        Name = "parentId"
                     },

               }),

            resolve: async context =>
            {

                var parentId = context.GetArgument<string>("parentId");
                var fields = context.SubFields;

                return await menuRepository.GetSubMenusTestAsync(parentId, "", fields, context.CancellationToken).ConfigureAwait(false);
            }
        );


            #endregion


            #region Location Dog Sevices 
            //this.AuthorizeWith("Authorized");
            FieldAsync<ListGraphType<UserLocationType>>("getUserLocation",
            arguments: new QueryArguments(new List<QueryArgument>
                {

                   new QueryArgument<StringGraphType>
                     {
                        Name = "locationId"
                     },
                     new QueryArgument<StringGraphType>
                     {
                        Name = "userId"
                     },
                      new QueryArgument<StringGraphType>
                     {
                        Name = "roleId"
                     },
                        new QueryArgument<FloatGraphType>
                     {
                        Name = "lat"
                     },
                          new QueryArgument<FloatGraphType>
                     {
                        Name = "lng"
                     },
                }),

             resolve: async context =>
             {
                 var userLocations = new List<UserLocation>();

                 var locationId = context.GetArgument<string>("locationId");
                 var userId = context.GetArgument<string>("userId");
                 var roleId = context.GetArgument<string>("roleId");

                 userLocations = await locationService.GetUserLocationAsync(locationId, userId, roleId, context.CancellationToken).ConfigureAwait(false);
                 return userLocations;
             }
         );


            FieldAsync<ListGraphType<UserLocationMediaType>>("getUserLocationMedia",
           arguments: new QueryArguments(new List<QueryArgument>
               {

                new QueryArgument<StringGraphType>
                     {
                        Name = "mediaId"
                     },
                   new QueryArgument<StringGraphType>
                     {
                        Name = "locationId"
                     },
                     new QueryArgument<StringGraphType>
                     {
                        Name = "userId"
                     }

               }),

            resolve: async context =>
            {
                var userLocations = new List<UserLocationMedia>();

                var mediaId = context.GetArgument<string>("mediaId");
                var locationId = context.GetArgument<string>("locationId");
                var userId = context.GetArgument<string>("userId");

                userLocations = await locationService.GetUserLocationMediaAsync(mediaId, locationId, userId, context.CancellationToken).ConfigureAwait(false);
                return userLocations;
            }
        );



            FieldAsync<ListGraphType<DogServicesType>>("getDogServices",
           arguments: new QueryArguments(new List<QueryArgument>
               {

                   new QueryArgument<StringGraphType>
                     {
                        Name = "serviceId"
                     },
                     new QueryArgument<StringGraphType>
                     {
                        Name = "locationId"
                     },new QueryArgument<StringGraphType>
                     {
                        Name = "userId"
                     },
                      new QueryArgument<StringGraphType>
                     {
                        Name = "serviceName"
                     },
               }),

            resolve: async context =>
            {
                var dogServices = new List<DogServices>();

                var serviceId = context.GetArgument<string>("serviceId");
                var locationId = context.GetArgument<string>("locationId");
                var userId = context.GetArgument<string>("userId");
                var serviceName = context.GetArgument<string>("serviceName");


                dogServices = await locationService.GetDogServicesAsync(serviceId, locationId, userId, serviceName, context.SubFields, context.CancellationToken).ConfigureAwait(false);
                return dogServices;
            }
        );

            #endregion


            #region Ticketing


            FieldAsync<ListGraphType<TicketingType>>("getTicketing",
           arguments: new QueryArguments(new List<QueryArgument>
               {
                     new QueryArgument<StringGraphType>
                     {
                        Name = "locationId"
                     },
                      new QueryArgument<StringGraphType>
                     {
                        Name = "tId"
                     },
                       new QueryArgument<IntGraphType>
                     {
                        Name = "day"
                     }
               }),

            resolve: async context =>
            {
                var ticketing = new List<Ticketing>();

                var locationId = context.GetArgument<string>("locationId");
                var tId = context.GetArgument<string>("tId");
                var day = context.GetArgument<int>("day");


                ticketing = await ticketingRepository.GetTicketingAsync(tId, locationId, day, context.SubFields, context.CancellationToken).ConfigureAwait(false);
                return ticketing;
            }
        );


            #endregion


            #region User Chat

            //Private Chat
            //Task<IList<UserConversation>> GetMyConvoList(string UserId, IDictionary<string, Field> fields);//--order by desc
            FieldAsync<ListGraphType<UserConversationType>>("getMyConvoList",
            arguments: new QueryArguments(new List<QueryArgument>
                {

                     new QueryArgument<StringGraphType>
                     {
                        Name = "userId"
                     },
                }),

             resolve: async context =>
             {
                 var userConversation = new List<UserConversation>();

                 var userId = context.GetArgument<string>("userId");
                 var fields = context.SubFields;

                 userConversation = await chatRepository.GetMyConvoListAsync(userId, fields, context.CancellationToken).ConfigureAwait(false);
                 return userConversation;
             }
         );

            //Task<long> GetUnreadMessageCount(string UserId);
            FieldAsync<IntGraphType>("getUnreadMessageCount",
           arguments: new QueryArguments(new List<QueryArgument>
               {
                     new QueryArgument<StringGraphType>
                     {
                        Name = "userId"
                     }
               }),

            resolve: async context =>
            {
                var userId = context.GetArgument<string>("userId");
                long unreadCount = await chatRepository.GetUnreadMessageCountAsync(userId, context.CancellationToken).ConfigureAwait(false);
                return unreadCount;
            }
        );



            // Task<IList<ConversationMessage>> GetAllMessages(string ConvoId, string UserId);
            FieldAsync<ListGraphType<ConversationMessageType>>("getAllMessages",
           arguments: new QueryArguments(new List<QueryArgument>
               {
                   new QueryArgument<StringGraphType>
                     {
                        Name = "convoId"
                     },
                     new QueryArgument<StringGraphType>
                     {
                        Name = "userId"
                     }
               }),

            resolve: async context =>
            {
                var conversationMessage = new List<ConversationMessage>();

                var convoId = context.GetArgument<string>("convoId");
                var userId = context.GetArgument<string>("userId");

                conversationMessage = await chatRepository.GetAllMessagesAsync(convoId, userId, context.CancellationToken).ConfigureAwait(false);
                return conversationMessage;
            }
        );

            ////GroupChat

            //Task<IList<UserGroups>> GetMyGroupList(string UserId, Field Fields);//--order by desc
            FieldAsync<ListGraphType<UserGroupsType>>("getMyGroupList",
          arguments: new QueryArguments(new List<QueryArgument>
              {
                    new QueryArgument<StringGraphType>
                    {
                    Name = "userId"
                    }
              }),

           resolve: async context =>
           {
               var userGroups = new List<UserGroups>();

               var userId = context.GetArgument<string>("userId");
               var fields = context.SubFields;


               userGroups = await chatRepository.GetMyGroupListAsync(userId, fields, context.CancellationToken).ConfigureAwait(false);
               return userGroups;
           }
       );

            //Task<UserGroups> GetGroupInfo(string GroupId, Field Fields);//--order by desc
            FieldAsync<UserGroupsType>("getGroupInfo",
          arguments: new QueryArguments(new List<QueryArgument>
              {

                     new QueryArgument<StringGraphType>
                     {
                        Name = "groupId"
                     }
              }),

           resolve: async context =>
           {
               var UserGroups = new UserGroups();

               var groupId = context.GetArgument<string>("groupId");
               var fields = context.SubFields;

               UserGroups = await chatRepository.GetGroupInfoAsync(groupId, fields, context.CancellationToken).ConfigureAwait(false);
               return UserGroups;
           }
       );

            //Task<IList<GroupMessages>> GetAllGroupMessage(string UserId, string GroupId, Field Fields);//--order by desc
            FieldAsync<ListGraphType<GroupMessagesType>>("getAllGroupMessage",
          arguments: new QueryArguments(new List<QueryArgument>
              {

                   new QueryArgument<StringGraphType>
                     {
                        Name = "userId"
                     },
                     new QueryArgument<StringGraphType>
                     {
                        Name = "groupId"
                     }

              }),

           resolve: async context =>
           {
               var groupMessages = new List<GroupMessages>();

               var userId = context.GetArgument<string>("userId");
               var groupId = context.GetArgument<string>("groupId");

               var fields = context.SubFields;
               groupMessages = await chatRepository.GetAllGroupMessageAsync(userId, groupId, fields, context.CancellationToken).ConfigureAwait(false);
               return groupMessages;
           }
       );




            #endregion




        }


    }
}
