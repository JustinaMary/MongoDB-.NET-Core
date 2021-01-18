using GraphQL.Language.AST;
using PetizenApi.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PetizenApi.Interfaces
{
    public interface IDogRepository
    {

        //dog breeds

        Task<IList<DogBreeds>> GetDogBreedAsync(string breedId, string groupName, string breedName, IDictionary<string, Field> fields, CancellationToken cancellationToken);

        Task<DogBreeds> InsUpdDogBreedAsync(DogBreeds breeds, CancellationToken cancellationToken);

        Task<bool> DeleteDogBreedAsync(string breedId, CancellationToken cancellationToken);


        //dog master

        Task<List<DogMaster>> GetDogAsync(string dogId, string breedId, string ownerId, string gender, IDictionary<string, Field> fields, CancellationToken cancellationToken);

        Task<List<DogMaster>> GetDogForDetailsAsync(string ownerId, CancellationToken cancellationToken);
        Task<DogMaster> InsUpdDogAsync(DogMaster dog, CancellationToken cancellationToken);

        Task<bool> DeleteDogAsync(string dogId, CancellationToken cancellationToken);

        Task<List<DogMaster>> SearchDogAsync(string breedId, string gender, string latitude, string longitude, int distance, IDictionary<string, Field> fields, CancellationToken cancellationToken);


        //dog media

        Task<IEnumerable<DogMedia>> GetDogsMediaAsync(string mediaId, string dogId, CancellationToken cancellationToken);

        Task<DogMedia> InsUpdDogMediaAsync(DogMedia dogMedia, CancellationToken cancellationToken);

        Task<bool> DeleteDogMediaAsync(string mediaId, CancellationToken cancellationToken);


        //Dog Characteristics

        Task<List<DogCharacteristics>> GetDogCharacteristicsAsync(string characterId, string description, CancellationToken cancellationToken);

        Task<DogCharacteristics> InsUpdDogCharacteristicsAsync(DogCharacteristics characteristics, CancellationToken cancellationToken);

        Task<bool> DeleteDogCharacteristicsAsync(string characterId, CancellationToken cancellationToken);


        //Favourite Dog

        Task<IEnumerable<FavouriteDog>> GetFavouriteAsync(string UserId, string forDogId, CancellationToken cancellationToken);

        Task<FavouriteDog> InsUpdFavDogAsync(FavouriteDog favouriteDog, CancellationToken cancellationToken);

        Task<bool> DeleteFavDogAsync(string FavId, CancellationToken cancellationToken);


        //Dog Dog Courses for trainer

        Task<List<DogCourses>> GetDogCoursesAsync(string courseId, string userId, IDictionary<string, Field> fields, CancellationToken cancellationToken);

        Task<DogCourses> InsUpdDogCoursesAsync(DogCourses courses, CancellationToken cancellationToken);

        Task<bool> DeleteDogCoursesAsync(string courseId, CancellationToken cancellationToken);


    }
}
