using Azure;
using DataAccessLayer.Model;
using DataAccessLayer.Uow.Implementation;
using DataAccessLayer.Uow.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.SqlServer.Management.Smo;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using WebApi.Services;
using WebApi.Services.Interface;

namespace WebApi.Controllers
{
    [Route("api/{region?}/[controller]")]
    [ApiController]
    public class ClientRoleController : ApiBaseController
    {
        private readonly ILogger<ClientRoleController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private SessionService _sessionService;
        private GUID _guid;
        private readonly IAuditLogService _auditLogService;

        public ClientRoleController(ILogger<ClientRoleController> logger, IHttpContextAccessor httpContextAccessor, IConfiguration configuration, SessionService sessionService, GUID gUID, IAuditLogService auditLogService) : base(configuration)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _auditLogService = auditLogService;
            _sessionService = sessionService;
            _guid = gUID;
        }
        [HttpGet("getAllClientRole")]
        public async Task<IActionResult> getAllClientRole()
        {
            try
            {
                using (IUowClientRole _repo = new UowClientRole(_httpContextAccessor))
                {
                    //string response = _sessionService.GetSession(Common.SessionVariables.Guid);
                    //if (!string.IsNullOrEmpty(response))
                    //{
                    await _auditLogService.LogAction("", "getAllClientRole", "");
                    string userIdStr = _sessionService.GetSession(Common.SessionVariables.UserID);
                    string ClientDBName= _sessionService.GetSession(Common.SessionVariables.DBName);
                    long userId = !string.IsNullOrEmpty(userIdStr) ? Convert.ToInt64(userIdStr) : 0;
                    var lstRoleModel = await _repo.RoleDALRepo.GetAllRole(userId, ClientDBName);
                    if (lstRoleModel != null && lstRoleModel.Count > 0)
                    {
                        return Ok(lstRoleModel);
                    }
                    else
                    {
                        return BadRequest(Common.Messages.NoRecordsFound);
                    }
                    //}
                    //else
                    //{
                    //    return BadRequest(Common.Messages.Login);
                    //}
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "  " + ex.StackTrace);
                throw;
            }
        }
        // This Method while Showing the Module Records Based UserID
        [HttpGet("getModulesBasedOnClientRole/{RoleGUID}")]
        public async Task<IActionResult> getModulesBasedOnClientRole(string RoleGUID)
        {
            try
            {
                using (IUowClientRole _repo = new UowClientRole(_httpContextAccessor))
                {
                    string userIdStr = _sessionService.GetSession(Common.SessionVariables.UserID);
                    long userId = !string.IsNullOrEmpty(userIdStr) ? Convert.ToInt64(userIdStr) : 0;
                    string ClientDBName = _sessionService.GetSession(Common.SessionVariables.DBName);
                    //string response = _sessionService.GetSession(Common.SessionVariables.Guid);
                    //if (!string.IsNullOrEmpty(response))
                    //{
                    await _auditLogService.LogAction("", "getModulesBasedOnClientRole", "");
                    
                    string? guidresp = await _guid.GetGUIDBasedOnClientUserRoleGuid(RoleGUID, ClientDBName);
                    if (guidresp == RoleGUID)
                    {
                        var objRoleModel = await _repo.RoleDALRepo.getModulesBasedOnRole(RoleGUID, userId, ClientDBName);
                        if (objRoleModel.ModuleDatatable == null || objRoleModel.ModuleDatatable != null)
                        {
                            var responseUpdate = new GetClientRoleUpdateRequest
                            {
                                RoleModel = objRoleModel.rolemodel,
                                ModuleDatatable = objRoleModel.ModuleDatatable,

                            };
                            return Ok(responseUpdate);
                        }
                        else
                        {
                            return BadRequest(Common.Messages.NoRecordsFound);
                        }
                    }
                    else
                    {
                        return BadRequest("Please Check Role Guid");
                    }
                    //}
                    //else
                    //{
                    //    return BadRequest(Common.Messages.Login);
                    //}

                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "  " + ex.StackTrace);
                throw;
            }
        }
        
        [HttpPost("InsertClientRole")]
        public async Task<IActionResult> InsertClientRole(ClientRoleUpdateRequest objModel)
        {
            try
            {
                string responsemsg = string.Empty;
                if (objModel == null || objModel.RoleModel == null || objModel.ModuleDatatable == null)
                {
                    return BadRequest("Invalid input data.");
                }

                else
                {
                    using (IUowClientRole _repo = new UowClientRole(_httpContextAccessor))
                    {
                        string userIdStr = _sessionService.GetSession(Common.SessionVariables.UserID);
                        long userId = !string.IsNullOrEmpty(userIdStr) ? Convert.ToInt64(userIdStr) : 0;
                        string response = _sessionService.GetSession(Common.SessionVariables.Guid);
                        if (!string.IsNullOrEmpty(response))
                        {
                            objModel.RoleModel.CreatedBy = userId;
                            await _auditLogService.LogAction("", "InsertClientRole", "");
                            DataTable dataTable = objModel.RoleModel.ConvertToDataTable(objModel.ModuleDatatable);
                            var result = await _repo.RoleDALRepo.InsertUpdateRole(objModel.RoleModel);
                            var msg = "Role Inserted Successfully";
                            _repo.Commit();
                            if (result.roleModels != null)
                            {
                                switch (result.RetVal)
                                {
                                    case >= 1://Success
                                        responsemsg = msg;
                                        break;
                                    case -1:
                                        responsemsg = result.Msg ?? string.Empty;
                                        break;
                                    case 0:
                                        responsemsg = result.Msg ?? string.Empty;
                                        break;
                                    default:
                                        _logger.LogError(Environment.NewLine);
                                        _logger.LogError("Bad Request occurred while accessing the InsertUpdateRole function in Role api controller");
                                        return BadRequest();
                                }

                            }
                            else
                            {
                                return BadRequest(Common.Messages.Login);
                            }
                        }
                    }
                    return Ok(responsemsg);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "  " + ex.StackTrace);
                throw;
            }
        }

        // Update User Based on UserID
        [HttpPut("EditUpdateUserClientRole")]
        public async Task<IActionResult> EditUpdateUserClientRole([FromBody] GetClientRoleModel roleModel)
        {
            if (roleModel == null)
            {
                return BadRequest("Invalid input data.");
            }

            else
            {
                try
                {
                    string responsemsg = string.Empty;
                    using (IUowClientRole _repo = new UowClientRole(_httpContextAccessor))
                    {
                        roleModel.ClientDBName = _sessionService.GetSession(Common.SessionVariables.DBName);
                        string userIdStr = _sessionService.GetSession(Common.SessionVariables.UserID);
                        long userId = !string.IsNullOrEmpty(userIdStr) ? Convert.ToInt64(userIdStr) : 0;
                        string response = _sessionService.GetSession(Common.SessionVariables.Guid);
                        //if (!string.IsNullOrEmpty(response))
                        //{
                            await _auditLogService.LogAction("", "EditUpdateUserRole", "");
                            string guidResp = await _guid.GetGUIDBasedOnClientUserRoleGuid(roleModel.RoleGuid, roleModel.ClientDBName);
                            if (roleModel.RoleGuid == guidResp)
                            {
                                var result = await _repo.RoleDALRepo.EditUpdateRoleAsync(roleModel);
                                var msg = "Role updated successfully.";
                                _repo.Commit();
                                if (result.roleModels != null)
                                {
                                    switch (result.RetVal)
                                    {
                                        case >= 1:
                                            responsemsg = msg;
                                            break;

                                        case -1:
                                            responsemsg = result.Msg ?? string.Empty;
                                            break;

                                        default:
                                            _logger.LogError(Environment.NewLine);
                                            _logger.LogError("Bad Request occurred while accessing the updateUserAccount function in User Account api controller");
                                            return BadRequest();

                                    }
                                }
                            }
                            else
                            {
                                return BadRequest("Please Check Role Guid");
                            }
                    //}
                        //else
                        //{
                        //    return BadRequest(Common.Messages.Login);
                        //}
                        return Ok(responsemsg);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message + "  " + ex.StackTrace);
                    throw;
                }
            }
        }

        [HttpPut("UpdateClientRole")]
        public async Task<IActionResult> UpdateClientRole(ClientRoleInsertUpdateRequest objModel)
        {
            try
            {
                string responsemsg = string.Empty;
                if (objModel == null || objModel.RoleModel == null || objModel.ModuleDatatable == null)
                {
                    return BadRequest("Invalid input data.");
                }

                else
                {
                    using (IUowClientRole _repo = new UowClientRole(_httpContextAccessor))
                    {
                        string userIdStr = _sessionService.GetSession(Common.SessionVariables.UserID);
                        long userId = !string.IsNullOrEmpty(userIdStr) ? Convert.ToInt64(userIdStr) : 0;
                        string response = _sessionService.GetSession(Common.SessionVariables.Guid);
                        //if (!string.IsNullOrEmpty(response))
                        //{
                        objModel.RoleModel.CreatedBy = userId;
                        objModel.RoleModel.ClientDBName = _sessionService.GetSession(Common.SessionVariables.DBName);
                        await _auditLogService.LogAction("", "UpdateClientRole", "");
                        string guidResp = await _guid.GetGUIDBasedOnClientUserRoleGuid(objModel.RoleModel.RoleGuid, objModel.RoleModel.ClientDBName);
                        if (objModel.RoleModel.RoleGuid == guidResp)
                        {
                            DataTable dataTable = objModel.RoleModel.ConvertToDataTable(objModel.ModuleDatatable);
                            var result = await _repo.RoleDALRepo.UpdateRole(objModel.RoleModel);
                            var msg = "Role Updated Successfully";
                            _repo.Commit();
                            if (result.roleModels != null)
                            {
                                switch (result.RetVal)
                                {
                                    case >= 1://Success
                                        responsemsg = msg;
                                        break;
                                    case -1:
                                        responsemsg = result.Msg ?? string.Empty;
                                        break;
                                    case 0:
                                        responsemsg = result.Msg ?? string.Empty;
                                        break;
                                    default:
                                        _logger.LogError(Environment.NewLine);
                                        _logger.LogError("Bad Request occurred while accessing the InsertUpdateRole function in Role api controller");
                                        return BadRequest();
                                }
                            }
                        }
                        else
                        {
                            return BadRequest("Please Check Role Guid");
                        }
                        //}
                        //else
                        //{
                        //    return BadRequest(Common.Messages.Login);
                        //}
                    }
                    return Ok(responsemsg);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "  " + ex.StackTrace);
                throw;
            }
        }
        [HttpDelete("deleteClientRole")]

        public async Task<IActionResult> DeleteClientRole(ClientRolesDelete RoleDelete)

        {
            try
            {
                using (IUowClientRole _repo = new UowClientRole(_httpContextAccessor))
                {
                    string userIdStr = _sessionService.GetSession(Common.SessionVariables.UserID);
                    long userId = !string.IsNullOrEmpty(userIdStr) ? Convert.ToInt64(userIdStr) : 0;
                    RoleDelete.ClientDBName = _sessionService.GetSession(Common.SessionVariables.DBName);
                    string response = _sessionService.GetSession(Common.SessionVariables.Guid);
                    //if (!string.IsNullOrEmpty(response))
                    //{
                    await _auditLogService.LogAction("", "deleteClientRole", "");
                    foreach (var guid in RoleDelete.DeleteRoleNames)
                    {
                        string? GuidResp = await _guid.GetGUIDBasedOnClientUserRoleGuid(guid.RoleGUID,RoleDelete.ClientDBName);
                        if (GuidResp == guid.RoleGUID)
                        {
                            DataTable? deleteTable = RoleDelete?.ConvertToDataTable(RoleDelete.DeleteRoleNames ?? new List<ClientRolesDeleteInList>());
                            var result = await _repo.RoleDALRepo.DeleteRole(RoleDelete, userId);
                            _repo.Commit();
                            if (result.DeleteRole == true || result.DeleteRole == false)
                            {
                                return Ok(result.deleteRoleInformation);
                            }
                            else
                            {
                                _logger.LogError(Environment.NewLine);
                                _logger.LogError("Bad Request occurred while accessing the DeleteRole function in Role api controller");
                                return BadRequest();
                            }
                        }
                        else
                        {
                            return BadRequest("Please Check Role GUID");
                        }
                    }
                    //}
                    //else
                    //{
                    //    return BadRequest(Common.Messages.Login);
                    //}
                    return Ok();
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
