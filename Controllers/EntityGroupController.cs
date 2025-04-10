using Dapper;
using DataAccessLayer.Model;
using DataAccessLayer.Uow.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using WebApi.Services.Interface;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EntityGroupController : ApiBaseController
    {

        private readonly IUowEntityGroup? _repository;
        private readonly ILogger<EntityGroupController>? _logger;
        private readonly IAuditLogService? _auditLogService;
        string token = string.Empty;
        string userGuid = string.Empty;

        public EntityGroupController(IUowEntityGroup? repository, IConfiguration configuration, ILogger<EntityGroupController>? logger, IAuditLogService auditLogService) : base(configuration)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger;
            _auditLogService = auditLogService;
        }


        [HttpGet("GetEntityGroup")]
        public async Task<IActionResult> GetOrganisationLevelInfo()
        {
            try
            {
                string token = string.Empty;
                string userGuid = string.Empty;

                var  entityGroupModel = new EntityGroupModel
                {
                    Mode = "GET"
                };

                var lsOrganisation = await _repository.entityGroupRepo.GetEntityGroup(entityGroupModel);
                await _auditLogService.LogAction(userGuid, "GetOrganisationLevelInfo", token);
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
        public async Task<IActionResult> GetEntityGroupDetails(long ID)
        {
            try
            {
                string token = string.Empty;
                string userGuid = string.Empty;

                var entityGroupModel = new EntityGroupModel
                {
                    Mode = "GET",
                    ID = ID
                };

                var lsOrganisation = await _repository.entityGroupRepo.GetEntityGroupDetails(entityGroupModel);
                await _auditLogService.LogAction(userGuid, "GetOrganisationLevelInfo", token);

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

        [HttpGet("AddEntityGroup")]
        public async Task<IActionResult> AddEntityGroup(string EntityCode, string EntityName, string EntityDescription, bool Ischild, long? parentEntityID,string? strLogo) //string? parentEntityGuid)
        {
            try
            {
                // Logging action (Ensure userGuid and token are properly assigned)
                string userGuid = HttpContext?.Session?.GetString(Common.SessionVariables.Guid); 
                string token = HttpContext?.Session?.GetString(Common.SessionVariables.Token); 
                // Create the entity model
                var entityGroupModel = new EntityGroupModel
                {
                    Mode = "Add",
                    EntityGroupCode = EntityCode,
                    EntityGroupName = EntityName,
                    EntityGroupDesc = EntityDescription,
                    IsChild = Ischild,
                    //ParentEntityGroupGuid = parentEntityGuid,
                    ParentID = parentEntityID,
                    CreatedBy = userGuid,
                    Logo = strLogo
                };

                // Call repository method
                var lstEntity = await _repository.entityGroupRepo.AddEntityGroup(entityGroupModel);

               
                await _auditLogService.LogAction(userGuid, "GetOrganisationLevelInfo", token);

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
        [HttpGet("EditEntityGroup")]
        public async Task<IActionResult> EditEntityGroup(long ID,string EntityCode, string EntityName, string EntityDescription, bool Ischild, long? parentEntityID,string? strLogo)  //string? parentEntityGuid)
        {
            try
            {
                // Logging action (Ensure userGuid and token are properly assigned)
                string userGuid = HttpContext?.Session?.GetString(Common.SessionVariables.Guid);
                string token = HttpContext?.Session?.GetString(Common.SessionVariables.Token);
                // Create the entity model
                var entityGroupModel = new EntityGroupModel
                {
                    Mode = "Edit",
                    EntityGroupCode = EntityCode,
                    EntityGroupDesc = EntityDescription,
                    EntityGroupName = EntityName,
                    IsChild = Ischild,
                    ParentID = parentEntityID,
                   // ParentEntityGroupGuid = parentEntityGuid,
                    CreatedBy = userGuid,
                    ID = ID,
                    Logo = strLogo
                };

                // Call repository method
                var lstEntity = await _repository.entityGroupRepo.EditEntityGroup(entityGroupModel);
                await _auditLogService.LogAction(userGuid, "EditEntityGroup", token);

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

        [HttpPost("DeleteEntityGroup")]
        public async Task<IActionResult> DeleteEntityGroup([FromBody] List<EntityGroupDel> lstEntityGroupDel)
        {
            try
            {
                string strUserGuid = HttpContext?.Session?.GetString(Common.SessionVariables.Guid) ?? string.Empty;
                string strMode = "DELETE";

                List<DeleteResultModel> lstDelete = await _repository.entityGroupRepo.DeleteEntityGroup(lstEntityGroupDel, strMode, strUserGuid);
                await _auditLogService.LogAction(strUserGuid, "DeleteEntityGroup", string.Empty);

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
