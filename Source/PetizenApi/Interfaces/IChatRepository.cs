using GraphQL.Language.AST;
using PetizenApi.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PetizenApi.Interfaces
{
    public interface IChatRepository
    {

        //Private Chat
        Task<List<UserConversation>> GetMyConvoListAsync(string UserId, IDictionary<string, Field> fields, CancellationToken cancellationToken);//--order by desc
        //Task<IList<ConversationInfo>> GetSuggestedUserList(string UserId);//--order by desc
        Task<long> GetUnreadMessageCountAsync(string UserId, CancellationToken cancellationToken);
        Task<List<ConversationMessage>> GetAllMessagesAsync(string ConvoId, string UserId, CancellationToken cancellationToken);

        Task<ConversationMessage> AddPrivateMessageAsync(ConversationMessage ConvoMessageObj, CancellationToken cancellationToken);
        Task<bool> RemoveSingleConvoMessageAsync(string ConvoMsgId, string UserId, CancellationToken cancellationToken);
        Task<bool> RemoveAllConvoMessageAsync(string ConvoId, string UserId, CancellationToken cancellationToken);
        Task<bool> RemoveConvoAsync(string ConvoId, string UserId, CancellationToken cancellationToken);

        ////GroupChat

        Task<List<UserGroups>> GetMyGroupListAsync(string UserId, IDictionary<string, Field> Fields, CancellationToken cancellationToken);//--order by desc
        //Task<IList<GroupInfo>> GetAllGroupList(string UserId, IDictionary<string, Field> Fields);//--order by desc
        Task<UserGroups> GetGroupInfoAsync(string GroupId, IDictionary<string, Field> Fields, CancellationToken cancellationToken);//--order by desc
        Task<List<GroupMessages>> GetAllGroupMessageAsync(string UserId, string GroupId, IDictionary<string, Field> Fields, CancellationToken cancellationToken);//--order by desc

        Task<UserGroups> CreateUpdateGroupAsync(UserGroups GroupInfoObj, CancellationToken cancellationToken);
        Task<bool> JoinGroupAsync(string AddedBy, string UserId, string GroupId, CancellationToken cancellationToken);
        Task<bool> LeaveGroupAsync(string RemovedBy, string UserId, string GroupId, CancellationToken cancellationToken);
        Task<GroupMessages> AddGroupMessageAsync(GroupMessages GroupMsgObj, CancellationToken cancellationToken);
        Task<bool> RemoveSingleGroupMessageAsync(string GroupMsgId, string UserId, CancellationToken cancellationToken);
        Task<bool> RemoveAllGroupMessageAsync(string GroupId, string UserId, CancellationToken cancellationToken);

    }
}
