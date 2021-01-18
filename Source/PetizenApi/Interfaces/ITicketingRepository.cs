using GraphQL.Language.AST;
using PetizenApi.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PetizenApi.Interfaces
{
    public interface ITicketingRepository
    {
        Task<List<Ticketing>> GetTicketingAsync(string tId, string locationId, int day, IDictionary<string, Field> fields, CancellationToken cancellationToken);

        Task<Ticketing> InsUpdTicketingAsync(Ticketing ticketing, CancellationToken cancellationToken);

        Task<bool> DeleteTicketingAsync(string tId, CancellationToken cancellationToken);

    }
}
