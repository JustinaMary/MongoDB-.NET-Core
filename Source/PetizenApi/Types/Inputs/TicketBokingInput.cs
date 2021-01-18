using GraphQL.Types;

namespace PetizenApi.Types.Inputs
{
    public class TicketingInput : InputObjectGraphType
    {
        public TicketingInput()
        {

            Name = "ticketingInput";
            Field<NonNullGraphType<StringGraphType>>("tId");
            Field<NonNullGraphType<StringGraphType>>("locationId");
            Field<NonNullGraphType<IntGraphType>>("day");
            Field<NonNullGraphType<StringGraphType>>("startTime");
            Field<NonNullGraphType<StringGraphType>>("endTime");
            Field<NonNullGraphType<StringGraphType>>("breakStartTime");
            Field<NonNullGraphType<StringGraphType>>("breakEndTime");
            Field<BooleanGraphType>("isHoliday");
            Field<StringGraphType>("insertedBy");
        }
    }


}
