using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace PetizenApi.Models
{
    public class Ticketing //ticketing change name.
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string TId { get; set; }

        public string LocationId { get; set; }//foreign key

        public int Day { get; set; }

        public string StartTime { get; set; } = "00:00";

        public string EndTime { get; set; } = "23:59";

        public string BreakStartTime { get; set; }

        public string BreakEndTime { get; set; }

        public string InsertedBy { get; set; } = "";

        public bool IsHoliday { get; set; } = false;

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime InsertedDate { get; set; } = DateTime.Now;

    }



}
