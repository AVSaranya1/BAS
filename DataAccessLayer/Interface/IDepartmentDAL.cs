using DataAccessLayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interface
{
    public interface IDepartmentDAL
    {
        Task<GetDept> GetDepartment(GetDepartmentInput departmentInput);  // ✅ No 'public' needed
        Task<GetDeptView> ViewDepartment(GetDepartmentInput departmentInput);  // ✅ No 'public' needed

        Task<string> InsertDepartmentDetails(AddDept addDept,string struserGuid);
        Task<string> UpdateDepartmentDetails(EditDept editDept);
        Task<List<DeptDeleteResult>> DeleteDepartmentDetails(List<DeleteDeptList> lstDeleteDept);
        
    }

}
