using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PetizenApi.Models
{
    public class MenuMaster
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string MenuId { get; set; }

        public int MenuType { get; set; }  //1 for Front-End 2 for Back-End

        public string MenuName { get; set; } = "";

        public string ParentId { get; set; } = "";

        public string MenuIcon { get; set; } = "";

        public string RouteLink { get; set; } = "";

        public string InsertedBy { get; set; } = "";

        public DateTime InsertedDate { get; set; } = DateTime.Now;

        public bool IsChild { get; set; } = false;

        private List<MenuAccess> menuAccess = new List<MenuAccess>();
        public List<MenuAccess> MenuAccess
        {
            get { return menuAccess != null ? menuAccess.OrderByDescending(c => c.InsertedDate).ToList() : new List<MenuAccess>(); }
            set => menuAccess = value;
        }


    }

    public class MenuAccess
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string AccessId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string MenuId { get; set; } = "";

        public string RoleId { get; set; } = "";

        public string InsertedBy { get; set; } = "";

        public DateTime InsertedDate { get; set; } = DateTime.Now;
    }


    public class GetRoleWiseMenu
    {
        public string RoleId { get; set; }

        public string RoleName { get; set; }

        public string MenuId { get; set; }

        public List<MenuAction> Menu { get; set; } //set
    }


    public class MenuAction
    {
        public string MenuId { get; set; }

        public string MenuName { get; set; }

        public bool isSelected { get; set; }
    }


    public class GetLoginWiseMenu
    {
        public string MenuId { get; set; }

        public string MenuName { get; set; }

        public string RouteLink { get; set; }
        public string RoleId { get; set; }

        public List<SubMenuList> SubMenu { get; set; }
    }


    public class SubMenuList
    {
        public string MenuId { get; set; }

        public string MenuName { get; set; }

        public string RouteLink { get; set; }
        public string RoleId { get; set; }

        public List<ChildMenuList> ChildMenu { get; set; }
    }


    public class ChildMenuList
    {
        public string MenuId { get; set; }

        public string MenuName { get; set; }

        public string RouteLink { get; set; }

    }



}
