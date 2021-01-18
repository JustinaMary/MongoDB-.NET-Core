using GraphQL.Language.AST;
using PetizenApi.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PetizenApi.Interfaces
{
    public interface ILocationServiceRepository
    {
        //user location

        Task<List<UserLocation>> GetUserLocationAsync(string locationId, string userId, string roleId, CancellationToken cancellationToken);

        Task<UserLocation> InsUpdUserLocationAsync(UserLocation userLocation, CancellationToken cancellationToken);

        Task<bool> DeleteUserLocationAsync(string locationId, CancellationToken cancellationToken);

        Task<bool> DeleteSingleLocationAsync(string locationId, string address, CancellationToken cancellationToken);


        Task<List<UserLocationMedia>> GetUserLocationMediaAsync(string mediaId, string locationId, string userId, CancellationToken cancellationToken);


        Task<bool> DeleteUserLocationMediaAsync(string mediaId, CancellationToken cancellationToken);


        Task<UserLocationMedia> InsUpdUserLocationMediaAsync(UserLocationMedia userLocationMedia, CancellationToken cancellationToken);
        Task<UserLocationMedia> InsUpdUserProfilePicAsync(UserLocationMedia userLocationMedia, CancellationToken cancellationToken);
        Task<bool> UpdProfilePicBitAsync(string mediaId, string userId, CancellationToken cancellationToken);

        //user search
        Task<List<UserMaster>> SearchUserAsync(string role, string latitude, string longitude, int distance, IDictionary<string, Field> fields, CancellationToken cancellationToken);



        //dog services

        Task<List<DogServices>> GetDogServicesAsync(string serviceId, string locationId, string serviceName, string userId, IDictionary<string, Field> fields, CancellationToken cancellationToken);

        Task<DogServices> InsUpdDogServicesAsync(DogServices dogServices, CancellationToken cancellationToken);

        Task<bool> DeleteDogServicesAsync(string serviceId, CancellationToken cancellationToken);
    }
}
