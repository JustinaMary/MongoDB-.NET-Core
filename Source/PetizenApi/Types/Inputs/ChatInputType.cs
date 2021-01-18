using GraphQL.Types;

namespace PetizenApi.Types.Inputs
{

    public class ConversationMessageInput : InputObjectGraphType
    {
        public ConversationMessageInput()
        {
            Name = "conversationMsgInput";
            Field<NonNullGraphType<StringGraphType>>("convoMsgId");
            Field<NonNullGraphType<StringGraphType>>("convoId");
            Field<NonNullGraphType<StringGraphType>>("fromUserId");
            Field<StringGraphType>("toUserId");
            Field<StringGraphType>("message");
        }
    }

}
