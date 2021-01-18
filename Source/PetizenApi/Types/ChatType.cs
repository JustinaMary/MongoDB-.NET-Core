using GraphQL.Types;
using PetizenApi.Models;


namespace PetizenApi.Types
{

    public class UserConversationType : ObjectGraphType<UserConversation>
    {
        public UserConversationType()
        {
            Field(f => f.ConvoId);
            Field(f => f.UserId1);
            Field(f => f.UserId2);
            Field(f => f.IsDeletedForUser1);
            Field(f => f.IsDeletedForUser2);

        }
    }

    public class ConversationMessageType : ObjectGraphType<ConversationMessage>
    {
        public ConversationMessageType()
        {
            Field(f => f.ConvoMsgId);
            Field(f => f.ConvoId);
            Field(f => f.FromUserId);
            Field(f => f.ToUserId);
            Field(f => f.Message);
            Field(f => f.MediaPath);
        }
    }
    public class UserGroupsType : ObjectGraphType<UserGroups>
    {
        public UserGroupsType()
        {
            Field(f => f.GroupId);
            Field(f => f.GroupName);
            Field(f => f.CreatedBy);
            Field(f => f.GroupIcon);
        }
    }
    public class GroupMessagesType : ObjectGraphType<GroupMessages>
    {
        public GroupMessagesType()
        {
            Field(f => f.GroupMsgId);
            Field(f => f.GroupId);
            Field(f => f.UserId);
            Field(f => f.Message);
            Field(f => f.MediaPath);
            Field(f => f.MsgType);
        }
    }






}
