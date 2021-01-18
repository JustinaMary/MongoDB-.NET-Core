using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace PetizenApi.Models
{
    public class Categories
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string CatId { get; set; }

        public string ParentId { get; set; }

        public string CategoryName { get; set; }

        public string ImagePath { get; set; }

        public bool showInFront { get; set; }

        public string InsertedBy { get; set; }

        public DateTime InsertedDate { get; set; } = DateTime.Now;
    }

    public class Brands
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string BrandId { get; set; }

        public string BrandName { get; set; }

        public string ImagePath { get; set; }

        public bool showInFront { get; set; }

        public string InsertedBy { get; set; }

        public DateTime InsertedDate { get; set; } = DateTime.Now;

    }

    public class DogProducts
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ProductId { get; set; }

        public string ProdName { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string BrandId { get; set; }


    }
}
