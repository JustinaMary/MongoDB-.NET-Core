using GraphQL.Types;

namespace PetizenApi.Types.Inputs
{
    public class DogBreedsInput : InputObjectGraphType
    {
        public DogBreedsInput()
        {
            Name = "dogBreedsInput";
            Field<NonNullGraphType<StringGraphType>>("breedId");
            Field<NonNullGraphType<StringGraphType>>("breedName");
            Field<StringGraphType>("groupName");
            Field<StringGraphType>("description");
            Field<StringGraphType>("imagePath");
            Field<StringGraphType>("insertedBy");

        }
    }

    public class DogMasterInput : InputObjectGraphType
    {
        public DogMasterInput()
        {
            Name = "dogMasterInput";
            Field<NonNullGraphType<StringGraphType>>("dogId");
            Field<NonNullGraphType<StringGraphType>>("breedId");
            Field<StringGraphType>("lineage");
            Field<StringGraphType>("ownerId");
            Field<StringGraphType>("name");
            Field<StringGraphType>("gender");
            Field<DateTimeGraphType>("dob");
            Field<StringGraphType>("color");
            Field<StringGraphType>("height");
            Field<StringGraphType>("weight");
            Field<IntGraphType>("timesMated");
            Field<StringGraphType>("summary");
            Field<BooleanGraphType>("inHeat");
            Field<StringGraphType>("insertedBy");

        }
    }


    public class DogMediaInput : InputObjectGraphType
    {
        public DogMediaInput()
        {
            Name = "dogMediaInput";
            Field<StringGraphType>("mediaId");
            Field<StringGraphType>("dogId");
            Field<IntGraphType>("fileType");
            Field<IntGraphType>("mediaType");
            Field<StringGraphType>("mediaPath");

        }
    }

    public class FavouriteDogInput : InputObjectGraphType
    {
        public FavouriteDogInput()
        {
            Name = "favouriteDogInput";
            Field<NonNullGraphType<StringGraphType>>("favId");
            Field<NonNullGraphType<StringGraphType>>("userId");
            Field<StringGraphType>("dogId");
            Field<StringGraphType>("favForDogId");

        }
    }

    public class DogCharacteristicsInput : InputObjectGraphType
    {
        public DogCharacteristicsInput()
        {
            Name = "dogCharacteristicsInput";
            Field<StringGraphType>("characterId");
            Field<StringGraphType>("description");

        }
    }

    public class DogCoursesInput : InputObjectGraphType
    {
        public DogCoursesInput()
        {
            Name = "dogCoursesInput";
            Field<NonNullGraphType<StringGraphType>>("courseId");
            Field<NonNullGraphType<StringGraphType>>("userId");
            Field<StringGraphType>("title");
            Field<StringGraphType>("description");
            Field<StringGraphType>("imagePath");
            Field<IntGraphType>("amount");
            //Field<StringGraphType>("duration");
            Field<StringGraphType>("insertedBy");
        }
    }
}
