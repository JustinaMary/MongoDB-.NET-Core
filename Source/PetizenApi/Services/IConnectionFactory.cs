using System.Data;

namespace PetizenApi.Services
{
    public interface IConnectionFactory
    {
        IDbConnection GetConnection { get; }
        void CloseConnection();

    }
}
