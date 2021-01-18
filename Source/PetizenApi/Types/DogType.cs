using GraphQL.Types;
using PetizenApi.Models;

namespace PetizenApi.Types
{
    public class DogBreedsType : ObjectGraphType<DogBreeds>
    {
        public DogBreedsType()
        {
            Field(x => x.BreedId);
            Field(x => x.BreedName, type: typeof(StringGraphType));
            Field(x => x.GroupName, type: typeof(StringGraphType));
            Field(x => x.Description, type: typeof(StringGraphType));
            Field(x => x.ImagePath, type: typeof(StringGraphType));
            Field(x => x.InsertedDate);


        }
    }

    public class DogMasterType : ObjectGraphType<DogMaster>
    {
        public DogMasterType()
        {
            Field(x => x.DogId);
            Field(x => x.BreedId);
            Field(x => x.Lineage, type: typeof(StringGraphType));
            Field(x => x.OwnerId, type: typeof(StringGraphType));
            Field(x => x.Name, type: typeof(StringGraphType));
            Field(x => x.Gender, type: typeof(StringGraphType));
            Field(x => x.DOB);
            Field(x => x.Color, type: typeof(StringGraphType));
            Field(x => x.Height, type: typeof(StringGraphType));
            Field(x => x.Weight, type: typeof(StringGraphType));
            Field(x => x.TimesMated);
            Field(x => x.Summary, type: typeof(StringGraphType));
            Field(x => x.InHeat, type: typeof(BooleanGraphType));
            Field(x => x.InsertedBy, type: typeof(StringGraphType));
            Field(x => x.InsertedDate);
            Field(x => x.DogMediaList, type: typeof(ListGraphType<DogMediaType>));
            Field(x => x.DogBreedInfo, type: typeof(ListGraphType<DogBreedsType>));
            Field(x => x.DogOwnerInfo, type: typeof(ListGraphType<UserMasterType>));

            //Field<ListGraphType<UserMasterType>>("owner",
            //   arguments: new QueryArguments(new QueryArgument<StringGraphType> { Name = "userId" }),
            //   resolve: context => contextServiceLocator.AccountRepository.GetUserMasterAsync(0, context.Source.OwnerId, "", 0, context.CancellationToken), description: "Owner's Id");
        }
    }

    public class DogMediaType : ObjectGraphType<DogMedia>
    {
        public DogMediaType()
        {
            Field(x => x.MediaId);
            Field(x => x.DogId);
            Field(x => x.MediaType);
            Field(x => x.FileType);
            Field(x => x.MediaPath, type: typeof(StringGraphType));
            Field(x => x.InsertedDate);

        }
    }

    public class FavouriteDogType : ObjectGraphType<FavouriteDog>
    {
        public FavouriteDogType()
        {
            Field(x => x.FavId);
            Field(x => x.UserId);
            Field(x => x.DogId);
            Field(x => x.InsertedDate);

        }
    }


    public class DogCharacteristicsType : ObjectGraphType<DogCharacteristics>
    {
        public DogCharacteristicsType()
        {
            Field(x => x.CharacterId);
            Field(x => x.Description, type: typeof(StringGraphType));
            Field(x => x.InsertedDate);

        }
    }

    public class DogCoursesType : ObjectGraphType<DogCourses>
    {
        public DogCoursesType()
        {
            Field(x => x.CourseId);
            Field(x => x.UserId);
            Field(x => x.Title, type: typeof(StringGraphType));
            Field(x => x.Description, type: typeof(StringGraphType));
            Field(x => x.Amount);
            //Field(x => x.Duration);
            Field(x => x.ImagePath, type: typeof(StringGraphType));
            Field(x => x.InsertedBy);
        }
    }
}
