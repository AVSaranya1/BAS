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
        
        string token = string.Empty;
        string userGuid = string.Empty;
        private readonly IWebHostEnvironment _environment;
        private readonly string _uploadFolder;
        public OrganisationController(ILogger<OrganisationController> logger,IConfiguration configuration,IUowOrganisation repository, IAuditLogService auditLogService, IWebHostEnvironment environment) : base(configuration)
        {
            _logger = logger;
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _auditLogService = auditLogService;
            _environment = environment;
            _uploadFolder= configuration["FileUpload:VirtualFilePath"] ?? "img";
        }

        [HttpPost("InsertOrganisation")]
        public async Task<IActionResult> InsertOrganisation([FromForm] OrganisationModel orgModel)
        {
            try
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };

                // Get file extension
                var extension = Path.GetExtension(orgModel.Logo.FileName).Trim().ToLowerInvariant();
                if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension))
                {
                    return BadRequest("Invalid file type.");
                }
                else
                {
                    if (!Directory.Exists(_uploadFolder))
                        Directory.CreateDirectory(_uploadFolder);

                    var filePath = Path.Combine(_uploadFolder, orgModel.Logo.FileName.Trim());
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await orgModel.Logo.CopyToAsync(stream);
                    }

                    var fileUrl = $"{Request.Scheme}://{Request.Host}/{_uploadFolder}/{orgModel.Logo.FileName}";
                }
                
                var result = await _repository.OrganisationDALRepo.InsertOrganisation(orgModel);
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
                if (!Directory.Exists(_uploadFolder))
                    Directory.CreateDirectory(_uploadFolder);
                // Retrieve session values if session exists

                var lsOrganisation = await _repository.OrganisationDALRepo.GetAllOrganisation();
                foreach(var org in lsOrganisation)
                {
                    var filePath = Path.Combine(_uploadFolder, org.Logo);
                    var fileUrl = $"{Request.Scheme}://{Request.Host}/{_uploadFolder}/{org.Logo}";
                    org.LogoUrl = fileUrl;
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
                    var filePath = Path.Combine(_uploadFolder, objOrganisationModel.Logo);
                    var fileUrl = $"{Request.Scheme}://{Request.Host}/{_uploadFolder}/{objOrganisationModel.Logo}";
                    objOrganisationModel.LogoUrl = fileUrl;
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
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };

                // Get file extension
                var extension = Path.GetExtension(Org.Logo.FileName).ToLowerInvariant();
                if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension))
                {
                    return BadRequest("Invalid file type.");
                }
                else
                {
                    if (!Directory.Exists(_uploadFolder))
                        Directory.CreateDirectory(_uploadFolder);

                    var filePath = Path.Combine(_uploadFolder, Org.Logo.FileName);
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await Org.Logo.CopyToAsync(stream);
                    }

                    var fileUrl = $"{Request.Scheme}://{Request.Host}/{_uploadFolder}/{Org.Logo.FileName}";
                }
                var result = await _repository.OrganisationDALRepo.UpdateOrganisation(Org);
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
