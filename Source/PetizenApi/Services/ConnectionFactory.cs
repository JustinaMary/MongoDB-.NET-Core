using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using PetizenApi.Database;
using System;
using System.Data;

namespace PetizenApi.Services
{
    public class ConnectionFactory : IConnectionFactory, IDisposable
    {
        private IDbConnection _connection;
        private readonly IOptions<DatabaseConfiguration> _configs;
        private bool isDisposed;
        public ConnectionFactory(IOptions<DatabaseConfiguration> Configs)
        {
            _configs = Configs;
        }


        public IDbConnection GetConnection
        {
            get
            {
                if (_connection == null)
                {
                    _connection = new SqlConnection(_configs.Value.DefaultConnection);
                }
                if (_connection.State != ConnectionState.Open)
                {
                    _connection.Open();
                }
                return _connection;
            }
        }

        public void CloseConnection()
        {
            if (_connection != null && _connection.State == ConnectionState.Open)
            {
                _connection.Close();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // The bulk of the clean-up code is implemented in Dispose(bool)
        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed) return;

            if (disposing)
            {
                // free managed resources
                _connection.Dispose();
            }
            isDisposed = true;
        }



    }
}
