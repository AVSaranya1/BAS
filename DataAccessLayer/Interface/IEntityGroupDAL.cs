using DataAccessLayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interface
{
    public interface IEntityGroupDAL
    {
      Task<List<GetEntityGroupModel>> GetEntityGroup(GetEntityGroupModel entityGroupModel);
      Task<List<GetEntityGroupModel>> GetEntityGroupDetails(GetEntityGroupModel entityGroupModel);

        Task<string> AddEntityGroup(EntityGroupModel entityGroupModel);
        Task<string> EditEntityGroup(UpdateEntityGroupModel entityGroupModel);

        Task<List<DeleteResultModel>> DeleteEntityGroup(List<EntityGroupDel> lstEntityGroupDel, string strMode,string strUserGuid);
        Task<IEnumerable<DropDownModel>> GetMapEntityGroup();
    }
}
