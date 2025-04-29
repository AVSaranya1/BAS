using Dapper;
using DataAccessLayer.Model;
using DataAccessLayer.Uow.Implementation;
using DataAccessLayer.Uow.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using WebApi.Services;
using WebApi.Services.Interface;

namespace WebApi.Controllers
{
    [Authorize]
    [Route("api/{region}/[controller]")]
    [ApiController]
    public class EntityGroupController : ApiBaseController
    {

        private readonly IUnitOfWork _repository;
        private readonly ILogger<EntityGroupController>? _logger;
        private readonly IAuditLogService? _auditLogService;
        string token = string.Empty;
        string userGuid = string.Empty;
        private UploadFileServices _uploadfile;
        private readonly IWebHostEnvironment _environment;
        private readonly string _physicalPath;
        private readonly string _virtualPath;
        private string _FolderName;

        public EntityGroupController(IUnitOfWork repository, IConfiguration configuration, ILogger<EntityGroupController>? logger, IAuditLogService auditLogService, UploadFileServices uploadFileServices,IWebHostEnvironment webHostEnvironment) : base(configuration)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger;
            _auditLogService = auditLogService;
            _environment = webHostEnvironment;
            _physicalPath = Path.Combine(configuration["FileUpload:PhysicalFilePath"], _FolderName = Common.FileFolder.EntityGroup) ?? Common.FileFolder.img;
            _virtualPath = Path.Combine(configuration["FileUpload:VirtualFilePath"], _FolderName = Common.FileFolder.EntityGroup) ?? Common.FileFolder.img;
            _uploadfile = uploadFileServices;
            _repository.SwitchDatabase(DatabaseType.Organization);
        }


        [HttpGet("GetEntityGroup")]
        public async Task<IActionResult> GetOrganisationLevelInfo()
        {
            try
            {
                string token = string.Empty;
                string userGuid = string.Empty;

                var entityGroupModel = new GetEntityGroupModel
                {
                    Mode = Common.PageMode.GET,
                };

                await _auditLogService.LogAction("GetOrganisationLevelInfo");
                
                var lsOrganisation = await _repository.EntityGroupDALRepo.GetEntityGroup(entityGroupModel);
               
                string filePath = string.Empty;

                foreach (var org in lsOrganisation)
                {
                    filePath = _uploadfile.GetFile(org.Logo, _virtualPath);
                    org.LogoUrl = filePath;
                }
                return lsOrganisation switch
                {
                    not null => Ok(lsOrganisation),
                    _ => BadRequest()
                };
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.StackTrace);
                return StatusCode(500);
            }
        }

        [HttpGet("GetEntityGroupDetails")]
        public async Task<IActionResult> GetEntityGroupDetails(Guid? Guid)
        {
            try
            {
                string token = string.Empty;
                string userGuid = string.Empty;

                var entityGroupModel = new GetEntityGroupModel
                {
                    Mode = Common.PageMode.GET,
                    Guid = Guid
                };

                await _auditLogService.LogAction("GetEntityGroupDetails");
                
                var lsOrganisation = await _repository.EntityGroupDALRepo.GetEntityGroupDetails(entityGroupModel);
                
                string filePath = string.Empty;

                foreach (var org in lsOrganisation)
                {
                    filePath = _uploadfile.GetFile(org.Logo, _virtualPath);
                    org.LogoUrl = filePath;
                }
                return lsOrganisation switch
                {
                    not null => Ok(lsOrganisation),
                    _ => BadRequest()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.StackTrace);
                return StatusCode(500);
            }
        }
        // Map Parent Entity Group Dropdown
        [HttpGet("GetMapEntityGroupDropdown")]
        public async Task<IActionResult> GetMapEntityGroupDropdown()
        {
            try
            {
                string userGuid = HttpContext?.Session?.GetString(Common.SessionVariables.Guid);
                if (!string.IsNullOrEmpty(userGuid))
                {                    
                    var lstData = await _repository.EntityGroupDALRepo.GetMapEntityGroup();
                    if (lstData != null)
                    {
                        switch (lstData.Count())
                        {
                            case > 0:
                                return Ok(lstData);
                            case 0:
                                return BadRequest(Common.Messages.NoRecordsFound);
                            default:
                                return BadRequest();
                        }
                    }
                }

                else
                {
                    return BadRequest(Common.Messages.Login);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "  " + ex.StackTrace);
                throw;
            }
        }

        [HttpPost("AddEntityGroup")]
        public async Task<IActionResult> AddEntityGroup([FromForm] EntityGroupModel objModel, IFormFile? strLogo) //string? parentEntityGuid)
        {
            try
            {
                string EntityCode = objModel.EntityGroupCode;
                string EntityName= objModel.EntityGroupName;
                string EntityDescription = objModel.EntityGroupDesc;
                bool? Ischild = objModel.IsChild;
                Guid? parentEntityGuid = objModel.ParentEntityGroupGuid;
                string? FileName = strLogo?.FileName.Trim() ?? string.Empty;
                string ImageUpdated = await _uploadfile.InsertandUpdateFileName(FileName, strLogo, _physicalPath);
                // Logging action (Ensure userGuid and token are properly assigned)
                string userGuid = HttpContext?.Session?.GetString(Common.SessionVariables.Guid); 
                string token = HttpContext?.Session?.GetString(Common.SessionVariables.Token); 
                // Create the entity model
                var entityGroupModel = new EntityGroupModel
                {
                    Mode = Common.PageMode.ADD,
                    EntityGroupCode = EntityCode,
                    EntityGroupName = EntityName,
                    EntityGroupDesc = EntityDescription,
                    IsChild = Ischild,
                    ParentEntityGroupGuid = parentEntityGuid,
                    //ParentID = parentEntityID,
                    CreatedBy = userGuid,
                    Logo = ImageUpdated
                };

                await _auditLogService.LogAction("AddEntityGroup");

                // Call repository method                
                _repository.BeginTransaction();
                    var lstEntity = await _repository.EntityGroupDALRepo.AddEntityGroup(entityGroupModel);                    
                await _repository.CompleteAsync();

                // Check if the result is valid
                if (lstEntity != null && lstEntity.Any())
                {
                    switch (Convert.ToString(lstEntity.First()))
                    {
                        case "1":
                            return Ok(Messages.MSG_SAVE_SUCCESS);
                        case "2":
                            return Ok(Messages.MSG_REC_EXISTS_CODE);
                        default:
                            return BadRequest(Messages.MSG_ADD_FAIL);
                    }
                }
                else
                {
                    return BadRequest(Messages.MSG_ADD_FAIL);
                }
            }
            catch (Exception ex)
            {
                // Log the error properly
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "An unexpected error occurred.");
            }
        }
        [HttpPut("EditEntityGroup")]
        public async Task<IActionResult> EditEntityGroup([FromForm] UpdateEntityGroupModel updateEntityGroupModel,IFormFile? strLogo)  //string? parentEntityGuid)
        {
            try
            {
                Guid? Guid = updateEntityGroupModel.Guid;
                string EntityCode = updateEntityGroupModel.EntityGroupCode;
                string EntityName = updateEntityGroupModel.EntityGroupName;
                string EntityDescription = updateEntityGroupModel.EntityGroupDesc;
                bool? Ischild = updateEntityGroupModel.IsChild;
                Guid? parentEntityGuid = updateEntityGroupModel.ParentEntityGroupGuid;
                

                string? FileName = strLogo?.FileName.Trim() ?? string.Empty;

                string ImageUpdated = await _uploadfile.InsertandUpdateFileName(FileName, strLogo, _physicalPath);
                // Logging action (Ensure userGuid and token are properly assigned)
                string userGuid = HttpContext?.Session?.GetString(Common.SessionVariables.Guid);
                string token = HttpContext?.Session?.GetString(Common.SessionVariables.Token);
                // Create the entity model
                var entityGroupModel = new UpdateEntityGroupModel
                {
                    Mode = Common.PageMode.EDIT,
                    EntityGroupCode = EntityCode,
                    EntityGroupDesc = EntityDescription,
                    EntityGroupName = EntityName,
                    IsChild = Ischild,
                    //ParentID = parentEntityID,
                    ParentEntityGroupGuid = parentEntityGuid,
                    CreatedBy = userGuid,
                    Guid = Guid,
                    Logo = ImageUpdated
                };

                await _auditLogService.LogAction("EditEntityGroup");

                // Call repository method                
                _repository.BeginTransaction();
                    var lstEntity = await _repository.EntityGroupDALRepo.EditEntityGroup(entityGroupModel);
                await _repository.CompleteAsync();

                // Check if the result is valid
                if (lstEntity != null && lstEntity.Any())
                {
                    switch (Convert.ToString(lstEntity.First()))
                    {
                        case "1":
                            return Ok(Messages.MSG_UPDATED_SUCCESS);
                        case "2":
                            return Ok(Messages.MSG_REC_EXISTS_CODE);
                        default:
                            return BadRequest(Messages.MSG_ADD_FAIL);
                    }
                }
                else
                {
                    return BadRequest(Messages.MSG_ADD_FAIL);
                }
            }
            catch (Exception ex)
            {
                // Log the error properly
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpDelete("DeleteEntityGroup")]
        public async Task<IActionResult> DeleteEntityGroup([FromBody] List<EntityGroupDel> lstEntityGroupDel)
        {
            try
            {
                string strUserGuid = HttpContext?.Session?.GetString(Common.SessionVariables.Guid) ?? string.Empty;
                string strMode = "DELETE";

                await _auditLogService.LogAction("DeleteEntityGroup");
                
                _repository.BeginTransaction();
                    List<DeleteResultModel> lstDelete = await _repository.EntityGroupDALRepo.DeleteEntityGroup(lstEntityGroupDel, strMode, strUserGuid);                    
                await _repository.CompleteAsync();

                return lstDelete switch
                {
                    { Count: > 0 } => Ok(lstDelete),
                    _ => BadRequest("No records found to delete.")
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in DeleteEntityGroup: {ex.Message} {ex.StackTrace}");
                return StatusCode(500, "An error occurred while deleting the entity group.");
            }
        }
        

    }
}
