using DataAccessLayer.Model;
using DataAccessLayer.Interface;
using DataAccessLayer.Implementation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics.Eventing.Reader;
using DataAccessLayer.Uow.Implementation;
using DataAccessLayer.Uow.Interface;
using WebApi.Services.Interface;
using Newtonsoft.Json.Linq;
using WebApi.Services;
using Microsoft.IdentityModel.Tokens;
using System.Linq;

namespace WebApi.Controllers
{
    [Route("api/{region?}/[controller]")]
    [ApiController]
    public class OrganisationController : ApiBaseController
    {
        private readonly IUowOrganisation _repository;
        private readonly ILogger<OrganisationController> _logger;
        private readonly IAuditLogService _auditLogService;
        private UploadFileServices _uploadfile;
        
        string token = string.Empty;
        string userGuid = string.Empty;
        private readonly IWebHostEnvironment _environment;
        private readonly string _physicalPath;
        private readonly string _virtualPath;
        
        public OrganisationController(ILogger<OrganisationController> logger,IConfiguration configuration, IUowOrganisation repository, IAuditLogService auditLogService, IWebHostEnvironment environment,UploadFileServices uploadFileServices) : base(configuration)
        {
            _logger = logger;
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _auditLogService = auditLogService;
            _environment = environment;
            _physicalPath = Path.Combine(configuration["FileUpload:PhysicalFilePath"], Common.FileFolder.Org) ?? Common.FileFolder.img;
            _virtualPath = Path.Combine(configuration["FileUpload:VirtualFilePath"], Common.FileFolder.Org) ?? Common.FileFolder.img;
            _uploadfile = uploadFileServices;
        }

        [HttpPost("InsertOrganisation")]
        public async Task<IActionResult> InsertOrganisation([FromForm] OrganisationModel orgModel)
        {
            try
            {
               
                string? fileName = orgModel.Logo?.FileName.Trim() ?? string.Empty;
               
                string? ImageUpdated = await _uploadfile.InsertandUpdateFileName(fileName, orgModel.Logo, _physicalPath);
                var result = await _repository.OrganisationDALRepo.InsertOrganisation(orgModel, ImageUpdated);
                await _auditLogService.LogAction(userGuid, "InsertOrganisation", token);

                var msg = "Organisation Inserted Successfully";

                if (result == "1")
                {
                    _repository.Commit();
                    return Ok($"{msg} {result}");
                }

                _logger.LogError("Bad Request while accessing InsertOrganisation.");
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message} {ex.StackTrace}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("getOrganisation")]
        public async Task<IActionResult> GetAllOrganisaion()
        {
            try
            {

                string token = string.Empty;
                string userGuid = string.Empty;
                if (!Directory.Exists(_virtualPath))
                    Directory.CreateDirectory(_virtualPath);
                // Retrieve session values if session exists

                var lsOrganisation = await _repository.OrganisationDALRepo.GetAllOrganisation();
                foreach(var org in lsOrganisation)
                {
                    string filePath = string.Empty;
                    filePath = _uploadfile.GetFile(org.Logo, _virtualPath);
                    org.LogoUrl= filePath;
                }

               
                await _auditLogService.LogAction(userGuid, "GetAllOrganisaion", token);

                if (lsOrganisation != null)
                    {
                        return Ok(lsOrganisation);
                    }
                    else
                    {
                        return BadRequest();
                    }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.StackTrace);
                return StatusCode(500); 
            }
        }
        [HttpGet("DataLocationInDropdown")]
        public async Task<IActionResult> DataLocationInDropdown()
        {
            try
            {
                var objOrganisationModel = await _repository.OrganisationDALRepo.DataLocationInDropdown();
                // Log the action before returning response
                await _auditLogService.LogAction("", "DataLocationInDropdown", token);
                if (objOrganisationModel != null)
                {
                    return Ok(objOrganisationModel);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "  " + ex.StackTrace);
                throw;
            }
        }

        [HttpGet("IndustryInDropdown")]
        public async Task<IActionResult> IndustryInDropdown()
        {
            try
            {
                var objOrganisationModel = await _repository.OrganisationDALRepo.IndustryDropdown();
                // Log the action before returning response
                await _auditLogService.LogAction("", "IndustryDropdown", token);
                if (objOrganisationModel != null)
                {
                    return Ok(objOrganisationModel);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "  " + ex.StackTrace);
                throw;
            }
        }

        [HttpGet("GetAllModules")]
        public async Task<IActionResult> GetAllModules()
        {
            try
            {
                var objOrganisationModel = await _repository.OrganisationDALRepo.GetAllModules();
                // Log the action before returning response
                await _auditLogService.LogAction("", "GetAllModules", token);
                if (objOrganisationModel != null)
                {
                    return Ok(objOrganisationModel);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "  " + ex.StackTrace);
                throw;
            }
        }

        [HttpGet("getOrganisationById/{Guid}")]
        public async Task<IActionResult> GetOrganisationById(string Guid)
        {
            try
            {
                var objOrganisationModel = await _repository.OrganisationDALRepo.GetOrganisationById(Guid);
                // Log the action before returning response
                await _auditLogService.LogAction(userGuid, "GetOrganisationById", token);
                if (objOrganisationModel != null)
                {
                    string? filePath = string.Empty;
                    filePath = _uploadfile.GetFile(objOrganisationModel.Logo, _virtualPath);
                    objOrganisationModel.LogoUrl= filePath;
                    return Ok(objOrganisationModel);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "  " + ex.StackTrace);
                throw;
            }
        }


        [HttpPut("UpdateOrganisation")]
        public async Task<IActionResult> UpdateOrganisation([FromForm] OrganisationModel Org)
        {

            if (Org.Guid!=null && Org.Guid=="string")
            {
                return BadRequest(Common.Messages.InvalidData);
            }

            try
            {
                // var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var allowedExtensions = Common.FileExtensions.FileNameExtension;
                string? fileName= Org.Logo?.FileName??string.Empty;
                string? ImageUpdated = await _uploadfile.InsertandUpdateFileName(fileName, Org.Logo, _physicalPath);
                var result = await _repository.OrganisationDALRepo.UpdateOrganisation(Org, ImageUpdated);
                await _auditLogService.LogAction("","UpdateOrganisation", token);
                var msg = "Organization updated successfully.";
                if (result == "1")
                {
                    Ok(msg);
                }
                else
                {
                    _logger.LogError(Environment.NewLine);
                    _logger.LogError("Bad Request occurred while accessing the update Organisation function in Organisation Update api controller");
                    return BadRequest();
                }
                return Ok(msg);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "  " + ex.StackTrace);
                throw;

            }
        }


        [HttpDelete("DeleteOrganisation")]
        public async Task<IActionResult> DeleteOrganisation([FromBody] List<DeleteRecord> dltOrg)
        {
            try
            {
                var objOrganisationModel = await _repository.OrganisationDALRepo.DeleteOrganisation(dltOrg);
                await _auditLogService.LogAction(userGuid, "DeleteOrganisation", token);

                if (objOrganisationModel != null)
                {
                    return Ok(objOrganisationModel);
                }
                else
                {
                    return BadRequest();
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
