using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;


namespace PetizenApi.Models
{

    public class UserConversation
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ConvoId { get; set; }
        public string UserId1 { get; set; }
        public string UserId2 { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public bool IsDeletedForUser1 { get; set; } = false;
        public bool IsDeletedForUser2 { get; set; } = false;

    }

    public class ConversationMessage
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ConvoMsgId { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string ConvoId { get; set; } // foreign key
        public string FromUserId { get; set; }
        public string ToUserId { get; set; }
        public string Message { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string MediaPath { get; set; }
        public bool IsSeen { get; set; } = false;
        public DateTime SeenOn { get; set; }
        public bool DeleteFrom { get; set; } = false;
        public bool DeleteTo { get; set; } = false;
    }


    public class UserGroups
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string GroupId { get; set; }
        public string GroupName { get; set; }
        public string CreatedBy { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string GroupIcon { get; set; }

    }

    public class GroupMembers
    {
        private readonly List<GroupMessages> groupMessages = new List<GroupMessages>();

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string MemberId { get; set; }
        public string GroupId { get; set; }
        public string UserId { get; set; }
        public bool IsDeleted { get; set; } = false;
        public bool IsAdmin { get; set; } = false;
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime InsertedDate { get; set; } = DateTime.Now;


        public List<GroupMessages> GroupMessages
        {
            get { return GroupMessages != null ? groupMessages.Where(c => c.GroupId == GroupId && c.CreatedDate >= InsertedDate).ToList() : new List<GroupMessages>(); }
            //set => groupMessages = value;
        }


    }

    public class GroupMessages
    {

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string GroupMsgId { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string GroupId { get; set; }
        public string UserId { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string Message { get; set; }
        public string MediaPath { get; set; }
        public int MsgType { get; set; } = 1; //1=msg,2=lbl
        public List<UserAction> DeletedUsers { get; set; }//
        public List<UserAction> SeenUsers { get; set; }//  set; 

    }

    public class UserAction
    {
        public string UserId { get; set; }
        public DateTime Date { get; set; }
    }

   


}
