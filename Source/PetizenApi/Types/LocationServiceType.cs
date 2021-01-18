using GraphQL.Types;
using PetizenApi.Models;

namespace PetizenApi.Types
{
    public class UserLocationType : ObjectGraphType<UserLocation>
    {
        public UserLocationType()
        {
            Field(f => f.LocationId);
            Field(f => f.UserId);
            Field(f => f.RoleId);
            Field(f => f.Name);
            Field(f => f.LocationOne);
            Field(f => f.LatitudeOne);
            Field(f => f.LongitudeOne);
            Field(f => f.LocationTwo);
            Field(f => f.LatitudeTwo);
            Field(f => f.LongitudeTwo);
            Field(f => f.InsertedBy);
            Field(f => f.InsertedDate);
        }
    }

    public class DogServicesType : ObjectGraphType<DogServices>
    {
        public DogServicesType()
        {
            Field(f => f.ServiceId);
            Field(f => f.LocationId);
            Field(f => f.ServiceName);
            Field(f => f.ServiceDetails);
            Field(f => f.Remarks);
            Field(f => f.ServiceFee, type: typeof(ServiceFeesType));
            Field(f => f.isHomeServiceAv);
            Field(f => f.HomeServiceFee, type: typeof(ServiceFeesType));
        }
    }

    public class ServiceFeesType : ObjectGraphType<ServiceFeesModel>
    {
        public ServiceFeesType()
        {
            Field(f => f.StartPrice);
            Field(f => f.EndPrice);

        }
    }


    public class UserLocationMediaType : ObjectGraphType<UserLocationMedia>
    {
        public UserLocationMediaType()
        {
            Field(f => f.MediaId);
            Field(f => f.LocationId);
            Field(f => f.UserId);
            Field(f => f.MediaTitle);
            Field(f => f.IsProfilePic);
            Field(f => f.MediaPath);
            Field(f => f.InsertedBy);
            Field(f => f.InsertedDate);
        }
    }







}
