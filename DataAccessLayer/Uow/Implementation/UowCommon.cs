using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Data.Common;

namespace DataAccessLayer.Uow.Implementation
{
    public abstract class UowCommon : IDisposable
    {
        protected readonly IHttpContextAccessor HttpContextAccessor;
        protected readonly DbConnection Connection;
        protected readonly DbTransaction Transaction;
        private bool _disposedValue = false;

        protected UowCommon(IHttpContextAccessor httpContextAccessor)
        {
            HttpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));

            string connectionString = GetConnectionStringFromContext();
            Connection = new SqlConnection(connectionString);
            Connection.Open();
            Transaction = Connection.BeginTransaction();
        }

        protected string GetConnectionStringFromContext()
        {
            var context = HttpContextAccessor.HttpContext;
            if (context?.Items.TryGetValue("connection", out var conn) == true && conn is string connectionString)
            {
                connectionString = connectionString.Replace("Multiple Active Result Sets", "MultipleActiveResultSets")
                                                   .Replace("Trust Server Certificate", "TrustServerCertificate");
                return connectionString;
            }
            throw new InvalidOperationException("Connection string not found in HttpContext.");
        }

        public void Commit()
        {
            try
            {
                Transaction?.Commit();
            }
            catch
            {
                Transaction?.Rollback();
                Dispose();
                throw;
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
                    Transaction?.Dispose();
                    if (Connection != null && Connection.State == ConnectionState.Open)
                    {
                        Connection.Dispose();
                    }
                }
                _disposedValue = true;
            }
        }

        ~UowCommon()
        {
            Dispose(false);
        }
    }
}
