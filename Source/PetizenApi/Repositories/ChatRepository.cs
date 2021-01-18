using GraphQL.Language.AST;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using PetizenApi.Database;
using PetizenApi.Interfaces;
using PetizenApi.Models;
using PetizenApi.Providers;
using Serilog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PetizenApi.Repositories
{
    public class ChatRepository : IChatRepository 
    {
        private readonly MongoConnection context = null;
        private readonly string MediaUrl = "";
        private readonly ICommonRepository _commonRepository;

        public ChatRepository(IOptions<MongoSettings> settings, IOptions<ApplicationUrl> webUrl, ICommonRepository commonRepository)
        {
            context = new MongoConnection(settings);
            if (webUrl != null)
            {
                MediaUrl = webUrl.Value.MediaUrl.ToString();
            }
            else
            {
                throw new ArgumentNullException(nameof(webUrl));
            }
            _commonRepository = commonRepository;
        }

        #region Private Chat

        public async Task<List<UserConversation>> GetMyConvoListAsync(string UserId, IDictionary<string, Field> fields, CancellationToken cancellationToken)
        {
            try
            {
                var builder = Builders<UserConversation>.Filter;
                var filterDefine = FilterDefinition<UserConversation>.Empty;

                filterDefine = (builder.Eq(d => d.UserId1, UserId) & builder.Eq(d => d.IsDeletedForUser1, false));
                filterDefine = filterDefine | (builder.Eq(d => d.UserId2, UserId) & builder.Eq(d => d.IsDeletedForUser2, false));

                var Result = new List<UserConversation>();

                if (fields == null)
                {
                    Result = await context.UserConversation.Find(filterDefine).ToListAsync().ConfigureAwait(false);
                }
                else
                {
                    var fieldsName = _commonRepository.GetFieldsName(fields);
                    var fieldsBuilder = Builders<UserConversation>.Projection;

                    var fieldsAdd = fieldsBuilder.Include(fieldsName[0]);

                    foreach (var item in fieldsName.Skip(1))
                    {
                        fieldsAdd = fieldsAdd.Include(item);
                    }
                    Result = await context.UserConversation
                                     .Find(filterDefine)
                                    .Project<UserConversation>(fieldsAdd)
                                 .ToListAsync().ConfigureAwait(false);
                }

                return Result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception");
                throw;
            }
        }
        //will be done later  when user following followup.
        //public async Task<List<ConversationInfo>> GetSuggestedUserList(string UserId)
        //{
        //    try
        //    {

        //        return new List<ConversationInfo>();


        //    }
        //    catch (Exception e)
        //    {

        //        throw new Exception(e.Message);
        //    }
        //}

        public async Task<long> GetUnreadMessageCountAsync(string UserId, CancellationToken cancellationToken)
        {
            try
            {

                int unseencnt = context.ConversationMessage.Find(t => (t.ToUserId == UserId && !t.IsSeen && !t.DeleteTo)).ToList().Select(x => x.ConvoId).Distinct().Count();



                context.GroupMembers.Find(t => t.UserId == UserId && !t.IsDeleted).ToList();

                await context.GroupMembers.Aggregate()
                    .Match(Builders<GroupMembers>.Filter.Eq(d => d.UserId, UserId))
                    .Lookup(
                     context.GroupMessages,
                     m => m.GroupId,
                     c => c.GroupId,
                     (GroupMembers m) => m.GroupMessages)
                    .ToListAsync().ConfigureAwait(false);

                return unseencnt;


            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception");
                throw;
            }
        }


        public async Task<List<ConversationMessage>> GetAllMessagesAsync(string ConvoId, string UserId, CancellationToken cancellationToken)
        {
            try
            {
                //MarkSeenAllPrivateMessage(ConvoId, UserId);
                var Result = await context.ConversationMessage.Find(x => x.ConvoId == ConvoId && ((x.FromUserId == UserId && !x.DeleteFrom) || (x.ToUserId == UserId && !x.DeleteTo))).ToListAsync().ConfigureAwait(false);
                return Result;

            }
            catch (Exception e)
            {
                Log.Error(e, "Exception");
                throw;
              
            }
        }
        public async Task<ConversationMessage> AddPrivateMessageAsync(ConversationMessage ConvoMessageObj, CancellationToken cancellationToken)
        {
            try
            {
                if (ConvoMessageObj == null) throw new ArgumentNullException(nameof(ConvoMessageObj));

                if (string.IsNullOrEmpty(ConvoMessageObj.ConvoId))
                {
                    var conversationUpdate = Builders<UserConversation>.Update
                        .Set(x => x.UserId1, ConvoMessageObj.FromUserId)
                        .Set(x => x.UserId2, ConvoMessageObj.ToUserId)
                        .Set(x => x.CreatedDate, DateTime.Now)
                        .Set(x => x.IsDeletedForUser1, false)
                        .Set(x => x.IsDeletedForUser1, false);


                    var result = await context.UserConversation.UpdateOneAsync(t => (t.UserId1 == ConvoMessageObj.FromUserId && t.UserId2 == ConvoMessageObj.ToUserId) ||
                     (t.UserId1 == ConvoMessageObj.ToUserId && t.UserId2 == ConvoMessageObj.FromUserId),
                        conversationUpdate, new UpdateOptions { IsUpsert = true }).ConfigureAwait(false);
                    if (result.UpsertedId != null)
                    {
                        ConvoMessageObj.ConvoId = result.UpsertedId.ToString();
                    }
                    else
                    {
                        var Result = await context.UserConversation.Find(t => (t.UserId1 == ConvoMessageObj.FromUserId && t.UserId2 == ConvoMessageObj.ToUserId) ||
                     (t.UserId1 == ConvoMessageObj.ToUserId && t.UserId2 == ConvoMessageObj.FromUserId)).FirstOrDefaultAsync().ConfigureAwait(false);

                        ConvoMessageObj.ConvoId = Result.ConvoId;
                    }
                }

                await context.ConversationMessage.InsertOneAsync(ConvoMessageObj).ConfigureAwait(false);
                return ConvoMessageObj;
            }
            catch (Exception e)
            {
                Log.Error(e, "Exception");
                throw;
            }
        }

        public async Task<bool> RemoveSingleConvoMessageAsync(string ConvoMsgId, string UserId, CancellationToken cancellationToken)
        {

            try
            {
                //only row updated by either 1 query
                var update = Builders<ConversationMessage>.Update.Set(x => x.DeleteTo, true);

                var filter = Builders<ConversationMessage>.Filter.And(
                    Builders<ConversationMessage>.Filter.Eq(x => x.ConvoMsgId, ConvoMsgId),
                    Builders<ConversationMessage>.Filter.Eq(x => x.ToUserId, UserId)//,
                    );
                var result = await context.ConversationMessage.UpdateOneAsync(filter, update).ConfigureAwait(false);

                if (result.IsAcknowledged && result.ModifiedCount == 0)
                {

                    update = Builders<ConversationMessage>.Update.Set(x => x.DeleteFrom, true);

                    filter = Builders<ConversationMessage>.Filter.And(
                    Builders<ConversationMessage>.Filter.Eq(x => x.ConvoMsgId, ConvoMsgId),
                    Builders<ConversationMessage>.Filter.Eq(x => x.FromUserId, UserId)//,
                    );
                    result = await context.ConversationMessage.UpdateOneAsync(filter, update).ConfigureAwait(false);

                }

                return result.IsAcknowledged && result.ModifiedCount > 0;


            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception");
                throw;
            }

        }

        public async Task<bool> RemoveAllConvoMessageAsync(string ConvoId, string UserId, CancellationToken cancellationToken)
        {

            try
            {
               

                var update = Builders<ConversationMessage>.Update.Set(x => x.DeleteTo, true);

                var filter = Builders<ConversationMessage>.Filter.And(
                    Builders<ConversationMessage>.Filter.Eq(x => x.ConvoId, ConvoId),
                    Builders<ConversationMessage>.Filter.Eq(x => x.ToUserId, UserId)//,
                    );
                var result = await context.ConversationMessage.UpdateManyAsync(filter, update).ConfigureAwait(false);

                if (result.IsAcknowledged && result.ModifiedCount == 0)
                {
                    update = Builders<ConversationMessage>.Update.Set(x => x.DeleteFrom, true);

                    filter = Builders<ConversationMessage>.Filter.And(
                    Builders<ConversationMessage>.Filter.Eq(x => x.ConvoId, ConvoId),
                    Builders<ConversationMessage>.Filter.Eq(x => x.FromUserId, UserId)//,
                    );
                    await context.ConversationMessage.UpdateManyAsync(filter, update).ConfigureAwait(false);

                }
               
                return true;

            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception");
                throw;
            }
        }


        public async Task<bool> RemoveConvoAsync(string ConvoId, string UserId, CancellationToken cancellationToken)
        {
            try
            {
                //to delete from ConversationInfo
                var update = Builders<UserConversation>.Update.Set(x => x.IsDeletedForUser1, true);
                var result = await context.UserConversation.UpdateOneAsync(t => t.ConvoId == ConvoId && t.UserId1 == UserId, update).ConfigureAwait(false);

                update = null;
                result = null;

                update = Builders<UserConversation>.Update.Set(x => x.IsDeletedForUser2, true);
                result = await context.UserConversation.UpdateOneAsync(t => t.ConvoId == ConvoId && t.UserId2 == UserId, update).ConfigureAwait(false);

                //to delete from ConversationMessage
                await RemoveAllConvoMessageAsync(ConvoId, UserId, cancellationToken).ConfigureAwait(false);

                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception");
                throw;
            }

        }

        public async Task<bool> MarkSeenAllPrivateMessageAsync(List<string> ConvoMsgIds, string UserId, CancellationToken cancellationToken)
        {

            try
            {
                //var allConvoMessages = context.ConversationMessage.Find(t => t.ConvoId == ConvoId).ToList();
                if (ConvoMsgIds == null) throw new ArgumentNullException(nameof(ConvoMsgIds));

                foreach (var item in ConvoMsgIds)
                {


                    var update = Builders<ConversationMessage>.Update.Combine(
                      Builders<ConversationMessage>.Update.Set(x => x.IsSeen, true),
                      Builders<ConversationMessage>.Update.Set(x => x.SeenOn, DateTime.Now)
                      );

                    var filter = Builders<ConversationMessage>.Filter.And(
                        Builders<ConversationMessage>.Filter.Eq(x => x.ConvoMsgId, item),
                        Builders<ConversationMessage>.Filter.Eq(x => x.ToUserId, UserId)//,
                        );
                    var result = await context.ConversationMessage.UpdateOneAsync(filter, update, null, cancellationToken).ConfigureAwait(false);

                    if (result.IsAcknowledged && result.ModifiedCount == 0)
                    {
                        filter = Builders<ConversationMessage>.Filter.And(
                        Builders<ConversationMessage>.Filter.Eq(x => x.ConvoMsgId, item),
                        Builders<ConversationMessage>.Filter.Eq(x => x.FromUserId, UserId)//,
                        );
                        await context.ConversationMessage.UpdateOneAsync(filter, update, null, cancellationToken).ConfigureAwait(false);

                    }
                    
                }
                return true;

            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception");
                throw;
            }
        }



        #endregion

        #region Group chat


        public async Task<List<UserGroups>> GetMyGroupListAsync(string UserId, IDictionary<string, Field> Fields, CancellationToken cancellationToken)//--order by desc
        {
            try
            {
                var builder = Builders<GroupMembers>.Filter;
                var filterDefine = FilterDefinition<GroupMembers>.Empty;

                filterDefine = (builder.Eq(d => d.UserId, UserId) & builder.Eq(d => d.IsDeleted, false));

                var Result = new List<GroupMembers>();
                Result = await context.GroupMembers.Find(filterDefine).ToListAsync().ConfigureAwait(false);

                var GroupInfoList = context.UserGroups.Find(a => Result.Select(y => y.GroupId).ToList().Contains(a.GroupId)).ToList();

                return GroupInfoList;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception");
                throw;
            }
        }


        //public Task<List<GroupInfo>> GetAllGroupList(string UserId, Field Fields)//--order by desc
        //{
        //    try
        //    {
        //        List<GroupInfoVM> groupList = new List<GroupInfoVM>();
        //        string baseurl = GetApplicationUrl();
        //        if (UserId != Guid.Empty)
        //        {
        //            groupList = _chatRepository.GetGroupList(UserId);

        //            groupList.Select(c => { c.GroupIcon = string.IsNullOrEmpty(c.GroupIcon) ? "" : baseurl + "/Media/" + c.GroupIcon; return c; }).ToList();
        //        }

        //        Result.Select(c =>
        //        {
        //            c.MediaUrl = string.IsNullOrEmpty(c.MediaUrl) ? c.MediaUrl :
        //            c.MediaUrl.IndexOf("http") > -1 ? c.MediaUrl : MediaUrl + "/Media/" + c.MediaUrl; return c;
        //        }).ToList();



        //    }
        //    catch (Exception ex)
        //    {

        //        throw new Exception(ex.Message);
        //    }
        //}



        public async Task<UserGroups> GetGroupInfoAsync(string GroupId, IDictionary<string, Field> Fields, CancellationToken cancellationToken)//--order by desc
        {
            try
            {
                var builder = Builders<UserGroups>.Filter;
                var filterDefine = FilterDefinition<UserGroups>.Empty;

                filterDefine = (builder.Eq(d => d.GroupId, GroupId));

                var Result = new UserGroups();
                Result = await context.UserGroups.Find(filterDefine).FirstOrDefaultAsync().ConfigureAwait(false);

                return Result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception");
                throw;
            }

        }
        //--order by desc
        public async Task<List<GroupMessages>> GetAllGroupMessageAsync(string UserId, string GroupId, IDictionary<string, Field> Fields, CancellationToken cancellationToken)
        {
            try
            {
                //incomplete Jinal

                var filterGroup = Builders<GroupMessages>.Filter.And(
                      Builders<GroupMessages>.Filter.Eq(x => x.GroupId, GroupId)//,
                                                                                //Builders<GroupMessages>.Filter.ElemMatch(x => x.SeenUsers, x => x.UserId == UserId && x.IsDeleted == false)
                      );


                await context.GroupMessages.Find(filterGroup).ToListAsync().ConfigureAwait(false);

                //update is seen for all messages
                //MarkSeenAllGroupMessage(GroupId, UserId);

                return new List<GroupMessages>();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception");
                throw;
            }
        }

        public async Task<UserGroups> CreateUpdateGroupAsync(UserGroups GroupInfoObj, CancellationToken cancellationToken)
        {
            try
            {
                if (GroupInfoObj == null) throw new ArgumentNullException(nameof(GroupInfoObj));


                if (string.IsNullOrEmpty(GroupInfoObj.GroupId))
                {
                    await context.UserGroups.InsertOneAsync(GroupInfoObj).ConfigureAwait(false);
                }
                else
                {
                    var dbGroupInfo = await context.UserGroups.Find(t => t.GroupId == GroupInfoObj.GroupId).FirstOrDefaultAsync().ConfigureAwait(false);


                    var update = Builders<UserGroups>.Update
                                        .Set(x => x.GroupName, !string.IsNullOrEmpty(GroupInfoObj.GroupName) ? GroupInfoObj.GroupName : dbGroupInfo.GroupName)
                                        .Set(x => x.GroupIcon, !string.IsNullOrEmpty(GroupInfoObj.GroupIcon) ? GroupInfoObj.GroupIcon : dbGroupInfo.GroupIcon);


                    await context.UserGroups.UpdateOneAsync(t => t.GroupId == GroupInfoObj.GroupId, update).ConfigureAwait(false);

                }

                return GroupInfoObj;

            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception");
                throw;
            }

        }
        public async Task<bool> JoinGroupAsync(string AddedBy, string UserId, string GroupId, CancellationToken cancellationToken)
        {
            try
            {
                if (AddedBy == null) throw new ArgumentNullException(nameof(AddedBy));

                if (UserId == null) throw new ArgumentNullException(nameof(UserId));

                if (GroupId == null) throw new ArgumentNullException(nameof(GroupId));


                var isExists = context.GroupMembers.Find(t => t.GroupId == GroupId && t.UserId.ToLower(CultureInfo.CurrentCulture) == UserId.ToLower(CultureInfo.CurrentCulture)).ToList().Count;
                if (isExists > 0)
                {
                    await context.GroupMembers.UpdateOneAsync((
                        Builders<GroupMembers>.Filter.Eq("GroupId", GroupId) & Builders<GroupMembers>.Filter.Eq("UserId", UserId)),
                        Builders<GroupMembers>.Update.Set("IsDeleted", false)).ConfigureAwait(false);
                }
                else
                {
                    var newMember = new GroupMembers()
                    {
                        MemberId = "",
                        GroupId = GroupId,
                        UserId = UserId,
                        IsDeleted = false,
                        IsAdmin = false,
                        InsertedDate = DateTime.Now
                    };

                    await context.GroupMembers.InsertOneAsync(newMember).ConfigureAwait(false);
                }


                //to add join message start
                GroupMessages GroupMsgObj = new GroupMessages();
                GroupMsgObj.GroupId = GroupId;
                GroupMsgObj.GroupMsgId = "";
                GroupMsgObj.Message = AddedBy.ToLower(CultureInfo.CurrentCulture) == UserId.ToLower(CultureInfo.CurrentCulture) ? UserId + " joined" : AddedBy + " added " + UserId;
                GroupMsgObj.MsgType = 2;
                GroupMsgObj.UserId = UserId;
                GroupMsgObj.MediaPath = "";

                await AddGroupMessageAsync(GroupMsgObj, cancellationToken).ConfigureAwait(false);
                //to add join message end

                return true;

            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception");
                throw;
            }

        }
        public async Task<bool> LeaveGroupAsync(string RemovedBy, string UserId, string GroupId, CancellationToken cancellationToken)
        {
            try
            {
                if (RemovedBy == null) throw new ArgumentNullException(nameof(RemovedBy));
                if (UserId == null) throw new ArgumentNullException(nameof(UserId));

                var update = Builders<GroupMembers>.Update.Combine(
                       Builders<GroupMembers>.Update.Set(x => x.IsDeleted, true)
                      );

                var filter = Builders<GroupMembers>.Filter.And(
                    Builders<GroupMembers>.Filter.Eq(x => x.GroupId, GroupId),
                    Builders<GroupMembers>.Filter.Eq(x => x.UserId, UserId)
                   );

                //to add left message start
                GroupMessages GroupMsgObj = new GroupMessages();
                GroupMsgObj.GroupId = GroupId;
                GroupMsgObj.GroupMsgId = "";
                GroupMsgObj.Message = RemovedBy.ToLower(CultureInfo.CurrentCulture) == UserId.ToLower(CultureInfo.CurrentCulture) ? UserId + " left" : RemovedBy + " removed " + UserId;
                GroupMsgObj.MsgType = 2;
                GroupMsgObj.UserId = UserId;
                GroupMsgObj.MediaPath = "";

                await AddGroupMessageAsync(GroupMsgObj, cancellationToken).ConfigureAwait(false);
                //to add left message end

                var result = await context.GroupMembers.UpdateOneAsync(filter, update).ConfigureAwait(false);

                await RemoveAllGroupMessageAsync(GroupId, UserId, cancellationToken).ConfigureAwait(false);

                if (result.IsAcknowledged && result.ModifiedCount > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception");
                throw;
            }

        }


        public async Task<GroupMessages> AddGroupMessageAsync(GroupMessages GroupMsgObj, CancellationToken cancellationToken)
        {
            try
            {
                if (GroupMsgObj == null) throw new ArgumentNullException(nameof(GroupMsgObj));

                List<UserAction> seenUsers = new List<UserAction>();
                seenUsers.Add(new UserAction()
                {
                    UserId = GroupMsgObj.UserId,
                    Date = DateTime.Now
                });
                GroupMsgObj.SeenUsers.AddRange(seenUsers);

                await context.GroupMessages.InsertOneAsync(GroupMsgObj).ConfigureAwait(false);


                return GroupMsgObj;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception");
                throw;
            }
        }

        //proper
        public async Task<bool> RemoveSingleGroupMessageAsync(string GroupMsgId, string UserId, CancellationToken cancellationToken)
        {

            try
            {

                var builder = Builders<GroupMessages>.Filter;
                var filter = builder.Eq(x => x.GroupMsgId, GroupMsgId);
                var update = Builders<GroupMessages>.Update
                    .AddToSet(x => x.DeletedUsers, new UserAction
                    {
                        UserId = UserId,
                        Date = DateTime.Now
                    });
                var updateResult = await context.GroupMessages.UpdateOneAsync(filter, update).ConfigureAwait(false);

                if (updateResult.IsAcknowledged && updateResult.ModifiedCount > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception");
                throw;
            }
        }

        //proper
        public async Task<bool> RemoveAllGroupMessageAsync(string GroupId, string UserId, CancellationToken cancellationToken)
        {
            try
            {
                var allGroupMessages = context.GroupMessages.Find(t => t.GroupId == GroupId).ToList();

                foreach (var item in allGroupMessages)
                {


                    var builder = Builders<GroupMessages>.Filter;
                    var filter = builder.Eq(x => x.GroupMsgId, item.GroupMsgId);
                    var update = Builders<GroupMessages>.Update
                        .AddToSet(x => x.DeletedUsers, new UserAction
                        {
                            UserId = UserId,
                            Date = DateTime.Now
                        });
                    await context.GroupMessages.UpdateOneAsync(filter, update).ConfigureAwait(false);

                }

                //return result.IsAcknowledged && result.ModifiedCount > 0;
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception");
                throw;
            }
        }


        //proper
        public async Task<bool> MarkSeenAllGroupMessageAsync(List<string> GroupMsgIds, string UserId)
        {
            try
            {
                if (GroupMsgIds == null) throw new ArgumentNullException(nameof(GroupMsgIds));

                foreach (var item in GroupMsgIds)
                {

                    var builder = Builders<GroupMessages>.Filter;
                    var filter = builder.Eq(x => x.GroupMsgId, item);
                    var update = Builders<GroupMessages>.Update
                        .AddToSet(x => x.SeenUsers, new UserAction
                        {
                            UserId = UserId,
                            Date = DateTime.Now
                        });

                     await context.GroupMessages.UpdateOneAsync(filter, update).ConfigureAwait(false);

                }

                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception");
                throw;
            }
        }

        #endregion



    }
}
