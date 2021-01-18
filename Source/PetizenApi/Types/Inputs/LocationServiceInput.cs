using GraphQL.Types;

namespace PetizenApi.Types.Inputs
{
    public class UserLocationInput : InputObjectGraphType
    {
        public UserLocationInput()
        {
            Name = "userLocationInput";
            Field<NonNullGraphType<StringGraphType>>("locationId");
            Field<NonNullGraphType<StringGraphType>>("userId");
            Field<NonNullGraphType<StringGraphType>>("roleId");
            Field<StringGraphType>("name");
            Field<StringGraphType>("locationOne");
            Field<FloatGraphType>("latitudeOne");
            Field<FloatGraphType>("longitudeOne");
            Field<ListGraphType<StringGraphType>>("locationTwo");
            Field<ListGraphType<FloatGraphType>>("latitudeTwo");
            Field<ListGraphType<FloatGraphType>>("longitudeTwo");
            Field<StringGraphType>("insertedBy");
        }
    }

    public class DogServicesInput : InputObjectGraphType
    {
        public DogServicesInput()
        {
            Name = "dogServicesInput";
            Field<NonNullGraphType<StringGraphType>>("serviceId");
            Field<NonNullGraphType<StringGraphType>>("locationId");
            Field<StringGraphType>("serviceName");
            Field<StringGraphType>("serviceDetails");
            Field<StringGraphType>("remarks");
            Field<ServiceFeesInput>("serviceFee");
            Field<BooleanGraphType>("isHomeServiceAv");
            Field<ServiceFeesInput>("homeServiceFee");
            Field<StringGraphType>("insertedBy");
        }
    }

    public class ServiceFeesInput : InputObjectGraphType
    {
        public ServiceFeesInput()
        {
            Field<DecimalGraphType>("startPrice");
            Field<DecimalGraphType>("endPrice");
        }

    }


    public class UserLocationMediaInput : InputObjectGraphType
    {
        public UserLocationMediaInput()
        {
            Name = "userLocationMediaInput";
            Field<StringGraphType>("mediaId");
            Field<StringGraphType>("locationId");
            Field<StringGraphType>("userId");
            Field<StringGraphType>("mediaTitle");
            Field<BooleanGraphType>("isProfilePic");
            Field<StringGraphType>("mediaPath");
            Field<StringGraphType>("insertedBy");

        }
    }

}
