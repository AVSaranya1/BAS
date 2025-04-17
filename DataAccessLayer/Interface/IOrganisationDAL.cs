using DataAccessLayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interface
{
    public interface IOrganisationDAL
    {
        Task<List<GetOrganisationModel>> GetAllOrganisation();

        Task<GetOrganisationModel> GetOrganisationById(string Guid);
        Task<ViewOrganisationModel> ViewOrganisationById(string Guid);


        Task<string> InsertOrganisation(OrganisationModel OrganisationModel,string? ImageUpdated);
        Task<string> UpdateOrganisation(OrganisationModel OrganisationModel, string? ImageUpdated);

        Task<List<OrganisationDeleteRecord>> DeleteOrganisation(List<DeleteRecord> dltOrg);
        Task<List<DataLocationDropdown>> DataLocationInDropdown();
        Task<List<MasterDropDownModel>> IndustryDropdown();
        Task<List<OrganisationModules>> GetAllModules();
        Task<List<MasterDropDownModel>> GetCountryInDropdown();
    }
}
