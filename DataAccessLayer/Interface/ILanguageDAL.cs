using DataAccessLayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interface
{
    public interface ILanguageDAL
    {
        Task<List<GetLanguageModel>> GetAllLanguage();
        Task<List<LanguageNameEnum>> GetAllLanguageinDropdown();

        Task<bool> InsertUpdateLanguage(LanguageModel LM);
        
        Task<bool> DeleteLanguage(Guid? guid);
        Task<GetLanguageModel> GetLanguageByGuid(Guid guid);
        Task<GetLanguageModel> ViewLanguageByGuid(Guid guid);

        Task<bool> UpdateLanguageAsync(UpdateLanguageModel objModel);
    }
}
