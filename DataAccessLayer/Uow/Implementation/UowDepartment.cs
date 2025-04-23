using DataAccessLayer.Implementation;
using DataAccessLayer.Interface;
using DataAccessLayer.Uow.Interface;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace DataAccessLayer.Uow.Implementation
{

    public class UowDepartment : UowCommon, IUowDepartment
    {
        public UowDepartment(IHttpContextAccessor httpContextAccessor)
            : base(httpContextAccessor)
        {
        }

        public IDepartmentDAL DepartmentDALRepo
        {
            get
            {
                return new DepartmentDAL(Transaction, GetConnectionStringFromContext());
            }
        }
    }
    
}
