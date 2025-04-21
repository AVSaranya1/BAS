using DataAccessLayer.Model;
using DataAccessLayer.Uow.Implementation;
using DataAccessLayer.Uow.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using WebApi.Services;
using WebApi.Services.Interface;

namespace WebApi.Controllers
{
    [Route("api/{region?}/[controller]")]
    [ApiController]
    public class DivisionController : ApiBaseController
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuditLogService _auditLogService;
        private SessionService _sessionService;
        private GUID _guid;
        private readonly ILogger<DivisionController> _logger;
        public DivisionController(ILogger<DivisionController> logger, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IAuditLogService auditLogService, GUID gUID, SessionService sessionService) : base(configuration)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _auditLogService = auditLogService;
            _guid = gUID;
            _sessionService = sessionService;
        }
        [HttpGet("getAllDivision")]
        public async Task<IActionResult> getAllClientDivision()
        {
            try
            {
                string strLevelDetailGUID = string.Empty;
                string strLevelGUID = string.Empty;
                string strTimeZoneID = string.Empty;
                if (HttpContext?.Session != null)
                {
                    strTimeZoneID = HttpContext.Session.GetString(Common.SessionVariables.TimeZoneID) ?? string.Empty;
                }
                using (IUowDivision _repo = new UowDivision(_httpContextAccessor))
                {
                    var getClientDivisionModel = new GetDivisionModel();

                    getClientDivisionModel.TimeZoneID = strTimeZoneID;
                   string userIdStr = _sessionService.GetSession(Common.SessionVariables.UserID);
                    long userId = !string.IsNullOrEmpty(userIdStr) ? Convert.ToInt64(userIdStr) : 0;
                    getClientDivisionModel.CreatedBy=userId;
                    
                    string response = _sessionService.GetSession(Common.SessionVariables.Guid);
                    if (!string.IsNullOrEmpty(response))
                    {
                        await _auditLogService.LogAction("", "getAllDivision", "");
                        var lstClientDivisionModel = await _repo.ClientDivisionDALRepo.GetAllClientDivision(getClientDivisionModel);
                        if (lstClientDivisionModel != null && lstClientDivisionModel.Count > 0)
                        {
                            return Ok(lstClientDivisionModel);
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
        [HttpGet("getDivisionDeptBusInsertMap")]
        public async Task<IActionResult> getClientDivisionforInsertDivision()
        {
            try
            {
                string strLevelDetailGUID = string.Empty;
                
                string strTimeZoneID = string.Empty;
                if (HttpContext?.Session != null)
                {
                    strTimeZoneID = HttpContext.Session.GetString(Common.SessionVariables.TimeZoneID) ?? string.Empty;
                }
                using (IUowDivision _repo = new UowDivision(_httpContextAccessor))
                {
                    string response = _sessionService.GetSession(Common.SessionVariables.Guid);
                  if (!string.IsNullOrEmpty(response))
                  {
                    var responseget = new GetDivisionList();
                    await _auditLogService.LogAction("", "getDivisionDeptBusMap", "");
                    var objClientDivisionModel = await _repo.ClientDivisionDALRepo.GetClientDivisionDeptCatMap();
                    if (objClientDivisionModel.getDepartmentDatatables != null)
                    {
                        responseget = new GetDivisionList
                        {
                            BusinessEntityDivMapDatatable = objClientDivisionModel.getBusinessEntityDatatables,
                            CostCenterDivisionDatatable = objClientDivisionModel.getCostCenterDivisionDatatables,
                            DepartmentDatatable = objClientDivisionModel.getDepartmentDatatables
                        };
                        return Ok(responseget);
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
        [HttpGet("getDivisionByDivisionGuid/{DivisionGuid}")]
        public async Task<IActionResult> getClientDivisionByDivisionGuid(Guid DivisionGuid)
        {
            try
            {
                string strTimeZoneID = string.Empty;
                if (HttpContext?.Session != null)
                {
                    strTimeZoneID = HttpContext.Session.GetString(Common.SessionVariables.TimeZoneID) ?? string.Empty;
                }
                using (IUowDivision _repo = new UowDivision(_httpContextAccessor))
                {
                    var responseSelect = new GetDivisionList();
                    string response = _sessionService.GetSession(Common.SessionVariables.Guid);
                    if (!string.IsNullOrEmpty(response))
                    {
                        await _auditLogService.LogAction("", "getDivisionByDivisionGuid", "");
                        string GuidDivision = await _guid.GetGUIDBasedOnClientDivision(DivisionGuid.ToString());
                        if (String.Equals(GuidDivision, DivisionGuid.ToString(),StringComparison.OrdinalIgnoreCase))
                        {
                            var objClientDivisionModel = await _repo.ClientDivisionDALRepo.GetClientDivisionByGUId(DivisionGuid.ToString());
                            if (objClientDivisionModel.getClientDivisionModel != null)
                            {
                                responseSelect=new GetDivisionList
                                {
                                    DivisionModel = objClientDivisionModel.getClientDivisionModel,
                                    BusinessEntityDivMapDatatable = objClientDivisionModel.getBusinessEntityDatatables,
                                    CostCenterDivisionDatatable = objClientDivisionModel.getCostCenterDivisionDatatables,
                                    DepartmentDatatable = objClientDivisionModel.getDepartmentDatatables
                                };
                                return Ok(responseSelect);
                            }
                            else
                            {
                                return BadRequest(Common.Messages.NoRecordsFound);
                            }
                        }
                        else
                        {
                            return BadRequest("Please Check Client Division Guid");
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
        [HttpPost("insertDivision")]
        public async Task<IActionResult> insertClientDivision([FromBody] DivisionModel? objModel)
        {
            try
            {
                string strTimeZoneID = string.Empty;
                
                    strTimeZoneID = HttpContext.Session.GetString(Common.SessionVariables.TimeZoneID) ?? string.Empty;
                
                using (IUowDivision _repo = new  UowDivision(_httpContextAccessor))
                {
                    string userIdStr = _sessionService.GetSession(Common.SessionVariables.UserID);
                    long userId = !string.IsNullOrEmpty(userIdStr) ? Convert.ToInt64(userIdStr) : 0;
                    string response = _sessionService.GetSession(Common.SessionVariables.Guid);
                    if (!string.IsNullOrEmpty(response))
                    {
                        await _auditLogService.LogAction("", "insertClientDivision", "");
                        objModel.CreatedBy = userId;
                        foreach(var Dept in objModel.DepartmentBusinessDatatable)
                        {
                            if(Dept.DeptGuid.Equals("string"))
                            {
                                return BadRequest("DeptGuid should not be string");
                            }
                        }
                        
                        var DeptCatTable = objModel.ConvertToDataTable(objModel.DepartmentBusinessDatatable);
                        
                        var result = await _repo.ClientDivisionDALRepo.InsertUpdateClientDivision(objModel);
                        _repo.Commit();
                        if (result. InsertClientDivision == true || result.InsertClientDivision == false)
                        {
                            switch (result.RetVal)
                            {
                                case >=1://Success
                                    return Ok("Division Added Successfully");
                                case -2:// Failure in  Division Name
                                    return Ok(result.Msg);
                                case -1:// Failure in Division Name
                                    return Ok(result.Msg);
                                default:
                                    _logger.LogError(Environment.NewLine);
                                    _logger.LogError("Bad Request occurred while accessing the InsertUpdateNationality function in Nationality api controller");
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
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "  " + ex.StackTrace);
                throw;
            }
        }

        [HttpPut("UpdateDivision")]
        public async Task<IActionResult> UpdateClientDivision([FromBody] UpdateDivision objModel)
        {
            try
            {
                string strTimeZoneID = string.Empty;
                if (HttpContext?.Session != null)
                {
                    
                    strTimeZoneID = HttpContext.Session.GetString(Common.SessionVariables.TimeZoneID) ?? string.Empty;
                }
                using (IUowDivision _repo = new UowDivision(_httpContextAccessor))
                {
                    string userIdStr = _sessionService.GetSession(Common.SessionVariables.UserID);
                    long userId = !string.IsNullOrEmpty(userIdStr) ? Convert.ToInt64(userIdStr) : 0;
                    string response = _sessionService.GetSession(Common.SessionVariables.Guid);
                    if (!string.IsNullOrEmpty(response))
                    {
                        objModel.CreatedBy = userId;
                        var GuidResp = await _guid.GetGUIDBasedOnClientDivision(objModel.DivisionGuid);
                        if(String.Equals(GuidResp, objModel.DivisionGuid,StringComparison.OrdinalIgnoreCase))
                        {
                            var DeptCatTable = objModel.ConvertToDataTable(objModel.DepartmentBusinessDatatable);
                            await _auditLogService.LogAction("", "UpdateClientDivision", "");
                            var result = await _repo.ClientDivisionDALRepo.UpdateClientDivision(objModel);
                            _repo.Commit();
                            if (result.UpdateClientDivision == true || result.UpdateClientDivision == false)
                            {
                                switch (result.RetVal)
                                {
                                    case >= 1://Success
                                        return Ok(result.Msg);
                                    case -2:// Failure in Division Name
                                        return Ok(result.Msg);
                                    case -1:// Failure in Division Name
                                        return Ok(result.Msg);
                                    default:
                                        _logger.LogError(Environment.NewLine);
                                        _logger.LogError("Bad Request occurred while accessing the InsertUpdateNationality function in Nationality api controller");
                                        return BadRequest();
                                }
                            }
                        }
                        else
                        {
                            return BadRequest("Please Check Division GUID");
                        }
                    }
                    else
                    {
                        return BadRequest(Common.Messages.Login);
                    }
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "  " + ex.StackTrace);
                throw;
            }
        }
        [HttpDelete("deleteDivision")]
        public async Task<IActionResult> DeleteClientDivision(DeleteDivision deleteClientDivision)
        {
            try
            {
                using (IUowDivision _repo = new UowDivision(_httpContextAccessor))
                {
                    string userIdStr = _sessionService.GetSession(Common.SessionVariables.UserID);
                    long userId = !string.IsNullOrEmpty(userIdStr) ? Convert.ToInt64(userIdStr) : 0;
                    string response = _sessionService.GetSession(Common.SessionVariables.Guid);
                    if (!string.IsNullOrEmpty(response))
                    {
                        await _auditLogService.LogAction("", "deleteDivision", "");
                        foreach (var UserGuid in deleteClientDivision.DeleteDataTable)
                        {
                            var GuidResp = await _guid.GetGUIDBasedOnClientDivision(UserGuid.DivisionGuid);
                            if (!String.Equals(GuidResp.ToString(),UserGuid.DivisionGuid.ToString(),StringComparison.OrdinalIgnoreCase))
                            {
                                return BadRequest("Please Check Division GUID");
                            }
                        }
                        
                            var DeptCatTable = deleteClientDivision.ConvertToDataTable(deleteClientDivision.DeleteDataTable);
                            var result = await _repo.ClientDivisionDALRepo.DeleteClientDivision(userId, deleteClientDivision);
                            _repo.Commit();
                            if (result.deleteClientDivision == true || result.deleteClientDivision == false)
                            {
                                return Ok(result.deleteResults);
                            }
                            else
                            {
                                _logger.LogError(Environment.NewLine);
                                _logger.LogError("Bad Request occurred while accessing the DeleteNationality function in Nationality api controller");
                                return BadRequest();
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
    }
}
