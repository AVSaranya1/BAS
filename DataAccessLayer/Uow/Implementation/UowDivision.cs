using DataAccessLayer.Uow.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using DataAccessLayer.Interface;
using DataAccessLayer.Implementation;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace DataAccessLayer.Uow.Implementation
{
    public class UowDivision : UowCommon,IUowDivision
    {
        public UowDivision(IHttpContextAccessor httpContextAccessor)
            : base(httpContextAccessor)
        {
        }
        public IDivisionDAL ClientDivisionDALRepo
        {
            get
            {
                return new DivisionDAL(Transaction, GetConnectionStringFromContext());
                
            }
        }
    }
}
