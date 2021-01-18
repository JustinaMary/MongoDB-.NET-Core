using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using PetizenApi.Models;
using System;

namespace PetizenApi.Database
{
    public class MongoConnection
    {
        private readonly IMongoDatabase _database = null;
        private readonly IOptions<MongoSettings> _setting;

        public MongoConnection(IOptions<MongoSettings> settings)
        {

            var camelCaseConvention = new ConventionPack { new CamelCaseElementNameConvention() };
            ConventionRegistry.Register("CamelCase", camelCaseConvention, type => true);

            if (settings == null) throw new ArgumentNullException(nameof(settings));
            _setting = settings;
            var client = new MongoClient(_setting.Value.ConnectionString);

            if (client != null)

                _database = client.GetDatabase(settings.Value.Database);
        }

        public IMongoCollection<RefreshToken> RefreshToken
        {
            get
            {
                return _database.GetCollection<RefreshToken>("RefreshToken");
            }


        }

        public IMongoCollection<DogBreeds> DogBreeds
        {
            get
            {
                return _database.GetCollection<DogBreeds>("DogBreeds");
            }
        }

        public IMongoCollection<DogMaster> DogMaster
        {
            get
            {
                return _database.GetCollection<DogMaster>("DogMaster");
            }
        }

        public IMongoCollection<DogMedia> DogMedia
        {
            get
            {
                return _database.GetCollection<DogMedia>("DogMedia");
            }
        }

        public IMongoCollection<FavouriteDog> FavouriteDog
        {
            get
            {
                return _database.GetCollection<FavouriteDog>("FavouriteDog");
            }
        }



        public IMongoCollection<DogCharacteristics> DogCharacteristics
        {
            get
            {
                return _database.GetCollection<DogCharacteristics>("DogCharacteristics");
            }
        }


        public IMongoCollection<DogCourses> DogCourses
        {
            get
            {
                return _database.GetCollection<DogCourses>("DogCourses");
            }
        }

        public IMongoCollection<MenuMaster> MenuMaster
        {
            get
            {
                return _database.GetCollection<MenuMaster>("MenuMaster");
            }
        }

        public IMongoCollection<MenuAccess> MenuAccess
        {
            get
            {
                return _database.GetCollection<MenuAccess>("MenuAccess");
            }
        }

        public IMongoCollection<UserAddress> UserAddress
        {
            get
            {
                return _database.GetCollection<UserAddress>("UserAddress");
            }
        }

        public IMongoCollection<UserLocation> UserLocation
        {
            get
            {
                return _database.GetCollection<UserLocation>("UserLocation");
            }
        }

        public IMongoCollection<UserLocationMedia> UserLocationMedia
        {
            get
            {
                return _database.GetCollection<UserLocationMedia>("UserLocationMedia");
            }
        }



        public IMongoCollection<DogServices> DogServices
        {
            get
            {
                return _database.GetCollection<DogServices>("DogServices");
            }
        }

        public IMongoCollection<UserConversation> UserConversation
        {
            get
            {
                return _database.GetCollection<UserConversation>("UserConversation");
            }
        }

        public IMongoCollection<ConversationMessage> ConversationMessage
        {
            get
            {
                return _database.GetCollection<ConversationMessage>("ConversationMessage");
            }
        }
        public IMongoCollection<UserGroups> UserGroups
        {
            get
            {
                return _database.GetCollection<UserGroups>("UserGroups");
            }
        }
        public IMongoCollection<GroupMessages> GroupMessages
        {
            get
            {
                return _database.GetCollection<GroupMessages>("GroupMessages");
            }
        }
        public IMongoCollection<GroupMembers> GroupMembers
        {
            get
            {
                return _database.GetCollection<GroupMembers>("GroupMembers");
            }
        }


        public IMongoCollection<Ticketing> Ticketing
        {
            get
            {
                return _database.GetCollection<Ticketing>("Ticketing");
            }
        }


    }
}
