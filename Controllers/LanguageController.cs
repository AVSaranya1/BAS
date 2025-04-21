using DataAccessLayer.Model;
using DataAccessLayer.Uow.Implementation;
using DataAccessLayer.Uow.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Diagnostics.Eventing.Reader;

namespace WebApi.Controllers
{
    [Route("api/{region?}/[controller]")]
    [ApiController]
    public class LanguageController : ApiBaseController
    {
        private readonly ILogger<LanguageController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LanguageController(ILogger<LanguageController> logger, IConfiguration configuration,IHttpContextAccessor httpContextAccessor) : base(configuration)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            
        }
        [HttpGet("getAllLanguage")]
        public async Task<IActionResult> GetAllLanguage()
        {
            try
            {
                using (IUowLanguage _repo = new UowLanguage(_httpContextAccessor))
                {
                    var lstLanguageModel = await _repo.LanguageDALRepo.GetAllLanguage();
                    if (lstLanguageModel != null)
                    {
                        switch (lstLanguageModel.Count)
                        {
                            case > 0:
                                return Ok(lstLanguageModel);
                            case 0:
                                return BadRequest(Common.Messages.NoRecordsFound);
                            default:
                                return BadRequest();

                        }
                        
                    }
                }
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "  " + ex.StackTrace);
                throw;
            }
        }
        [HttpGet("getLanguageByGuid/{Guid}")]
        public async Task<IActionResult> GetLanguageByGuId(Guid Guid)
        {
            try
            {
                using (IUowLanguage _repo = new UowLanguage(_httpContextAccessor))
                {
                    var objLanguageModel = await _repo.LanguageDALRepo.GetLanguageByGuid(Guid);
                    if (objLanguageModel != null)
                    {
                        return Ok(objLanguageModel);
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "  " + ex.StackTrace);
                throw;
            }
        }
        [HttpGet("ViewLanguageByGuid/{Guid}")]
        public async Task<IActionResult> ViewLanguageByGuId(Guid Guid)
        {
            try
            {
                using (IUowLanguage _repo = new UowLanguage(_httpContextAccessor))
                {
                    var objLanguageModel = await _repo.LanguageDALRepo.ViewLanguageByGuid(Guid);
                    if (objLanguageModel != null)
                    {
                        return Ok(objLanguageModel);
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "  " + ex.StackTrace);
                throw;
            }
        }
        [HttpPost("insertUpdateLanguage")]
        public async Task<IActionResult> InsertUpdateLanguage(LanguageModel objModel)
        {
            try
            {
                using (IUowLanguage _repo = new UowLanguage(_httpContextAccessor))
                {
                    var result = await _repo.LanguageDALRepo.InsertUpdateLanguage(objModel);
                    var msg = "Language Inserted Successfully";
                    _repo.Commit();
                    if (result)
                    {
                        return Ok(msg);
                    }
                    else
                    {
                        _logger.LogError(Environment.NewLine);
                        _logger.LogError("Bad Request occurred while accessing the InsertUpdateLanguage function in Language api controller");
                        return BadRequest();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "  " + ex.StackTrace);
                throw;
            }
        }
        [HttpPut("UpdateLanguage")]
        public async Task<IActionResult> UpdateLanguage(UpdateLanguageModel objModel)
        {

            if (objModel == null)
            {
                return BadRequest(Common.Messages.InvalidData);
            }
            else
            {
                try
                {
                    using (IUowLanguage _repo = new UowLanguage(_httpContextAccessor))
                    {
                        var result = await _repo.LanguageDALRepo.UpdateLanguageAsync(objModel);
                        var msg = "User account updated successfully.";
                        _repo.Commit();
                        if (result)
                        {
                            return Ok(msg);
                        }
                        else
                        {
                            _logger.LogError(Environment.NewLine);
                            _logger.LogError("Bad Request occurred while accessing the updateUserAccount function in User Account api controller");
                            return BadRequest();
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message + "  " + ex.StackTrace);
                    throw;

                }
            }
        }
        [HttpGet("deleteLanguage/{guid}")]
        public async Task<IActionResult> DeleteLanguage(Guid? guid)
        {
            try
            {
                using (IUowLanguage _repo = new UowLanguage(_httpContextAccessor))
                {
                    var result = await _repo.LanguageDALRepo.DeleteLanguage(guid);
                    _repo.Commit();
                    if (result)
                    {
                        return Ok(result);
                    }
                    else
                    {
                        _logger.LogError(Environment.NewLine);
                        _logger.LogError("Bad Request occurred while accessing the DeleteLanguage function in Language api controller");
                        return BadRequest();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "  " + ex.StackTrace);
                throw;
            }
        }

    }
}
