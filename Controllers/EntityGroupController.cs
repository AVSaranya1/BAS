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

        [HttpGet("AddEntityGroup")]
        public async Task<IActionResult> AddEntityGroup(string EntityCode, string EntityDescription, bool Ischild, long? parentEntityID) //string? parentEntityGuid)
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
                    EntityGroupDesc = EntityDescription,
                    IsChild = Ischild,
                    //ParentEntityGroupGuid = parentEntityGuid,
                    ParentID = parentEntityID,
                    CreatedBy = userGuid
                };

                // Call repository method
                var lstEntity = await _repository.entityGroupRepo.AddEntityGroup(entityGroupModel);

               
                await _auditLogService.LogAction(userGuid, "GetOrganisationLevelInfo", token);

                // Check if the result is valid
                if (lstEntity != null && lstEntity.Any())
                {
                    return Ok(lstEntity);
                }
                else
                {
                    return BadRequest("Failed to add Entity Group.");
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
        public async Task<IActionResult> EditEntityGroup(long ID,string EntityCode, string EntityDescription, bool Ischild, long? parentEntityID)  //string? parentEntityGuid)
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
                    IsChild = Ischild,
                    ParentID = parentEntityID,
                   // ParentEntityGroupGuid = parentEntityGuid,
                    CreatedBy = userGuid,
                    ID = ID
                };

                // Call repository method
                var lstEntity = await _repository.entityGroupRepo.EditEntityGroup(entityGroupModel);
                await _auditLogService.LogAction(userGuid, "EditEntityGroup", token);

                // Check if the result is valid
                if (lstEntity != null && lstEntity.Any())
                {
                    return Ok(lstEntity);
                }
                else
                {
                    return BadRequest("Failed to add Entity Group.");
                }
            }
            catch (Exception ex)
            {
                // Log the error properly
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        //[HttpGet("DeleteEntityGroup")]
        //public async Task<IActionResult> DeleteEntityGroup(List<EntityGroupDel> lstEntityGroupDel)
        //{
        //    try
        //    {
        //        string strUserGuid = HttpContext?.Session?.GetString(Common.SessionVariables.Guid) ?? string.Empty;
        //        string strMode = "DELETE";

        //        // Ensure levelInfoDetails is a collection
        //        List<DeleteResultModel> lstDelete = await _repository.entityGroupRepo.DeleteEntityGroup(lstEntityGroupDel, strMode, strUserGuid);
        //        await _auditLogService.LogAction(strUserGuid, "DeleteEntityGroup", string.Empty);

        //        if (lstDelete != null && lstDelete.Count > 0)
        //        {
        //            return Ok(lstDelete);
        //        }
        //        else
        //        {
        //            return BadRequest("No records found to delete.");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Error in DeleteOrganisationLevel: {ex.Message} {ex.StackTrace}");
        //        return StatusCode(500, "An error occurred while deleting the organisation level.");
        //    }
        //}


        [HttpPost("DeleteEntityGroup")]
        public async Task<IActionResult> DeleteEntityGroup([FromBody] List<EntityGroupDel> lstEntityGroupDel)
        {
            try
            {
                string strUserGuid = HttpContext?.Session?.GetString(Common.SessionVariables.Guid) ?? string.Empty;
                string strMode = "DELETE";

                List<DeleteResultModel> lstDelete = await _repository.entityGroupRepo.DeleteEntityGroup(lstEntityGroupDel, strMode, strUserGuid);
                await _auditLogService.LogAction(strUserGuid, "DeleteEntityGroup", string.Empty);

                if (lstDelete != null && lstDelete.Count > 0)
                {
                    return Ok(lstDelete);
                }
                else
                {
                    return BadRequest("No records found to delete.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in DeleteEntityGroup: {ex.Message} {ex.StackTrace}");
                return StatusCode(500, "An error occurred while deleting the entity group.");
            }
        }


    }
}
