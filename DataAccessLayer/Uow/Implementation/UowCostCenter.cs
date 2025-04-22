using System;
using DataAccessLayer.Implementation;
using DataAccessLayer.Interface;
using DataAccessLayer.Uow.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DataAccessLayer.Uow.Implementation
{
    public class UowCostCenter: UowCommon, IUowCostCenter
    {
        public UowCostCenter(IHttpContextAccessor httpContextAccessor)
            : base(httpContextAccessor)
        {
        }
        public ICostCenterDAL CostCenterDALRepo
        {
            get
            {
                return new CostCenterDAL(Transaction, GetConnectionStringFromContext());
            }
        }
        
    }
}
