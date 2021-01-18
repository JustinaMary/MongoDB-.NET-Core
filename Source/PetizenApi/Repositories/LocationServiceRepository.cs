


using Dapper;
using GraphQL.Language.AST;
//using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.GeoJsonObjectModel;
using PetizenApi.Database;
using PetizenApi.Interfaces;
using PetizenApi.Models;
using PetizenApi.Providers;
using PetizenApi.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PetizenApi.Repositories
{
    public class LocationServiceRepository : ILocationServiceRepository
    {
        private readonly MongoConnection context = null;
        private readonly ICommonRepository _commonRepository;
        private readonly IConnectionFactory _connectionFactory;
        //private readonly ApplicationUrl _appurl = null;

        string MediaUrl = "";

        public LocationServiceRepository(IOptions<MongoSettings> settings, ICommonRepository commonRepository, IConnectionFactory connectionFactory,
            IOptions<ApplicationUrl> webUrl)//, IOptions<ApplicationUrl> appurl
        {
            context = new MongoConnection(settings);
            _commonRepository = commonRepository;
            _connectionFactory = connectionFactory;

            //_appurl = appurl.Value;
            if (webUrl != null)
            {
                MediaUrl = webUrl.Value.MediaUrl.ToString();
            }




        }

        #region User location

        public async Task<List<UserLocation>> GetUserLocationAsync(string locationId, string userId, string roleId, CancellationToken cancellationToken)
        {
            try
            {
                var builder = Builders<UserLocation>.Filter;
                var filterDefine = FilterDefinition<UserLocation>.Empty;


                if (!string.IsNullOrEmpty(locationId))
                {
                    filterDefine = builder.Eq(d => d.LocationId, locationId);
                }
                if (!string.IsNullOrEmpty(userId))
                {
                    filterDefine = filterDefine & builder.Eq(d => d.UserId, userId);
                }
                if (!string.IsNullOrEmpty(roleId))
                {
                    filterDefine = filterDefine & builder.Eq(d => d.RoleId, roleId);
                }
                var Result = await context.UserLocation
                                  .Find(filterDefine)
                                  .ToListAsync().ConfigureAwait(false);
                return Result;
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }



        }

        //public async Task<List<UserLocation>> SearchUser(double lat, double lng)
        //{
        //    try
        //    {

        //        var point = GeoJson.Point(GeoJson.Geographic(lng, lat));
        //        var locationQuery = new FilterDefinitionBuilder<UserLocation>().NearSphere(tag => tag.Location1, point,
        //            5000); //fetch results that are within a 5000 metre radius of the point we're searching.
        //        var query = context.UserLocation.Find(locationQuery).Limit(10); //Limit the query to return only the top 10 results.
        //        return await query.ToListAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        //do something;
        //    }
        //    return null;
        //}


        public async Task<List<UserMaster>> SearchUserAsync(string role, string latitude, string longitude, int distance, IDictionary<string, Field> fields, CancellationToken cancellationToken)
        {
            try
            {
                List<Guid> allOwnerIds = new List<Guid>();
                bool onlyRoleWiseFilter = false;
                var userMaster = new List<UserMaster>();


                if ((!string.IsNullOrEmpty(latitude)) && (!string.IsNullOrEmpty(longitude)) && distance != 0)
                {
                    List<UserLocation> userLocList = new List<UserLocation>();
                    var point = GeoJson.Point(GeoJson.Geographic(Convert.ToDouble(longitude, CultureInfo.CurrentCulture), Convert.ToDouble(latitude, CultureInfo.CurrentCulture)));

                    if (!string.IsNullOrEmpty(role) && role.ToLower(CultureInfo.CurrentCulture) == "ccab934c-5823-4e16-8112-1e28ee4dd430" || role.ToLower(CultureInfo.CurrentCulture) == "fa26e5f9-c65d-4ea2-8c81-bec260fa090e")
                    {//for Dog Walker,Trainer
                        var locationMultiPointQuery = new FilterDefinitionBuilder<UserLocation>().Near(tag => tag.Location2, point,
                       (distance * 1000)); //fetch results that are within a 5000 metre radius of the point we're searching.
                        userLocList = await context.UserLocation.Find(locationMultiPointQuery).ToListAsync().ConfigureAwait(false);
                    }
                    else if (!string.IsNullOrEmpty(role))
                    {//Vet,Parent,Breeder,Agency

                        var locationQuery = new FilterDefinitionBuilder<UserLocation>().NearSphere(tag => tag.Location1, point,
                        (distance * 1000)); //fetch results that are within a 5000 metre radius of the point we're searching.

                        userLocList = await context.UserLocation.Find(locationQuery).ToListAsync().ConfigureAwait(false);

                    }
                    else
                    {
                        var locationMultiPointQuery = new FilterDefinitionBuilder<UserLocation>().Near(tag => tag.Location2, point,
                                5000); //fetch results that are within a 5000 metre radius of the point we're searching.
                        var loc1 = await context.UserLocation.Find(locationMultiPointQuery).ToListAsync().ConfigureAwait(false);

                        var locationQuery = new FilterDefinitionBuilder<UserLocation>().NearSphere(tag => tag.Location1, point,
                        5000); //fetch results that are within a 5000 metre radius of the point we're searching.

                        var loc2 = await context.UserLocation.Find(locationQuery).ToListAsync().ConfigureAwait(false);

                        userLocList.AddRange(loc1);
                        userLocList.AddRange(loc2);
                    }

                    if (!string.IsNullOrEmpty(role))
                    {
                        userLocList = userLocList.Where(x => x.RoleId.ToLower(CultureInfo.CurrentCulture) == role.ToLower(CultureInfo.CurrentCulture)).ToList();
                    }

                    allOwnerIds = userLocList.Select(x => new Guid(x.UserId)).ToList();

                }
                else if (!string.IsNullOrEmpty(role))
                {
                    //only role wise filter
                    onlyRoleWiseFilter = true;
                }
                else
                {
                    //no filter
                    return userMaster;
                }


                DataTable _DataTable = GenerateDataTable(allOwnerIds);
                var procedureName = "usp_UserMasterGetFromUserList";

                if (!onlyRoleWiseFilter)
                    role = Guid.Empty.ToString();


                try
                {
                    using (var multiResult = await SqlMapper.QueryMultipleAsync(_connectionFactory.GetConnection, procedureName,
                        new { UserList = _DataTable, Role = role }, commandType: CommandType.StoredProcedure).ConfigureAwait(false))
                    {
                        userMaster = (await multiResult.ReadAsync<UserMaster>().ConfigureAwait(false)).ToList();
                    }

                }
                finally
                {
                    _connectionFactory.CloseConnection();
                }

                return userMaster;


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }


        public async Task<UserLocation> InsUpdUserLocationAsync(UserLocation userLocation, CancellationToken cancellationToken)
        {
            var coordinates = new List<GeoJson2DGeographicCoordinates>();

            try
            {
                if (userLocation == null) throw new ArgumentNullException(nameof(userLocation));

                if (string.IsNullOrEmpty(userLocation.LocationId))
                {

                    if (userLocation.LatitudeOne > 0 && userLocation.LongitudeOne > 0)
                    {
                        userLocation.Location1 = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(new GeoJson2DGeographicCoordinates(userLocation.LongitudeOne, userLocation.LatitudeOne));

                    }

                    if (userLocation.LatitudeTwo != null && userLocation.LongitudeTwo != null)
                    {
                        if (userLocation.LatitudeTwo.Any() && userLocation.LongitudeTwo.Any() && userLocation.LatitudeTwo.Count == userLocation.LongitudeTwo.Count)
                        {
                            for (int j = 0; j < userLocation.LatitudeTwo.Count; j++)
                            {
                                var coordinate = new GeoJson2DGeographicCoordinates(userLocation.LongitudeTwo[j], userLocation.LatitudeTwo[j]);
                                coordinates.Add(coordinate);
                            }

                            userLocation.Location2 = GeoJson.MultiPoint(coordinates.ToArray());
                        }
                    }

                    await context.UserLocation.InsertOneAsync(userLocation).ConfigureAwait(false);
                }
                else
                {
                    var UserLocation = await GetUserLocationAsync(userLocation.LocationId, "", "", cancellationToken).ConfigureAwait(false);


                    var update = Builders<UserLocation>.Update
                                        .Set(x => x.UserId, !string.IsNullOrEmpty(userLocation.UserId) ? userLocation.UserId : UserLocation[0].UserId)
                                        .Set(x => x.RoleId, !string.IsNullOrEmpty(userLocation.RoleId) ? userLocation.RoleId : UserLocation[0].RoleId)
                                        .Set(x => x.Name, !string.IsNullOrEmpty(userLocation.Name) ? userLocation.Name : UserLocation[0].Name)
                                        .Set(x => x.LocationOne, !string.IsNullOrEmpty(userLocation.LocationOne) ? userLocation.LocationOne : UserLocation[0].LocationOne)
                                        .Set(x => x.Location1, userLocation.LatitudeOne > 0 && userLocation.LongitudeOne > 0 ? new GeoJsonPoint<GeoJson2DGeographicCoordinates>(new GeoJson2DGeographicCoordinates(userLocation.LongitudeOne, userLocation.LatitudeOne)) : UserLocation[0].Location1);

                    if (userLocation.LatitudeTwo != null && userLocation.LongitudeTwo != null)
                    {
                        if (userLocation.LatitudeTwo.Any() && userLocation.LongitudeTwo.Any() && userLocation.LatitudeTwo.Count == userLocation.LongitudeTwo.Count)
                        {
                            var updatedLocation = new List<string>();
                            updatedLocation.AddRange(UserLocation[0].LocationTwo);
                            var checkExist = userLocation.LocationTwo.Except(UserLocation[0].LocationTwo);
                            updatedLocation.AddRange(checkExist);
                            update = update.Set(x => x.LocationTwo, updatedLocation);

                            //for coordinates 
                            for (int i = 0; i < UserLocation[0].Location2.Coordinates.Positions.Count; i++)
                            {
                                var coordinate = new GeoJson2DGeographicCoordinates(UserLocation[0].Location2.Coordinates.Positions[i].Longitude,
                                    UserLocation[0].Location2.Coordinates.Positions[i].Latitude);
                                coordinates.Add(coordinate);
                            }
                            if (checkExist.Any())
                            {
                                for (int j = 0; j < userLocation.LatitudeTwo.Count; j++)
                                {
                                    var coordinate = new GeoJson2DGeographicCoordinates(userLocation.LongitudeTwo[j], userLocation.LatitudeTwo[j]);
                                    coordinates.Add(coordinate);
                                }
                            }


                            update = update.Set(x => x.Location2, GeoJson.MultiPoint(coordinates.ToArray()));
                        }
                    }

                    await context.UserLocation.UpdateOneAsync(t => t.LocationId == userLocation.LocationId, update).ConfigureAwait(false);
                }

                return userLocation;
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }
        }


        public async Task<bool> DeleteUserLocationAsync(string locationId, CancellationToken cancellationToken)
        {
            try
            {
                await context.UserLocation.DeleteOneAsync(
                        Builders<UserLocation>.Filter.Eq("LocationId", locationId)).ConfigureAwait(false);

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }
        }

        public async Task<bool> DeleteSingleLocationAsync(string locationId, string address, CancellationToken cancellationToken)
        {
            try
            {
                var UserLocation = await GetUserLocationAsync(locationId, "", "", cancellationToken).ConfigureAwait(false);

                //if (address == null) throw new ArgumentNullException(nameof(address));






                int index = UserLocation[0].LocationTwo.FindIndex(x => x.Contains(address, StringComparison.CurrentCulture));

                UserLocation[0].LocationTwo.RemoveAt(index);
                //var index = Array.FindIndex(UserLocation[0].LocationTwo, row => row.Contains(address));
                //UserLocation[0].LocationTwo = UserLocation[0].LocationTwo.Where((val, idx) => idx != index).ToArray();

                var coordinates = new List<GeoJson2DGeographicCoordinates>();

                //for coordinates 
                for (int i = 0; i < UserLocation[0].Location2.Coordinates.Positions.Count; i++)
                {
                    var coordinate = new GeoJson2DGeographicCoordinates(UserLocation[0].Location2.Coordinates.Positions[i].Longitude,
                        UserLocation[0].Location2.Coordinates.Positions[i].Latitude);
                    if (index != i)
                    {
                        coordinates.Add(coordinate);
                    }
                }


                var update = Builders<UserLocation>.Update
                                      .Set(x => x.LocationTwo, UserLocation[0].LocationTwo);
                update = update.Set(x => x.Location2, GeoJson.MultiPoint(coordinates.ToArray()));


                await context.UserLocation.UpdateOneAsync(t => t.LocationId == locationId, update).ConfigureAwait(false);

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion

        #region  dog services

        public async Task<List<DogServices>> GetDogServicesAsync(string serviceId, string locationId, string userId, string serviceName, IDictionary<string, Field> fields, CancellationToken cancellationToken)
        {
            try
            {

                var builder = Builders<DogServices>.Filter;
                var filterDefine = FilterDefinition<DogServices>.Empty;
                var filterDefineLocation = FilterDefinition<DogServices>.Empty;

                List<string> locIds = new List<string>();
                if (!string.IsNullOrEmpty(userId))
                {
                    //var a = context.UserLocation.Find(t => (t.UserId.ToLower() == userId.ToLower())).ToList();
                    locIds = context.UserLocation.Find(t => t.UserId == userId).ToList().Select(x => x.LocationId).ToList();


                }

                if (!string.IsNullOrEmpty(serviceId))
                {
                    filterDefine = builder.Eq(d => d.ServiceId, serviceId);
                }

                if (locIds.Count > 0)
                {
                    foreach (var item in locIds)
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            if (filterDefineLocation == FilterDefinition<DogServices>.Empty)
                            {
                                filterDefineLocation = builder.Eq(d => d.LocationId, item);
                            }
                            else
                            {
                                filterDefineLocation = filterDefineLocation | builder.Eq(d => d.LocationId, item);
                            }

                        }
                    }

                    filterDefine = filterDefineLocation;
                }
                else
                {
                    if (!string.IsNullOrEmpty(locationId))
                    {
                        filterDefine = filterDefine & builder.Eq(d => d.LocationId, locationId);
                    }
                }

                if (!string.IsNullOrEmpty(serviceName))
                {
                    filterDefine = filterDefine & builder.Where(t => t.ServiceName.ToLower(CultureInfo.CurrentCulture).Contains(serviceName.ToLower(CultureInfo.CurrentCulture), StringComparison.CurrentCulture));

                }
                var Result = new List<DogServices>();

                if (filterDefine == FilterDefinition<DogServices>.Empty)
                {
                    return Result;
                }


                if (fields == null)
                {
                    Result = await context.DogServices
                                 .Find(filterDefine)
                                 .ToListAsync().ConfigureAwait(false);
                }
                else
                {
                    var fieldsName = _commonRepository.GetFieldsName(fields);
                    if (fieldsName.Any(str => str.Contains("serviceFee", StringComparison.CurrentCulture)))
                    {
                        fieldsName.Add("serviceFees");
                    }
                    if (fieldsName.Any(str => str.Contains("homeServiceFee", StringComparison.CurrentCulture)))
                    {
                        fieldsName.Add("homeServiceFees");
                    }
                    var fieldsBuilder = Builders<DogServices>.Projection;
                    var fieldsAdd = fieldsBuilder.Include(fieldsName[0]);
                    foreach (var item in fieldsName.Skip(1))
                    {
                        fieldsAdd = fieldsAdd.Include(item);
                    }

                    Result = await context.DogServices
                                 .Find(filterDefine)
                                  .Project<DogServices>(fieldsAdd)
                                 .ToListAsync().ConfigureAwait(false);
                }
                return Result;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.StackTrace);
            }
        }

        public async Task<DogServices> InsUpdDogServicesAsync(DogServices dogServices, CancellationToken cancellationToken)
        {
            try
            {
                if (dogServices == null) throw new ArgumentNullException(nameof(dogServices));
                if (string.IsNullOrEmpty(dogServices.ServiceId))
                {
                    var model = new List<ServiceFeesModel>();
                    if (dogServices.ServiceFee.StartPrice > 0)
                    {
                        model.Add(dogServices.ServiceFee);
                        dogServices.ServiceFees = model.ToArray();
                    }
                    if (dogServices.HomeServiceFee.StartPrice > 0)
                    {
                        model = new List<ServiceFeesModel>();
                        model.Add(dogServices.HomeServiceFee);
                        dogServices.HomeServiceFees = model.ToArray();
                    }
                    await context.DogServices.InsertOneAsync(dogServices).ConfigureAwait(false);
                }
                else
                {
                    var DogServices = await GetDogServicesAsync(dogServices.ServiceId, "", "", "", null, cancellationToken).ConfigureAwait(false);
                    var queryBuilder = Builders<DogServices>.Filter;
                    var filterDefine = FilterDefinition<DogServices>.Empty;

                    var update = Builders<DogServices>.Update
                                       .Set(x => x.ServiceName, !string.IsNullOrEmpty(dogServices.ServiceName) ? dogServices.ServiceName : DogServices[0].ServiceName)
                                       .Set(x => x.ServiceDetails, !string.IsNullOrEmpty(dogServices.ServiceDetails) ? dogServices.ServiceDetails : DogServices[0].ServiceDetails)
                                       .Set(x => x.Remarks, !string.IsNullOrEmpty(dogServices.Remarks) ? dogServices.Remarks : DogServices[0].Remarks)
                                       .Set(x => x.isHomeServiceAv, dogServices.isHomeServiceAv);



                    if (DogServices[0].ServiceFee.StartPrice != dogServices.ServiceFee.StartPrice || DogServices[0].ServiceFee.EndPrice != dogServices.ServiceFee.EndPrice)
                    {
                        update = update.Push(x => x.ServiceFees, dogServices.ServiceFee);
                    }


                    //// another way of handling it
                    ////var filter = queryBuilder.Eq(document => document.Id, id) & queryBuilder.ElemMatch(document => document.Subjects, elemMatchBuilder.Ne(document => document.Id, subjectId));
                    //filterDefine = queryBuilder.Eq(d => d.ServiceId, dogServices.ServiceId);
                    //filterDefine = filterDefine & queryBuilder.ElemMatch(service => service.ServiceFees, x => x.StartPrice == dogServices.ServiceFee.StartPrice && x.EndPrice == dogServices.ServiceFee.EndPrice);
                    //if (context.DogServices.Find(filterDefine).CountDocuments() == 0)
                    //{
                    //    update = update.Push(x => x.ServiceFees, dogServices.ServiceFee);

                    //}



                    if (dogServices.isHomeServiceAv)
                    {

                        if (DogServices[0].HomeServiceFee.StartPrice != dogServices.HomeServiceFee.StartPrice || DogServices[0].HomeServiceFee.EndPrice != dogServices.HomeServiceFee.EndPrice)
                        {
                            update = update.Push(x => x.HomeServiceFees, dogServices.HomeServiceFee);
                        }

                        //filterDefine = FilterDefinition<DogServices>.Empty;
                        //filterDefine = queryBuilder.Eq(d => d.ServiceId, dogServices.ServiceId);
                        //filterDefine = filterDefine & queryBuilder.ElemMatch(service => service.HomeServiceFees, x => x.StartPrice == dogServices.HomeServiceFee.StartPrice && x.EndPrice == dogServices.HomeServiceFee.EndPrice);
                        //if (context.DogServices.Find(filterDefine).CountDocuments() == 0)
                        //{
                        //    update = update.Push(x => x.HomeServiceFees, dogServices.HomeServiceFee);
                        //}

                    }
                    await context.DogServices.UpdateOneAsync(t => t.ServiceId == dogServices.ServiceId, update).ConfigureAwait(false);


                }
                return dogServices;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<bool> DeleteDogServicesAsync(string serviceId, CancellationToken cancellationToken)
        {
            try
            {
                await context.DogServices.DeleteOneAsync(
                        Builders<DogServices>.Filter.Eq("ServiceId", serviceId)).ConfigureAwait(false);

                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion



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

        #region userlocationmedia
        public async Task<List<UserLocationMedia>> GetUserLocationMediaAsync(string mediaId, string locationId, string userId, CancellationToken cancellationToken)
        {
            try
            {
                var builder = Builders<UserLocationMedia>.Filter;
                var filterDefine = FilterDefinition<UserLocationMedia>.Empty;

                if (!string.IsNullOrEmpty(locationId))
                {
                    filterDefine = builder.Eq(d => d.LocationId, locationId);
                }
                if (!string.IsNullOrEmpty(mediaId))
                {
                    filterDefine = builder.Eq(d => d.MediaId, mediaId);
                }
                if (!string.IsNullOrEmpty(userId))
                {
                    filterDefine = filterDefine & builder.Eq(d => d.UserId, userId);
                }
                var Result = await context.UserLocationMedia
                                  .Find(filterDefine)
                                  .ToListAsync().ConfigureAwait(false);

                Result.Select(c =>
                {
                    c.MediaPath = string.IsNullOrEmpty(c.MediaPath) ? c.MediaPath :
                     MediaUrl + c.MediaPath; return c;
                }).ToList();

                return Result;
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }
        }


        public async Task<UserLocationMedia> InsUpdUserLocationMediaAsync(UserLocationMedia userLocationMedia, CancellationToken cancellationToken)
        {
            try
            {
                if (userLocationMedia == null) throw new ArgumentNullException(nameof(userLocationMedia));


                if (string.IsNullOrEmpty(userLocationMedia.MediaId))
                {
                    await context.UserLocationMedia.InsertOneAsync(userLocationMedia).ConfigureAwait(false);
                }
                else
                {
                    await context.UserLocationMedia.ReplaceOneAsync(t => t.MediaId == userLocationMedia.MediaId, userLocationMedia).ConfigureAwait(false);
                }

                return userLocationMedia;
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }
        }


        //new for upload image
        public async Task<UserLocationMedia> InsUpdUserProfilePicAsync(UserLocationMedia userLocationMedia, CancellationToken cancellationToken)
        {
            try
            {

                if (userLocationMedia == null) throw new ArgumentNullException(nameof(userLocationMedia));

                var builder = Builders<UserLocationMedia>.Filter;
                var filterDefine = FilterDefinition<UserLocationMedia>.Empty;


                filterDefine = builder.Eq(d => d.IsProfilePic, true) & builder.Eq(d => d.UserId, userLocationMedia.UserId);

                var Result = await context.UserLocationMedia
                                  .Find(filterDefine)
                                  .ToListAsync().ConfigureAwait(false);


                if (Result.Count > 0)
                {
                    userLocationMedia.MediaId = Result[0].MediaId;
                    await context.UserLocationMedia.ReplaceOneAsync(t => t.MediaId == userLocationMedia.MediaId, userLocationMedia).ConfigureAwait(false);
                }
                else
                {
                    await context.UserLocationMedia.InsertOneAsync(userLocationMedia).ConfigureAwait(false);
                }

                return userLocationMedia;
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }
        }



        public async Task<bool> UpdProfilePicBitAsync(string mediaId, string userId, CancellationToken cancellationToken)
        {
            try
            {


                var builder = Builders<UserLocationMedia>.Filter;
                var filterDefine = FilterDefinition<UserLocationMedia>.Empty;


                filterDefine = builder.Eq(d => d.IsProfilePic, true) & builder.Eq(d => d.UserId, userId);

                var Result = await context.UserLocationMedia
                                  .Find(filterDefine)
                                  .ToListAsync().ConfigureAwait(false);



                var update = Builders<UserLocationMedia>.Update.Set(x => x.IsProfilePic, false);
                var filter = Builders<UserLocationMedia>.Filter.Eq(x => x.UserId, userId);

                var result = await context.UserLocationMedia.UpdateManyAsync(filter, update).ConfigureAwait(false);

                //set profilepic true
                update = null;
                filter = null;
                update = Builders<UserLocationMedia>.Update.Set(x => x.IsProfilePic, true);
                filter = Builders<UserLocationMedia>.Filter.Eq(x => x.MediaId, mediaId);

                result = await context.UserLocationMedia.UpdateOneAsync(filter, update).ConfigureAwait(false);

                if (result.IsAcknowledged && result.ModifiedCount > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }
        }




        public async Task<bool> DeleteUserLocationMediaAsync(string mediaId, CancellationToken cancellationToken)
        {
            try
            {
                await context.UserLocationMedia.DeleteOneAsync(
                        Builders<UserLocationMedia>.Filter.Eq("MediaId", mediaId)).ConfigureAwait(false);

                return true;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);

            }
        }



        #endregion

    }
}


