using Azure;
using DataAccessLayer.Model;
using DataAccessLayer.Uow.Implementation;
using DataAccessLayer.Uow.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.SqlServer.Management.Smo;
using System.Diagnostics.Eventing.Reader;
using WebApi.Services;
using WebApi.Services.Interface;

namespace WebApi.Controllers
{
    [Route("api/{region?}/[controller]")]
    [ApiController]
    public class ClientUserPolicyController : ApiBaseController
    {
        private readonly ILogger<ClientUserPolicyController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private SessionService _sessionService;
        private GUID _guid;
        private readonly IAuditLogService _auditLogService;
        public ClientUserPolicyController(ILogger<ClientUserPolicyController> logger, IHttpContextAccessor httpContextAccessor, IConfiguration configuration, SessionService sessionService, GUID gUID, IAuditLogService auditLogService) : base(configuration)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _sessionService = sessionService;
            _guid = gUID;
            _auditLogService = auditLogService;
        }
        [HttpGet("getAllClientUserPolicy")]
        public async Task<IActionResult> getAllClientUserPolicy()
        {
            try
            {
                using (IUowClientUserGroup _repo = new UowClientUserGroup(_httpContextAccessor))
                {
                    string userIdStr = _sessionService.GetSession(Common.SessionVariables.UserID);
                    long userId = !string.IsNullOrEmpty(userIdStr) ? Convert.ToInt64(userIdStr) : 0;
                    string response = _sessionService.GetSession(Common.SessionVariables.Guid);
                    if (!string.IsNullOrEmpty(response))
                    {
                        await _auditLogService.LogAction("", "getAllClientUserPolicy", "");
                        string ClientDBName=_sessionService.GetSession(Common.SessionVariables.DBName);
                        var lstUserGroupModel = await _repo.UserGroupDALRepo.GetAllUserPolicy(userId, ClientDBName);
                        if (lstUserGroupModel != null && lstUserGroupModel.Count > 0)
                        {
                            return Ok(lstUserGroupModel);
                        }
                        else
                        {
                            return BadRequest(Common.Messages.NoRecordsFound);
                        }
                    }
                    else
                    {
                        return BadRequest(Common.Messages.Login);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "  " + ex.StackTrace);
                throw;
            }
        }
        [HttpGet("getClientUserPolicyByGuId/{userPolicyGuid}")]
        public async Task<IActionResult> getClientUserPolicyByGuId(string userPolicyGuid)
        {
            try
            {
                using (IUowClientUserGroup _repo = new UowClientUserGroup(_httpContextAccessor))
                {
                    string response = _sessionService.GetSession(Common.SessionVariables.Guid);
                    if (!string.IsNullOrEmpty(response))
                    {
                        await _auditLogService.LogAction("", "getUserPolicyByGuId", "");
                        string ClientDBName = _sessionService.GetSession(Common.SessionVariables.DBName);
                        string GuidUserPolicy = await _guid.GetGUIDBasedOnClientUserPolicy(userPolicyGuid, ClientDBName);
                        
                        if (GuidUserPolicy == userPolicyGuid)
                        {
                            var objUserGroupModel = await _repo.UserGroupDALRepo.GetUserPolicyByGUId(userPolicyGuid, ClientDBName);
                            if (objUserGroupModel != null)
                            {
                                return Ok(objUserGroupModel);
                            }
                            else
                            {
                                return BadRequest();
                            }
                        }
                        else
                        {
                            return BadRequest("Please Check Guid");
                        }
                    }
                    else
                    {
                        return BadRequest(Common.Messages.Login);
                    }

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "  " + ex.StackTrace);
                throw;
            }
        }
        [HttpPost("insertClientUserPolicy")]
        public async Task<IActionResult> insertClientUserPolicy(ClientUserGroupModel objModel)
        {
            try
            {
                using (IUowClientUserGroup _repo = new UowClientUserGroup(_httpContextAccessor))
                {
                    string response = _sessionService.GetSession(Common.SessionVariables.Guid);
                    if (!string.IsNullOrEmpty(response)) 
                    {
                        await _auditLogService.LogAction("", "insertClientUserPolicy", "");
                        string userIdStr = _sessionService.GetSession(Common.SessionVariables.UserID);
                        long userId = !string.IsNullOrEmpty(userIdStr) ? Convert.ToInt64(userIdStr) : 0;
                        objModel.CreatedBy = userId;
                        objModel.ClientDBName = _sessionService.GetSession(Common.SessionVariables.DBName);

                        var result = await _repo.UserGroupDALRepo.InsertUpdateUserPolicy(objModel);
                        var msg = "User Policy Inserted Successfully";
                        _repo.Commit();
                        if (result.InsertUserGroup == true || result.InsertUserGroup == false)
                        {
                            switch (result.RetVal)
                            {
                                case 1:// Success
                                    return Ok(msg);
                                case 0:// Exists
                                    return Ok(result.Msg);
                                default:
                                        
                                    _logger.LogError(Environment.NewLine);
                                    _logger.LogError("Bad Request occurred while accessing the InsertUpdateUserGroup function in User Group api controller");
                                    return NotFound("User Policy Already Exists");
                            }   
                        }

                    } 
                    else 
                    { 
                        return BadRequest(Common.Messages.Login);
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

        [HttpPut("updateClientUserPolicy")]
        public async Task<IActionResult> updateClientUserPolicy([FromBody] ClientUpdateUserGroupModel UserGroup)
        {

            if (UserGroup == null)
            {
                return BadRequest(Common.Messages.InvalidData);
            }

            try
            {
                using (IUowClientUserGroup _repo = new UowClientUserGroup(_httpContextAccessor))
                {
                    string response = _sessionService.GetSession(Common.SessionVariables.Guid);
                    if (!string.IsNullOrEmpty(response))
                    {
                        UserGroup.ClientDBName = _sessionService.GetSession(Common.SessionVariables.DBName);
                        await _auditLogService.LogAction("", "updateClientUserPolicy", "");
                        string userIdStr = _sessionService.GetSession(Common.SessionVariables.UserID);
                        long userId = !string.IsNullOrEmpty(userIdStr) ? Convert.ToInt64(userIdStr) : 0;
                        string UserPolicyGuid = await _guid.GetGUIDBasedOnClientUserPolicy(UserGroup.UserPolicyGuid, UserGroup.ClientDBName);
                        if (UserPolicyGuid == UserGroup.UserPolicyGuid)
                        {
                            UserGroup.CreatedBy = userId;
                            UserGroup.ClientDBName = _sessionService.GetSession(Common.SessionVariables.DBName);
                            var result = await _repo.UserGroupDALRepo.UpdateUserPolicyAsync(UserGroup);
                            var msg = "User Group updated successfully.";
                            _repo.Commit();
                            if (result.UpdateUserGroup == true || result.UpdateUserGroup == false)
                            {
                                switch (result.RetVal)
                                {
                                    case 1:// Success
                                        return Ok(msg);
                                    case 0:// Exists
                                        return Ok(result.Msg);
                                    default:
                                            
                                        _logger.LogError(Environment.NewLine);
                                        _logger.LogError("Bad Request occurred while accessing the InsertUpdateUserGroup function in User Group api controller");
                                        return NotFound("User Policy Already Exists");
                                }   
                            }
                        }
                        else
                        {
                            return BadRequest("Please Check UserPolicy GUID");
                        }

                    }
                else
                {
                    return BadRequest(Common.Messages.Login);
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

        [HttpDelete("deleteClientUserPolicy")]
        public async Task<IActionResult> DeleteUserGroup(DeleteClientUserGroup deleteUserGroup)
        {
            try
            {
                using (IUowClientUserGroup _repo = new UowClientUserGroup(_httpContextAccessor))
                {
                    deleteUserGroup.ClientDBName = _sessionService.GetSession(Common.SessionVariables.DBName);
                    string userIdStr = _sessionService.GetSession(Common.SessionVariables.UserID);
                    long userId = !string.IsNullOrEmpty(userIdStr) ? Convert.ToInt64(userIdStr) : 0;
                    string response = _sessionService.GetSession(Common.SessionVariables.Guid);
                    if (!string.IsNullOrEmpty(response))
                    {
                        await _auditLogService.LogAction("", "deleteClientUserPolicy", "");
                        foreach (var UserGuid in deleteUserGroup.DeleteDataTable)
                        {
                            var GuidResp = await _guid.GetGUIDBasedOnClientUserPolicy(UserGuid.UserPolicyGUID, deleteUserGroup.ClientDBName);
                            if (GuidResp == UserGuid.UserPolicyGUID)
                            {
                                var dataTable = deleteUserGroup.ConvertToDataTable(deleteUserGroup.DeleteDataTable);
                                var result = await _repo.UserGroupDALRepo.DeleteUserPolicy(userId, deleteUserGroup);
                                _repo.Commit();
                                if (result.deleteuserGroup == true || result.deleteuserGroup == false)
                                {
                                    if (result.deleteResults.Count > 0)
                                    {
                                        return Ok(result.deleteResults);
                                    }
                                }
                                else
                                {
                                    _logger.LogError(Environment.NewLine);
                                    _logger.LogError("Bad Request occurred while accessing the DeleteUserPolicy function in User Policy api controller");
                                    return BadRequest();
                                }
                            }
                        }
                    }
                    else
                    {
                        return BadRequest(Common.Messages.Login);
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
    }
}
