using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.GeoJsonObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PetizenApi.Models
{
    public class UserLocation
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string LocationId { get; set; }

        public string UserId { get; set; } = "";

        public string RoleId { get; set; } = "";

        public string Name { get; set; } = "";  //like clinic name or SPA brand name

        public string LocationOne { get; set; } = "";

        public GeoJsonPoint<GeoJson2DGeographicCoordinates> Location1 { get; set; }

        public List<string> LocationTwo { get; set; } = new List<string>();

        public GeoJsonMultiPoint<GeoJson2DGeographicCoordinates> Location2 { get; set; }

        public string InsertedBy { get; set; } = "";

        public DateTime InsertedDate { get; set; } = DateTime.Now;

        [BsonIgnore]
        public double LatitudeOne { get; set; } = 0;
        [BsonIgnore]
        public double LongitudeOne { get; set; } = 0;
        [BsonIgnore]
        public List<double> LatitudeTwo { get; set; }
        [BsonIgnore]
        public List<double> LongitudeTwo { get; set; }

    }


    public class UserLocationMedia
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string MediaId { get; set; }

        public string LocationId { get; set; } = "";

        public string UserId { get; set; } = "";

        public string MediaTitle { get; set; } = "";

        public string MediaPath { get; set; } = "";

        public string InsertedBy { get; set; } = "";

        public bool IsProfilePic { get; set; } = false;

        public DateTime InsertedDate { get; set; } = DateTime.Now;

    }

    //document in MongoDB
    public class DogServices
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ServiceId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string LocationId { get; set; }

        public string ServiceName { get; set; } = "";

        public string ServiceDetails { get; set; } = "";

        public string Remarks { get; set; } = "";

        public IEnumerable<ServiceFeesModel> ServiceFees { get; set; } = Array.Empty<ServiceFeesModel>();

        public bool isHomeServiceAv { get; set; }   //is home service available

        public IEnumerable<ServiceFeesModel> HomeServiceFees { get; set; } = Array.Empty<ServiceFeesModel>();

        public string InsertedBy { get; set; } = "";

        public DateTime InsertedDate { get; set; } = DateTime.Now;


        //only to get the data from client


        ServiceFeesModel _serviceFee;
        ServiceFeesModel _homeServiceFee;
        [BsonIgnore]
        public ServiceFeesModel ServiceFee
        {
            get { return ServiceFees.Any() ? ServiceFees.OrderByDescending(c => c.InsertedDate).FirstOrDefault() : _serviceFee != null ? this._serviceFee : new ServiceFeesModel(); }
            set { _serviceFee = value; }
        }
        [BsonIgnore]
        public ServiceFeesModel HomeServiceFee
        {
            get { return HomeServiceFees.Any() ? HomeServiceFees.OrderByDescending(c => c.InsertedDate).FirstOrDefault() : _homeServiceFee != null ? this._homeServiceFee : new ServiceFeesModel(); }
            set { _homeServiceFee = value; }
        }

    }

    public class ServiceFeesModel
    {
        public decimal StartPrice { get; set; }

        public decimal EndPrice { get; set; }

        public DateTime InsertedDate { get; set; } = DateTime.Now;

    }

}
