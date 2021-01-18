using GraphQL.Types;
using PetizenApi.Models;

namespace PetizenApi.Types
{
    public class TicketingType : ObjectGraphType<Ticketing>
    {
        public TicketingType()
        {
            Field(x => x.TId);
            Field(x => x.LocationId);
            Field(x => x.Day);
            Field(x => x.StartTime);
            Field(x => x.EndTime);
            Field(x => x.BreakStartTime);
            Field(x => x.BreakEndTime);
            Field(x => x.InsertedBy);
            Field(x => x.IsHoliday);
            //Field(x => x.InsertedDate);
        }
    }



}
