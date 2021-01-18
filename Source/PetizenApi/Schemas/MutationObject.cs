namespace PetizenApi.Schemas
{
    using GraphQL.Types;
    using Newtonsoft.Json;
    using PetizenApi.Interfaces;
    using PetizenApi.Models;
    using PetizenApi.Types;
    using PetizenApi.Types.Inputs;
    using System.Collections.Generic;

    /// <summary>
    /// All mutations defined in the schema used to modify data.
    /// </summary>

    public class MutationObject : ObjectGraphType<object>
    {
        //IClockService clockService,
        public MutationObject(IAccountRepository accountRepository, IDogRepository dogRepository,
             IMenuRepository menuRepository, ILocationServiceRepository locationService,//UploadRepository uploads,
            IChatRepository chatRepository, ITicketingRepository ticketingRepository)//ChatServices chatServices,
        {
            this.Name = "Mutation";
            this.Description = "The mutation type, represents all updates we can make to our data.";

            #region Account

            FieldAsync<BooleanGraphType>(
            "deleteUser",
            arguments: new QueryArguments(new QueryArgument<NonNullGraphType<IntGraphType>> { Name = "id" }),
            resolve: async context =>
            {
                var Id = context.GetArgument<int>("id");
                return await accountRepository.DeleteUserAsync(Id, context.CancellationToken).ConfigureAwait(false);
            }
            );

            #endregion




            //#region fileupload

            ////to store files

            //Field<FileGraphType>(
            //  "singleUpload",
            //  arguments: new QueryArguments(
            //      new QueryArgument<UploadGraphType> { Name = "file" }),
            //  resolve: context =>
            //  {
            //      var file = context.GetArgument<IFormFile>("file");
            //      return uploads.UploadFilesAsync(file);
            //  });

            //Field<ListGraphType<FileGraphType>>(
            //    "multipleUpload",
            //    arguments: new QueryArguments(
            //        new QueryArgument<ListGraphType<UploadGraphType>> { Name = "files" }),
            //    resolve: context =>
            //    {
            //        var files = context.GetArgument<IEnumerable<IFormFile>>("files");
            //        return Task.WhenAll(files.Select(file => uploads.UploadFilesAsync(file)));
            //    });

            //#endregion








            #region Dog module

            FieldAsync<DogBreedsType>(
"insUpdDogBreed",
arguments: new QueryArguments(new QueryArgument<NonNullGraphType<DogBreedsInput>> { Name = "dogBreed" }),
resolve: async context =>
{
    var dogBreeds = context.GetArgument<dynamic>("dogBreed");
    DogBreeds breeds = new DogBreeds();
    if (!string.IsNullOrEmpty(dogBreeds["breedId"]))
    {
        var BreedDb = await dogRepository.GetDogBreedAsync(dogBreeds["breedId"], "", "", null, context.CancellationToken);
        var json = JsonConvert.SerializeObject(dogBreeds);
        JsonConvert.PopulateObject(json, BreedDb[0]);
        breeds = BreedDb[0];
    }
    else
    {
        breeds = context.GetArgument<DogBreeds>("dogBreed");
    }
    return await dogRepository.InsUpdDogBreedAsync(breeds, context.CancellationToken).ConfigureAwait(false);
}
);


            FieldAsync<BooleanGraphType>(
"deleteDogBreed",
arguments: new QueryArguments(new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "breedId" }),
resolve: async context =>
{
    var breedId = context.GetArgument<string>("breedId");
    return await dogRepository.DeleteDogBreedAsync(breedId, context.CancellationToken).ConfigureAwait(false);
}
);


            FieldAsync<DogMasterType>(
"insUpdDog",
arguments: new QueryArguments(new QueryArgument<NonNullGraphType<DogMasterInput>> { Name = "dog" }),
resolve: async context =>
{
    var dogMaster = context.GetArgument<DogMaster>("dog");
    return await dogRepository.InsUpdDogAsync(dogMaster, context.CancellationToken).ConfigureAwait(false);
}
);


            FieldAsync<BooleanGraphType>(
            "deleteDog",
            arguments: new QueryArguments(new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "dogId" }),
            resolve: async context =>
            {
                var dogId = context.GetArgument<string>("dogId");
                return await dogRepository.DeleteDogAsync(dogId, context.CancellationToken).ConfigureAwait(false);
            }
            );

            FieldAsync<DogMediaType>(
            "insUpdDogMedia",
            arguments: new QueryArguments(new QueryArgument<NonNullGraphType<DogMediaInput>> { Name = "dogMedia" }),
            resolve: async context =>
            {
                var dogMedia = context.GetArgument<DogMedia>("dogMedia");
                return await dogRepository.InsUpdDogMediaAsync(dogMedia, context.CancellationToken).ConfigureAwait(false);
            }
            );


            FieldAsync<BooleanGraphType>(
            "deleteDogMedia",
            arguments: new QueryArguments(new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "mediaId" }),
            resolve: async context =>
            {
                var mediaId = context.GetArgument<string>("mediaId");
                return await dogRepository.DeleteDogMediaAsync(mediaId, context.CancellationToken).ConfigureAwait(false);
            }
            );



            FieldAsync<DogCharacteristicsType>(
            "insUpdDogCharacter",
            arguments: new QueryArguments(new QueryArgument<NonNullGraphType<DogCharacteristicsInput>> { Name = "dogCharacter" }),
            resolve: async context =>
            {
                var dogCharacteristics = context.GetArgument<DogCharacteristics>("dogCharacter");
                return await dogRepository.InsUpdDogCharacteristicsAsync(dogCharacteristics, context.CancellationToken).ConfigureAwait(false);
            }
            );


            Field<BooleanGraphType>(
            "deleteDogCharacter",
            arguments: new QueryArguments(new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "characterId" }),
            resolve: context =>
            {
                var characterId = context.GetArgument<string>("characterId");
                return dogRepository.DeleteDogCharacteristicsAsync(characterId, context.CancellationToken).ConfigureAwait(false);
            }
            );


            FieldAsync<FavouriteDogType>(
            "insUpdFavDog",
            arguments: new QueryArguments(new QueryArgument<NonNullGraphType<FavouriteDogInput>> { Name = "favDog" }),
            resolve: async context =>
            {
                var favouriteDog = context.GetArgument<FavouriteDog>("favDog");
                return await dogRepository.InsUpdFavDogAsync(favouriteDog, context.CancellationToken).ConfigureAwait(false);
            }
            );


            FieldAsync<BooleanGraphType>(
            "deleteFavDog",
            arguments: new QueryArguments(new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "favId" }),
            resolve: async context =>
            {
                var favId = context.GetArgument<string>("favId");

                return await dogRepository.DeleteFavDogAsync(favId, context.CancellationToken).ConfigureAwait(false);
            }
            );


            FieldAsync<DogCoursesType>(
           "insUpdDogCourses",
           arguments: new QueryArguments(new QueryArgument<NonNullGraphType<DogCoursesInput>> { Name = "dogCourses" }),
           resolve: async context =>
           {
               var dogCourses = context.GetArgument<DogCourses>("dogCourses");
               return await dogRepository.InsUpdDogCoursesAsync(dogCourses, context.CancellationToken).ConfigureAwait(false);
           }
           );


            FieldAsync<BooleanGraphType>(
            "deleteDogCourses",
            arguments: new QueryArguments(new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "courseId" }),
            resolve: async context =>
            {
                var courseId = context.GetArgument<string>("courseId");
                return await dogRepository.DeleteDogCoursesAsync(courseId, context.CancellationToken).ConfigureAwait(false);
            }
            );


            #endregion



            #region Menu Master

            FieldAsync<MenuMasterType>(
"insUpdMenuMaster",
arguments: new QueryArguments(new QueryArgument<NonNullGraphType<MenuMasterInput>> { Name = "menuMaster" }),
resolve: async context =>
{
    var menuMaster = context.GetArgument<MenuMaster>("menuMaster");
    return await menuRepository.InsUpdMenuMasterAsync(menuMaster, context.CancellationToken).ConfigureAwait(false);
}
);


            FieldAsync<BooleanGraphType>(
"deleteMenuMaster",
arguments: new QueryArguments(new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "menuId" }),
resolve: async context =>
{
    var menuId = context.GetArgument<string>("menuId");
    return await menuRepository.DeleteMenuMasterAsync(menuId, context.CancellationToken).ConfigureAwait(false);
}
);

            FieldAsync<BooleanGraphType>(
"insUpdMenuAccess",
arguments: new QueryArguments(new QueryArgument<NonNullGraphType<ListGraphType<MenuAccessInput>>> { Name = "menuAccess" }),
resolve: async context =>
{
    var menuAccess = context.GetArgument<List<MenuAccess>>("menuAccess");
    return await menuRepository.InsUpdMenuAccessAsync(menuAccess, context.CancellationToken).ConfigureAwait(false);
}
);


            FieldAsync<BooleanGraphType>(
"deleteMenuAccess",
arguments: new QueryArguments(new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "accessId" }),
resolve: async context =>
{
    var accessId = context.GetArgument<string>("accessId");
    return await menuRepository.DeleteMenuAccessAsync(accessId, context.CancellationToken).ConfigureAwait(false);
}
);


            #endregion

            #region Location Dog Sevices 

            FieldAsync<UserLocationType>(
"insUpdUserLocation",
arguments: new QueryArguments(new QueryArgument<NonNullGraphType<UserLocationInput>> { Name = "userLocation" }),
resolve: async context =>
{
    var userLocation = context.GetArgument<UserLocation>("userLocation");
    return await locationService.InsUpdUserLocationAsync(userLocation, context.CancellationToken).ConfigureAwait(false);
}
);


            FieldAsync<BooleanGraphType>(
"deleteUserLocation",
arguments: new QueryArguments(new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "locationId" }),
resolve: async context =>
{
    var locationId = context.GetArgument<string>("locationId");
    return await locationService.DeleteUserLocationAsync(locationId, context.CancellationToken).ConfigureAwait(false);
}
);

            FieldAsync<BooleanGraphType>(
"deleteSingleLocation",
arguments: new QueryArguments(new List<QueryArgument>
               {
                   new QueryArgument<StringGraphType>
                     {
                        Name = "locationId"
                     },
                     new QueryArgument<StringGraphType>
                     {
                        Name = "address"
                     }
               }),
resolve: async context =>
{
    var locationId = context.GetArgument<string>("locationId");
    var address = context.GetArgument<string>("address");
    return await locationService.DeleteSingleLocationAsync(locationId, address, context.CancellationToken).ConfigureAwait(false);
}
);



            FieldAsync<DogServicesType>(
"insUpdDogServices",
arguments: new QueryArguments(new QueryArgument<NonNullGraphType<DogServicesInput>> { Name = "dogServices" }),
resolve: async context =>
{
    var dogServices = context.GetArgument<DogServices>("dogServices");
    return await locationService.InsUpdDogServicesAsync(dogServices, context.CancellationToken).ConfigureAwait(false);
}
);


            FieldAsync<BooleanGraphType>(
"deleteDogServices",
arguments: new QueryArguments(new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "serviceId" }),
resolve: async context =>
{
    var serviceId = context.GetArgument<string>("serviceId");
    return await locationService.DeleteDogServicesAsync(serviceId, context.CancellationToken).ConfigureAwait(false);
}
);

            #endregion

            #region chat module

            FieldAsync<ConversationMessageType>(
"addPrivateMessage",
arguments: new QueryArguments(new QueryArgument<NonNullGraphType<ConversationMessageInput>> { Name = "userMessage" }),
resolve: async context =>
{
    var userMessage = context.GetArgument<ConversationMessage>("userMessage");
    await chatRepository.AddPrivateMessageAsync(userMessage, context.CancellationToken).ConfigureAwait(false);
    //chatServices.AddUserMessage(userMessage);
    return userMessage;
}
);

            #endregion

            #region Ticket booking


            FieldAsync<TicketingType>(
            "insUpdTicketing",
            arguments: new QueryArguments(new QueryArgument<NonNullGraphType<TicketingInput>> { Name = "ticketing" }),
            resolve: async context =>
            {
                var ticketing = context.GetArgument<Ticketing>("ticketing");
                return await ticketingRepository.InsUpdTicketingAsync(ticketing, context.CancellationToken).ConfigureAwait(false);
            }
            );


            FieldAsync<BooleanGraphType>(
            "deleteTicketing",
            arguments: new QueryArguments(new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "tId" }),
            resolve: async context =>
            {
                var tId = context.GetArgument<string>("tId");
                return await ticketingRepository.DeleteTicketingAsync(tId, context.CancellationToken).ConfigureAwait(false);
            }
            );


            #endregion


            #region UserLocationMedia


            FieldAsync<UserLocationMediaType>(
           "insUpdUserLocationMedia",
           arguments: new QueryArguments(new QueryArgument<NonNullGraphType<UserLocationMediaInput>> { Name = "userLocationMedia" }),
           resolve: async context =>
           {
               var userLocationMedia = context.GetArgument<UserLocationMedia>("userLocationMedia");
               return await locationService.InsUpdUserLocationMediaAsync(userLocationMedia, context.CancellationToken).ConfigureAwait(false);
           }
           );

            FieldAsync<UserLocationMediaType>(
         "insUpdUserProfilePic",
         arguments: new QueryArguments(new QueryArgument<NonNullGraphType<UserLocationMediaInput>> { Name = "userLocationMedia" }),
         resolve: async context =>
         {
             var userLocationMedia = context.GetArgument<UserLocationMedia>("userLocationMedia");
             return await locationService.InsUpdUserProfilePicAsync(userLocationMedia, context.CancellationToken).ConfigureAwait(false);
         }
         );

            FieldAsync<BooleanGraphType>(
            "updProfilePicBit",
            arguments: new QueryArguments(new List<QueryArgument>
                           {
                               new QueryArgument<StringGraphType>
                                 {
                                    Name = "mediaId"
                                 },
                                 new QueryArgument<StringGraphType>
                                 {
                                    Name = "userId"
                                 }
                           }),
            resolve: async context =>
            {
                var mediaId = context.GetArgument<string>("mediaId");
                var userId = context.GetArgument<string>("userId");
                return await locationService.UpdProfilePicBitAsync(mediaId, userId, context.CancellationToken).ConfigureAwait(false);
            }
            );


            FieldAsync<BooleanGraphType>(
"deleteUserLocationMedia",
arguments: new QueryArguments(new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "mediaId" }),
resolve: async context =>
{
    var mediaId = context.GetArgument<string>("mediaId");
    return await locationService.DeleteUserLocationMediaAsync(mediaId, context.CancellationToken).ConfigureAwait(false);
}
);


            #endregion























        }
    }
}
