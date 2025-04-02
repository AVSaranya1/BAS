using DataAccessLayer.Implementation;
using DataAccessLayer.Interface;
using DataAccessLayer.Uow.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Uow.Implementation
{
    public class UowEntityMenu : IUowEntityMenu, IDisposable
    {

        private readonly IHttpContextAccessor? _httpContextAccessor;
        private readonly DbTransaction? _transaction;
        private readonly DbConnection? _connection;
        private bool _disposedValue = false;

        public UowEntityMenu(IHttpContextAccessor? httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));

            // Fetch the latest connection string dynamically
            string connectionString = GetConnectionStringFromContext();

            _connection = new SqlConnection(connectionString);
            _connection.Open();
            _transaction = _connection.BeginTransaction();
        }

        private string GetConnectionStringFromContext()
        {
            var context = _httpContextAccessor.HttpContext;
            if (context?.Items.TryGetValue("connection", out var conn) == true && conn is string connectionString)
            {
                return connectionString;
            }
            throw new InvalidOperationException("Connection string not found in HttpContext.");
        }


        public IEntityMenuDAL entityMenuRepo
        {
            get
            {
                return new EntityMenuDAL(_transaction, GetConnectionStringFromContext());
            }
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _transaction?.Dispose();
                    _connection?.Dispose();
                }
                _disposedValue = true;
            }
        }

        public void Commit()
        {
            try
            {
                _transaction?.Commit();
            }
            catch
            {
                _transaction?.Rollback();
                Dispose();
                throw;
            }
        }

        ~UowEntityMenu()
        {
            Dispose(false);
        }
    }
}
