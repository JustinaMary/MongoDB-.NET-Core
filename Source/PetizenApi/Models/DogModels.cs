using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "<Pending>")]
namespace PetizenApi.Models
{
    public enum DogGroup
    {
        [Display(Name = "Sporting")]
        Sporting,

        [Display(Name = "Hound")]
        Hound,

        [Display(Name = "Working")]
        Working,

        [Display(Name = "Terrier")]
        Terrier,

        [Display(Name = "Toy")]
        Toy,

        [Display(Name = "Non Sporting")]
        NonSporting,

        [Display(Name = "Herding")]
        Herding
    }

    public enum Lineage
    {
        [Display(Name = "Pedigree")]
        Pedigree,

        [Display(Name = "Cross Breed")]
        CrossBreed,

        [Display(Name = "Mixed Breed")]
        MixedBreed
    }

    public enum MediaType
    {
        [Display(Name = "Image")]
        Image = 1,

        [Display(Name = "Video")]
        Video = 2,

    }

    public enum FileType
    {
        [Display(Name = "Upload")]
        Upload = 1,

        [Display(Name = "Link")]
        Link = 2,
    }

    public enum DaysOfWeek
    {
        [Display(Name = "Monday")]
        Monday = 1,

        [Display(Name = "Tuesday")]
        Tuesday = 2,

        [Display(Name = "Wednesday")]
        Wednesday = 3,

        [Display(Name = "Thursday")]
        Thursday = 4,

        [Display(Name = "Friday")]
        Friday = 5,

        [Display(Name = "Saturday")]
        Saturday = 6,

        [Display(Name = "Sunday")]
        Sunday = 7
    }
    public class DogBreeds
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string BreedId { get; set; }

        public string BreedName { get; set; } = "";

        public string GroupName { get; set; } = "";

        public string Description { get; set; } = "";

        public string ImagePath { get; set; } = "";

        public string InsertedBy { get; set; } = "";

        public DateTime InsertedDate { get; set; } = DateTime.Now;
    }

    public class BreedsByTextProjection : DogBreeds
    {
        [BsonElement("score")]
        public double Score { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class DogMaster
    {

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string DogId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string BreedId { get; set; } = "";
        public string Lineage { get; set; } = "";
        public string OwnerId { get; set; } = "";
        public string Name { get; set; } = "";
        public string Gender { get; set; } = "";
        public DateTime DOB { get; set; }
        public string Color { get; set; } = "";
        public string Height { get; set; } = "";
        public string Weight { get; set; } = "";
        public int TimesMated { get; set; }
        public string Summary { get; set; } = "";
        public bool InHeat { get; set; }
        public string InsertedBy { get; set; } = "";
        public DateTime InsertedDate { get; set; } = DateTime.Now;
        public string UpdatedBy { get; set; } = "";
        public DateTime UpdatedDate { get; set; }
        private List<DogMedia> dogMediaList;
        private List<DogBreeds> dogBreedInfo;
        private List<UserMaster> dogOwnerInfo;

        public List<DogMedia> DogMediaList
        {
            get { return dogMediaList != null ? dogMediaList : new List<DogMedia>(); }
            set => dogMediaList = value;
        }

        public List<DogBreeds> DogBreedInfo
        {
            get { return dogBreedInfo != null ? dogBreedInfo : new List<DogBreeds>(); }
            set => dogBreedInfo = value;
        }

        public List<UserMaster> DogOwnerInfo
        {
            get { return dogOwnerInfo != null ? dogOwnerInfo : new List<UserMaster>(); }
            set => dogOwnerInfo = value;
        }

    }

    public class DogMedia
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string MediaId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string DogId { get; set; } = "";

        public int MediaType { get; set; }

        public int FileType { get; set; }

        public string MediaPath { get; set; } = "";

        public DateTime InsertedDate { get; set; } = DateTime.Now;

    }

    public class FavouriteDog
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string FavId { get; set; }

        public string UserId { get; set; } = "";

        [BsonRepresentation(BsonType.ObjectId)]
        public string DogId { get; set; } = "";

        public DateTime InsertedDate { get; set; } = DateTime.Now;
    }

    public class DogCharacteristics
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string CharacterId { get; set; }

        public string Description { get; set; }

        public DateTime InsertedDate { get; set; } = DateTime.Now;
    }

    public class DogCourses
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string CourseId { get; set; }

        public string UserId { get; set; } = "";//trainer Id

        public string Title { get; set; } = "";

        public string Description { get; set; } = "";

        public long Amount { get; set; }

        public string ImagePath { get; set; } = "";

        public string InsertedBy { get; set; } = "";

        public DateTime InsertedDate { get; set; } = DateTime.Now;


    }
}
