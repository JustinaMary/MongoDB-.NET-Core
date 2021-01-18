using Dapper;
using GraphQL.Language.AST;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
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
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PetizenApi.Repositories
{
    public class DogRepository : IDogRepository
    {
        private readonly MongoConnection context = null;
        private readonly string MediaUrl = "";
        private readonly ICommonRepository _commonRepository;
        private readonly IConnectionFactory _connectionFactory;


        public DogRepository(IOptions<MongoSettings> settings, IOptions<ApplicationUrl> webUrl, ICommonRepository commonRepository, IConnectionFactory connectionFactory)
        {

            context = new MongoConnection(settings);
            if (webUrl != null)
            {
                MediaUrl = webUrl.Value.MediaUrl.ToString();
            }
            _commonRepository = commonRepository;
            _connectionFactory = connectionFactory;

        }

        #region dog breeds


        public async Task<IList<DogBreeds>> GetDogBreedAsync(string breedId, string groupName, string breedName, IDictionary<string, Field> fields, CancellationToken cancellationToken)
        {
            try
            {
                var builder = Builders<DogBreeds>.Filter;
                var filterDefine = FilterDefinition<DogBreeds>.Empty;

                if (!string.IsNullOrEmpty(breedId))
                {
                    filterDefine = builder.Eq(d => d.BreedId, breedId);
                }
                if (!string.IsNullOrEmpty(groupName))
                {
                    filterDefine = filterDefine & builder.Eq(d => d.GroupName, groupName);
                }
                if (!string.IsNullOrEmpty(breedName))
                {
                    filterDefine = filterDefine & builder.Regex("BreedName", new BsonRegularExpression(breedName, "i"));
                }

                var Result = new List<DogBreeds>();
                if (fields == null)
                {
                    Result = await context.DogBreeds
                                 .Find(filterDefine)
                                 .ToListAsync().ConfigureAwait(false);
                }
                else
                {
                    var fieldsName = fields.Keys.ToList();

                    var fieldsBuilder = Builders<DogBreeds>.Projection;

                    var fieldsAdd = fieldsBuilder.Include(fieldsName[0]);

                    foreach (var item in fieldsName.Skip(1))
                    {
                        fieldsAdd = fieldsAdd.Include(item);
                    }
                    Result = await context.DogBreeds
                                    .Find(filterDefine)
                                    .Project<DogBreeds>(fieldsAdd)
                                 .ToListAsync().ConfigureAwait(false);

                    _ = Result.Select(c => { c.ImagePath = string.IsNullOrEmpty(c.ImagePath) ? c.ImagePath : Path.Combine(MediaUrl, c.ImagePath); return c; }).ToList();

                }

                return Result;


            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception");
                throw;
            }
        }

        public async Task<DogBreeds> InsUpdDogBreedAsync(DogBreeds breeds, CancellationToken cancellationToken)
        {
            try
            {
                if (breeds == null) throw new ArgumentNullException(nameof(breeds));

                if (string.IsNullOrEmpty(breeds.BreedId))
                {
                    await context.DogBreeds.InsertOneAsync(breeds).ConfigureAwait(false);
                }
                else
                {
                    var update = Builders<DogBreeds>.Update.Set(x => x.BreedName, breeds.BreedName)
                                          .Set(x => x.GroupName, breeds.GroupName)
                                          .Set(x => x.Description, breeds.Description);
                    //.Set(x => x.ImagePath, breeds.ImagePath);
                    await context.DogBreeds.UpdateOneAsync(t => t.BreedId == breeds.BreedId, update).ConfigureAwait(false);

                    if (!string.IsNullOrEmpty(breeds.ImagePath))
                    {
                        update = Builders<DogBreeds>.Update.Set(x => x.ImagePath, breeds.ImagePath);
                        await context.DogBreeds.UpdateOneAsync(t => t.BreedId == breeds.BreedId, update).ConfigureAwait(false);

                    }
                }

                return breeds;

            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception");
                throw;
            }
        }

        public async Task<bool> DeleteDogBreedAsync(string breedId, CancellationToken cancellationToken)
        {
            try
            {
                await context.DogBreeds.DeleteOneAsync(
                         Builders<DogBreeds>.Filter.Eq("BreedId", breedId)).ConfigureAwait(false);

                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception");
                throw;
            }
        }


        #endregion


        #region Dog Master


        public async Task<List<DogMaster>> GetDogAsync(string dogId, string breedId, string ownerId, string gender, IDictionary<string, Field> fields, CancellationToken cancellationToken)
        {
            try
            {
                bool bGetOwnerInfo = false;

                var builder = Builders<DogMaster>.Filter;
                var filterDefine = FilterDefinition<DogMaster>.Empty;

                if (!string.IsNullOrEmpty(dogId))
                {
                    filterDefine = builder.Eq(d => d.DogId, dogId);
                }
                if (!string.IsNullOrEmpty(breedId))
                {
                    filterDefine = filterDefine & builder.Eq(d => d.BreedId, breedId);
                }
                if (!string.IsNullOrEmpty(ownerId))
                {
                    filterDefine = filterDefine & builder.Eq(d => d.OwnerId, ownerId);
                }
                if (!string.IsNullOrEmpty(gender))
                {
                    filterDefine = filterDefine & builder.Eq(d => d.Gender, gender);
                }

                var Result = new List<DogMaster>();


                if (fields == null)
                {
                    Result = await context.DogMaster
                                  .Find(filterDefine)
                                  .ToListAsync().ConfigureAwait(false);

                    bGetOwnerInfo = true;
                }
                else
                {
                    var fieldsName = fields.Keys.ToList();

                    var fieldsBuilder = Builders<DogMaster>.Projection;

                    var fieldsAdd = fieldsBuilder.Include(fieldsName[0]);

                    foreach (var item in fieldsName.Skip(1))
                    {
                        fieldsAdd = fieldsAdd.Include(item);
                    }

                    if (fieldsName.Any(str => str.Contains("dogOwnerInfo", StringComparison.CurrentCulture)))
                    {
                        bGetOwnerInfo = true;
                    }

                    if (fieldsName.Any(str => str.Contains("dogMediaList", StringComparison.CurrentCulture)) && fieldsName.Any(str => str.Contains("dogBreedInfo", StringComparison.CurrentCulture)))
                    {
                        Result = await context.DogMaster.Aggregate()
                        .Match(filterDefine)
                        .Lookup(context.DogMedia, m => m.DogId, c => c.DogId, (DogMaster m) => m.DogMediaList)
                        .Lookup(context.DogBreeds, m => m.BreedId, c => c.BreedId, (DogMaster m) => m.DogBreedInfo)
                        .Project<DogMaster>(fieldsAdd)
                        .ToListAsync().ConfigureAwait(false);

                        //Result = await context.DogMaster.Aggregate().Lookup("foreignCollectionName", "localFieldName", "foreignFieldName", "result").Project(Builders<BsonDocument>.Projection.Exclude("_id"))
                        //.ToList()
                    }
                    else if (fieldsName.Any(str => str.Contains("dogMediaList", StringComparison.CurrentCulture)))
                    {
                        Result = await context.DogMaster.Aggregate()
                       .Match(filterDefine)
                       .Lookup(context.DogMedia, m => m.DogId, c => c.DogId, (DogMaster m) => m.DogMediaList)
                       .Project<DogMaster>(fieldsAdd)
                       .ToListAsync().ConfigureAwait(false);
                    }
                    else if (fieldsName.Any(str => str.Contains("dogBreedInfo", StringComparison.CurrentCulture)))
                    {
                        Result = await context.DogMaster.Aggregate()
                       .Match(filterDefine)
                       .Lookup(context.DogBreeds, m => m.BreedId, c => c.BreedId, (DogMaster m) => m.DogBreedInfo)
                       .Project<DogMaster>(fieldsAdd)
                       .ToListAsync().ConfigureAwait(false);
                    }
                    else
                    {
                        Result = await context.DogMaster.Aggregate()
                       .Match(filterDefine)
                       .Project<DogMaster>(fieldsAdd)
                       .ToListAsync().ConfigureAwait(false);
                    }

                    //Result = await context.DogMaster.Aggregate().Lookup("foreignCollectionName", "localFieldName", "foreignFieldName", "result").Project(Builders<BsonDocument>.Projection.Exclude("_id")).ToList()
                    // Result.Select(c => { c.ImagePath = string.IsNullOrEmpty(c.ImagePath) ? c.ImagePath : Path.Combine(MediaUrl, c.ImagePath); return c; }).ToList();

                }


                if (Result.Count > 0 && bGetOwnerInfo)
                {
                    List<Guid> allOwnerIds = new List<Guid>();
                    var ownerUserMaster = new List<UserMaster>();
                    allOwnerIds = Result.Select(x => new Guid(x.OwnerId)).ToList();

                    DataTable _DataTable = GenerateDataTable(allOwnerIds);
                    var procedureName = "usp_UserMasterGetFromUserList";


                    try
                    {
                        using (var multiResult = await SqlMapper.QueryMultipleAsync(_connectionFactory.GetConnection, procedureName,
                            new { UserList = _DataTable, Role = Guid.Empty.ToString() }, commandType: CommandType.StoredProcedure).ConfigureAwait(false))
                        {
                            ownerUserMaster = (await multiResult.ReadAsync<UserMaster>().ConfigureAwait(false)).ToList();

                            Result = Result.Select(c => { c.DogOwnerInfo = ownerUserMaster.Where(x => x.UserId.ToString() == c.OwnerId.ToString()).ToList(); return c; }).ToList();

                        }

                    }
                    finally
                    {
                        _connectionFactory.CloseConnection();
                    }
                }

                return Result;

            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception");
                throw;
            }
        }

        private static DataTable GenerateDataTable(List<Guid> UserId)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("UserId");


            foreach (var item in UserId)
            {
                DataRow row = dt.NewRow();
                row["UserId"] = (item);
                dt.Rows.Add(row);

            }

            return dt;
        }



        public async Task<List<DogMaster>> GetDogForDetailsAsync(string ownerId, CancellationToken cancellationToken)
        {
            try
            {
                var builder = Builders<DogMaster>.Filter;
                var filterDefine = FilterDefinition<DogMaster>.Empty;


                if (!string.IsNullOrEmpty(ownerId))
                {
                    filterDefine = filterDefine & builder.Eq(d => d.OwnerId, ownerId);
                }

                var Result = new List<DogMaster>();

                Result = await context.DogMaster.Aggregate()
                    .Match(Builders<DogMaster>.Filter.Eq(d => d.OwnerId, ownerId))
                    .Lookup(context.DogMedia, m => m.DogId, c => c.DogId, (DogMaster m) => m.DogMediaList)
                    .Lookup(context.DogBreeds, m => m.BreedId, c => c.BreedId, (DogMaster m) => m.DogBreedInfo)
                    .Project<DogMaster>(Builders<DogMaster>.Projection.Exclude("UpdatedDate"))
                    .ToListAsync().ConfigureAwait(true);


                //Result = await context.DogMaster.Aggregate().Lookup("foreignCollectionName", "localFieldName", "foreignFieldName", "result").Project(Builders<BsonDocument>.Projection.Exclude("_id"))
                //.ToList()

                // Result.Select(c => { c.ImagePath = string.IsNullOrEmpty(c.ImagePath) ? c.ImagePath : Path.Combine(MediaUrl, c.ImagePath); return c; }).ToList();

                Result = Result.Select(t => { _ = t.DogMediaList.Select(c => { c.MediaPath = string.IsNullOrEmpty(c.MediaPath) ? c.MediaPath : MediaUrl + c.MediaPath; return c; }).ToList(); return t; }).ToList();

                return Result;

            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception");
                throw;
            }
        }



        public async Task<DogMaster> InsUpdDogAsync(DogMaster dog, CancellationToken cancellationToken)
        {
            try
            {
                if (dog == null) throw new ArgumentNullException(nameof(dog));

                if (string.IsNullOrEmpty(dog.DogId))
                {
                    await context.DogMaster.InsertOneAsync(dog).ConfigureAwait(false);
                }
                else
                {

                    var dogMasters = await GetDogAsync(dog.DogId, "", "", "", null, cancellationToken).ConfigureAwait(false);

                    var update = Builders<DogMaster>.Update.
                                         Set(x => x.Name, !string.IsNullOrEmpty(dog.Name) ? dog.Name : dogMasters[0].Name)
                                        .Set(x => x.Gender, !string.IsNullOrEmpty(dog.Gender) ? dog.Gender : dogMasters[0].Gender)
                                        .Set(x => x.BreedId, !string.IsNullOrEmpty(dog.BreedId) ? dog.BreedId : dogMasters[0].BreedId)
                                        .Set(x => x.Lineage, !string.IsNullOrEmpty(dog.Lineage) ? dog.Lineage : dogMasters[0].Lineage)
                                        .Set(x => x.DOB, dog.DOB != DateTime.MinValue ? dog.DOB : dogMasters[0].DOB)
                                        .Set(x => x.Color, !string.IsNullOrEmpty(dog.Color) ? dog.Color : dogMasters[0].Color)
                                        .Set(x => x.Height, !string.IsNullOrEmpty(dog.Height) ? dog.Height : dogMasters[0].Height)
                                        .Set(x => x.Weight, !string.IsNullOrEmpty(dog.Weight) ? dog.Weight : dogMasters[0].Weight)
                                        .Set(x => x.Summary, !string.IsNullOrEmpty(dog.Summary) ? dog.Summary : dogMasters[0].Summary);

                    dog.UpdatedDate = DateTime.Now;
                    dog.UpdatedBy = dog.InsertedBy;
                    await context.DogMaster.UpdateOneAsync(t => t.DogId == dog.DogId, update).ConfigureAwait(false);

                }

                return dog;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception");
                throw;
            }
        }


        public async Task<bool> DeleteDogAsync(string dogId, CancellationToken cancellationToken)
        {
            try
            {
                await context.DogMaster.FindOneAndDeleteAsync(
                        Builders<DogMaster>.Filter.Eq("DogId", dogId)).ConfigureAwait(false);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception");
                throw;
            }
        }


        public async Task<List<DogMaster>> SearchDogAsync(string breedId, string gender, string latitude, string longitude, int distance, IDictionary<string, Field> fields, CancellationToken cancellationToken)
        {
            try
            {
                bool isLocationFiltered = false;
                List<UserLocation> userLocList = new List<UserLocation>();

                if (!string.IsNullOrEmpty(latitude) && !string.IsNullOrEmpty(longitude) && distance != 0)
                {
                    var provider = new NumberFormatInfo();
                    var point = GeoJson.Point(GeoJson.Geographic(Convert.ToDouble(longitude, provider), Convert.ToDouble(latitude, provider)));

                    //filter owner
                    var locationQuery = new FilterDefinitionBuilder<UserLocation>().NearSphere(tag => tag.Location1, point,
                    (distance * 1000)); //fetch results that are within a 5000 metre radius of the point we're searching.

                    userLocList = await context.UserLocation.Find(locationQuery).ToListAsync().ConfigureAwait(false);
                    userLocList = userLocList.Where(x => x.RoleId.ToLower(CultureInfo.CurrentCulture) == "763938ce-8a55-4a78-82f7-b1a97ac17802").ToList();

                    isLocationFiltered = true;
                }
                var builder = Builders<DogMaster>.Filter;
                var filterDefine = FilterDefinition<DogMaster>.Empty;

                if (!string.IsNullOrEmpty(breedId))
                {
                    filterDefine = builder.Eq(d => d.BreedId, breedId);
                }

                if (!string.IsNullOrEmpty(gender))
                {
                    filterDefine = filterDefine & builder.Eq(d => d.Gender, gender);
                }

                var Result = new List<DogMaster>();
                if (fields == null) throw new ArgumentNullException(nameof(fields));
                var fieldsName = fields.Keys.ToList();

                var fieldsBuilder = Builders<DogMaster>.Projection;

                var fieldsAdd = fieldsBuilder.Include(fieldsName[0]);

                foreach (var item in fieldsName.Skip(1))
                {
                    fieldsAdd = fieldsAdd.Include(item);
                }

                if (fieldsName.Any(str => str.Contains("dogBreedInfo", StringComparison.CurrentCulture)))
                {
                    Result = await context.DogMaster.Aggregate()
                   .Match(filterDefine)
                   .Lookup(context.DogBreeds, m => m.BreedId, c => c.BreedId, (DogMaster m) => m.DogBreedInfo)
                   .Project<DogMaster>(fieldsAdd)
                   .ToListAsync().ConfigureAwait(false);
                }
                else
                {
                    Result = await context.DogMaster
                                    .Find(filterDefine)
                                    .Project<DogMaster>(fieldsAdd)
                                 .ToListAsync().ConfigureAwait(false);
                }






                var finalRes = new List<DogMaster>();
                if (isLocationFiltered)//no idea y added
                {
                    finalRes = Result.Where(x => userLocList.Select(y => y.UserId).Contains(x.OwnerId)).ToList();
                }
                else
                {
                    finalRes = Result;
                }


                if (finalRes.Count > 0 && fieldsName.Any(str => str.Contains("dogOwnerInfo", StringComparison.CurrentCulture)))
                {
                    List<Guid> allOwnerIds = new List<Guid>();
                    var ownerUserMaster = new List<UserMaster>();
                    allOwnerIds = Result.Select(x => new Guid(x.OwnerId)).ToList();

                    DataTable _DataTable = GenerateDataTable(allOwnerIds);
                    var procedureName = "usp_UserMasterGetFromUserList";


                    try
                    {
                        using (var multiResult = await SqlMapper.QueryMultipleAsync(_connectionFactory.GetConnection, procedureName,
                            new { UserList = _DataTable, Role = Guid.Empty.ToString() }, commandType: CommandType.StoredProcedure).ConfigureAwait(false))
                        {
                            ownerUserMaster = (await multiResult.ReadAsync<UserMaster>().ConfigureAwait(false)).ToList();

                            finalRes = finalRes.Select(c => { c.DogOwnerInfo = ownerUserMaster.Where(x => x.UserId.ToString() == c.OwnerId.ToString()).ToList(); return c; }).ToList();

                        }

                    }
                    finally
                    {
                        _connectionFactory.CloseConnection();
                    }
                }

                return finalRes;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception");
                throw;
            }
        }




        #endregion


        #region Dog media
        public async Task<IEnumerable<DogMedia>> GetDogsMediaAsync(string mediaId, string dogId, CancellationToken cancellationToken)
        {
            try
            {
                var builder = Builders<DogMedia>.Filter;
                var filterDefine = FilterDefinition<DogMedia>.Empty;

                if (!string.IsNullOrEmpty(mediaId))
                {
                    filterDefine = builder.Eq(d => d.MediaId, mediaId);
                }
                if (!string.IsNullOrEmpty(dogId))
                {
                    filterDefine = filterDefine & builder.Eq(d => d.DogId, dogId);
                }
                var Result = await context.DogMedia
                                  .Find(filterDefine)
                                  .ToListAsync().ConfigureAwait(false);

                Result = Result.Select(c =>
                  {
                      c.MediaPath = string.IsNullOrEmpty(c.MediaPath) ? c.MediaPath :
                      c.MediaPath.IndexOf("http", StringComparison.CurrentCulture) > -1 ? c.MediaPath : MediaUrl + "/Media/" + c.MediaPath; return c;
                  }).ToList();

                return Result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception");
                throw;
            }
        }

        public async Task<DogMedia> InsUpdDogMediaAsync(DogMedia dogMedia, CancellationToken cancellationToken)
        {
            try
            {
                if (dogMedia == null) throw new ArgumentNullException(nameof(dogMedia));

                if (string.IsNullOrEmpty(dogMedia.MediaId))
                {
                    await context.DogMedia.InsertOneAsync(dogMedia).ConfigureAwait(false);
                }
                else
                {
                    await context.DogMedia.ReplaceOneAsync(t => t.MediaId == dogMedia.MediaId, dogMedia).ConfigureAwait(false);
                }

                return dogMedia;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception");
                throw;
            }
        }



        public async Task<bool> DeleteDogMediaAsync(string mediaId, CancellationToken cancellationToken)
        {
            try
            {
                await context.DogMedia.DeleteOneAsync(
                        Builders<DogMedia>.Filter.Eq("MediaId", mediaId)).ConfigureAwait(false);

                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception");
                throw;
            }
        }

        #endregion

        #region Dog Characteristics

        public async Task<List<DogCharacteristics>> GetDogCharacteristicsAsync(string characterId, string description, CancellationToken cancellationToken)
        {
            try
            {
                var builder = Builders<DogCharacteristics>.Filter;
                var filterDefine = FilterDefinition<DogCharacteristics>.Empty;

                if (!string.IsNullOrEmpty(characterId))
                {
                    filterDefine = builder.Eq(d => d.CharacterId, characterId);
                }
                if (!string.IsNullOrEmpty(description))
                {
                    filterDefine = filterDefine & builder.Regex("Description", new BsonRegularExpression(description, "i"));

                }
                return await context.DogCharacteristics
                                   .Find(filterDefine)
                                   .ToListAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception");
                throw;
            }
        }

        public async Task<DogCharacteristics> InsUpdDogCharacteristicsAsync(DogCharacteristics characteristics, CancellationToken cancellationToken)
        {
            try
            {
                if (characteristics == null) throw new ArgumentNullException(nameof(characteristics));

                if (string.IsNullOrEmpty(characteristics.CharacterId))
                {
                    await context.DogCharacteristics.InsertOneAsync(characteristics).ConfigureAwait(false);
                }
                else
                {
                    var dogCharacteristics = await GetDogCharacteristicsAsync(characteristics.CharacterId, "", cancellationToken).ConfigureAwait(false);

                    var update = Builders<DogCharacteristics>.Update.
                                            Set(x => x.Description, !string.IsNullOrEmpty(characteristics.Description) ? characteristics.Description :
                                            dogCharacteristics[0].Description);

                    await context.DogCharacteristics.UpdateOneAsync(t => t.CharacterId == characteristics.CharacterId, update).ConfigureAwait(false);

                }

                return characteristics;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception");
                throw;
            }
        }

        public async Task<bool> DeleteDogCharacteristicsAsync(string characterId, CancellationToken cancellationToken)
        {
            try
            {
                await context.DogCharacteristics.DeleteOneAsync(
                        Builders<DogCharacteristics>.Filter.Eq("CharacterId", characterId)).ConfigureAwait(false);

                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception");
                throw;
            }
        }

        #endregion

        #region Favourite Dog

        public async Task<IEnumerable<FavouriteDog>> GetFavouriteAsync(string UserId, string forDogId, CancellationToken cancellationToken)
        {
            try
            {

                var builder = Builders<FavouriteDog>.Filter;
                var filterDefine = FilterDefinition<FavouriteDog>.Empty;

                if (!string.IsNullOrEmpty(UserId))
                {
                    filterDefine = builder.Eq(d => d.UserId, UserId);
                }
                return await context.FavouriteDog
                                  .Find(filterDefine)
                                  .ToListAsync().ConfigureAwait(false);

            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception");
                throw;
            }
        }

        public async Task<FavouriteDog> InsUpdFavDogAsync(FavouriteDog favouriteDog, CancellationToken cancellationToken)
        {
            try
            {
                if (favouriteDog == null) throw new ArgumentNullException(nameof(favouriteDog));

                if (string.IsNullOrEmpty(favouriteDog.FavId))
                {
                    await context.FavouriteDog.InsertOneAsync(favouriteDog).ConfigureAwait(false);
                }
                else
                {
                    await context.FavouriteDog.ReplaceOneAsync(t => t.FavId == favouriteDog.FavId, favouriteDog).ConfigureAwait(false);
                }

                return favouriteDog;

            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception");
                throw;
            }
        }

        public async Task<bool> DeleteFavDogAsync(string FavId, CancellationToken cancellationToken)
        {
            try
            {
                await context.FavouriteDog.DeleteOneAsync(
                      Builders<FavouriteDog>.Filter.Eq("FavId", FavId)).ConfigureAwait(false);

                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception");
                throw;
            }
        }

        #endregion


        #region Dog Courses/trainer

        public async Task<List<DogCourses>> GetDogCoursesAsync(string courseId, string userId, IDictionary<string, Field> fields, CancellationToken cancellationToken)
        {


            try
            {
                var builder = Builders<DogCourses>.Filter;
                var filterDefine = FilterDefinition<DogCourses>.Empty;

                if (!string.IsNullOrEmpty(courseId))
                {
                    filterDefine = builder.Eq(d => d.CourseId, courseId);
                }

                if (!string.IsNullOrEmpty(userId))
                {
                    filterDefine = builder.Eq(d => d.UserId, userId);
                }

                var Result = new List<DogCourses>();
                if (fields == null)
                {
                    Result = await context.DogCourses
                                   .Find(filterDefine)
                                   .ToListAsync().ConfigureAwait(false);
                }
                else
                {
                    var fieldsName = fields.Keys.ToList();

                    var fieldsBuilder = Builders<DogCourses>.Projection;

                    var fieldsAdd = fieldsBuilder.Include(fieldsName[0]);

                    foreach (var item in fieldsName.Skip(1))
                    {
                        fieldsAdd = fieldsAdd.Include(item);
                    }
                    Result = await context.DogCourses
                                    .Find(filterDefine)
                                    .Project<DogCourses>(fieldsAdd)
                                 .ToListAsync().ConfigureAwait(false);

                }

                Result = Result.Select(c => { c.ImagePath = string.IsNullOrEmpty(c.ImagePath) ? c.ImagePath : Path.Combine(MediaUrl, c.ImagePath); return c; }).ToList();

                return Result;


            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception");
                throw;
            }
        }

        public async Task<DogCourses> InsUpdDogCoursesAsync(DogCourses courses, CancellationToken cancellationToken)
        {
            try
            {
                if (courses == null) throw new ArgumentNullException(nameof(courses));


                if (string.IsNullOrEmpty(courses.CourseId))
                {
                    await context.DogCourses.InsertOneAsync(courses).ConfigureAwait(false);
                }
                else
                {
                    var dogCourses = await GetDogCoursesAsync(courses.CourseId, "", null, cancellationToken).ConfigureAwait(false);

                    var update = Builders<DogCourses>.Update.Set(x => x.Title, !string.IsNullOrEmpty(courses.Title) ? courses.Title : dogCourses[0].Title)
                       .Set(x => x.Description, !string.IsNullOrEmpty(courses.Description) ? courses.Description : dogCourses[0].Description)
                       .Set(x => x.ImagePath, !string.IsNullOrEmpty(courses.ImagePath) ? courses.ImagePath : dogCourses[0].ImagePath)
                       //.Set(x => x.Duration, !string.IsNullOrEmpty(courses.Duration) ? courses.Duration : dogCourses[0].Duration)
                       .Set(x => x.Amount, courses.Amount != 0 ? courses.Amount : dogCourses[0].Amount);

                    await context.DogCourses.UpdateOneAsync(t => t.CourseId == courses.CourseId, update).ConfigureAwait(false);

                }

                return courses;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception");
                throw;
            }
        }

        public async Task<bool> DeleteDogCoursesAsync(string courseId, CancellationToken cancellationToken)
        {
            try
            {
                await context.DogCourses.DeleteOneAsync(
                        Builders<DogCourses>.Filter.Eq("CourseId", courseId)).ConfigureAwait(false);

                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception");
                throw;
            }
        }

        #endregion




    }
}
