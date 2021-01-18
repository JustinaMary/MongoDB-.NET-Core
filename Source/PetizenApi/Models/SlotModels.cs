using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace PetizenApi.Models
{
    public class DayWiseSlot  //by default one user will have 7 slots
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string DayId { get; set; }

        public string UserId { get; set; } = "";

        public int Day { get; set; }

        public bool isClosed { get; set; }  //is closed is true it will not have further list of slots 

        public string InsertedBy { get; set; } = "";

        public DateTime InsertedDate { get; set; } = DateTime.Now;

    }

    public class DaySlotList
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string SlotId { get; set; }

        public string DayId { get; set; } = "";

        public string StartTime { get; set; } = "";

        public string EndTime { get; set; } = "";

        public decimal Fees { get; set; }

        public string InsertedBy { get; set; } = "";

        public DateTime InsertedDate { get; set; } = DateTime.Now;

    }

    public class SpecialDateSlot
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string SDId { get; set; }

        public string UserId { get; set; } = "";

        public DateTime Date { get; set; }

        public bool isClosed { get; set; }  //is closed is true it will not have further list of slots 

        public string InsertedBy { get; set; } = "";

        public DateTime InsertedDate { get; set; } = DateTime.Now;
    }

    public class SpecialDateSlotList
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string SlotId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string SDId { get; set; } = "";

        public string StartTime { get; set; } = "";

        public string EndTime { get; set; } = "";

        public decimal Fees { get; set; }

        public string InsertedBy { get; set; } = "";

        public DateTime InsertedDate { get; set; } = DateTime.Now;

    }


    public class SlotBooking
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string BookId { get; set; }

        public string BookedBy { get; set; } = "";

        [BsonRepresentation(BsonType.ObjectId)]
        public string DogId { get; set; } = "";

        public bool isSpecialDate { get; set; }

        public string SlotId { get; set; } = "";

        public decimal Fees { get; set; }

        public string InsertedBy { get; set; } = "";

        public DateTime InsertedDate { get; set; } = DateTime.Now;

    }


}
