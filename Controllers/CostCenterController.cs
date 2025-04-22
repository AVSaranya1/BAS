using Microsoft.AspNetCore.Mvc;
using WebApi.Services.Interface;
using WebApi.Services;
using DataAccessLayer.Model;
using DataAccessLayer.Uow.Implementation;
using DataAccessLayer.Uow.Interface;
using System;


namespace WebApi.Controllers
{
    [Route("api/{region}/[controller]")]
    [ApiController]
    public class CostCenterController : ApiBaseController
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuditLogService _auditLogService;
        private SessionService _sessionService;
        private readonly ILogger<CostCenterController> _logger;
        public CostCenterController(ILogger<CostCenterController> logger,IHttpContextAccessor httpContextAccessor, IAuditLogService auditLogService, SessionService sessionService,IConfiguration configuration) : base(configuration)
        {
            _logger = logger;
            _sessionService = sessionService;
            _httpContextAccessor = httpContextAccessor;
            _auditLogService = auditLogService;
        }
        [HttpGet("getCostCenter")]
        public async Task<IActionResult> GetAllCostCenter()
        {
            try
            {
                string response = _sessionService.GetSession(Common.SessionVariables.Guid);
                if (!string.IsNullOrEmpty(response))
                {
                    using (IUowCostCenter _repo = new UowCostCenter(_httpContextAccessor))
                    {
                        var lstData = await _repo.CostCenterDALRepo.GetAllCostCenter();
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

        [HttpGet("getCostCenterByGuid/{Guid}")]
        public async Task<IActionResult> GetCostCenterByGuID([FromRoute] string Guid)
        {
            try
            {
                string response = _sessionService.GetSession(Common.SessionVariables.Guid);
                if (!string.IsNullOrEmpty(response))
                {
                    using (IUowCostCenter _repo = new UowCostCenter(_httpContextAccessor))
                    {
                        var responseSelect = new GetSelectedCostCenterGuidList();
                        var result = await _repo.CostCenterDALRepo.GetCostCenterByGuId(Guid);
                        if (result.getCostCenterModel != null)
                        {
                            responseSelect = new GetSelectedCostCenterGuidList
                            {
                                CostCenterModel = result.getCostCenterModel,
                                BusinessEntityDivMapDatatable = result.getBusinessEntityTables,
                                DivisionDatatable = result.getDivisionDatatables,
                                DepartmentDatatable = result.getDepartmentDatatables
                            };
                            return Ok(responseSelect);
                        }
                        else
                        {
                            return BadRequest(Common.Messages.NoRecordsFound);
                        }
                    }
                }
                else
                {
                    return BadRequest(Common.Messages.Login);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "  " + ex.StackTrace);
                throw;
            }
        }
        [HttpGet("getMapParentCostCenter")]
        public async Task<IActionResult> getMapParentCostCenter()
        {
            try
            {
                string response = _sessionService.GetSession(Common.SessionVariables.Guid);
                if (!string.IsNullOrEmpty(response))
                {
                    using (IUowCostCenter _repo = new UowCostCenter(_httpContextAccessor))
                    {
                        var lstData = await _repo.CostCenterDALRepo.getMapParentCostCenter();
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
        [HttpGet("getMapBusinessUnit")]
        public async Task<IActionResult> GetMapBusinessUnit()
        {
            try
            {
                string response = _sessionService.GetSession(Common.SessionVariables.Guid);
                if (!string.IsNullOrEmpty(response))
                {
                    using (IUowCostCenter _repo = new UowCostCenter(_httpContextAccessor))
                    {
                        var lstData = await _repo.CostCenterDALRepo.GetMapBusinessUnit();
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
        [HttpGet("getMapDivision")]
        public async Task<IActionResult> getMapDivision()
        {
            try
            {
                string response = _sessionService.GetSession(Common.SessionVariables.Guid);
                if (!string.IsNullOrEmpty(response))
                {
                    using (IUowCostCenter _repo = new UowCostCenter(_httpContextAccessor))
                    {
                        var lstData = await _repo.CostCenterDALRepo.GetMapDivisionCost();
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
        [HttpGet("getMapDepartment")]
        public async Task<IActionResult> getMapDepartment()
        {
            try
            {
                string response = _sessionService.GetSession(Common.SessionVariables.Guid);
                if (!string.IsNullOrEmpty(response))
                {
                    using (IUowCostCenter _repo = new UowCostCenter(_httpContextAccessor))
                    {
                        var lstData = await _repo.CostCenterDALRepo.GetMapDepartment();
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
        [HttpPost("addCostCenter")]
        public async Task<IActionResult> AddCostCenter(CostCenterModel objModel)
        {
            if (objModel == null)
            {
                return BadRequest(Common.Messages.InvalidData);
            }
            else
            {
                try
                {
                    string response = _sessionService.GetSession(Common.SessionVariables.Guid);
                    
                    objModel.CreatedBy = response;
                    if (!string.IsNullOrEmpty(response))
                    {
                        using (IUowCostCenter _repo = new UowCostCenter(_httpContextAccessor))
                        {
                            await _auditLogService.LogAction("", "AddCostCenter", "");
                            var dataTable = objModel.ConvertToDataTable(objModel.InsertandUpdateCostCenterList);
                            var result = await _repo.CostCenterDALRepo.AddCostCenterAsync(objModel, dataTable);
                            _repo.Commit();
                            if (string.IsNullOrEmpty(result))
                            {
                                _logger.LogError(Environment.NewLine);
                                _logger.LogError("Bad Request occurred while accessing the InsertCostCenter function in CostCenter api controller");
                                return BadRequest();
                            }
                            else
                            {
                                return Ok(result);
                            }
                        }
                    }
                    else
                    {
                        return BadRequest(Common.Messages.Login);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message + "  " + ex.StackTrace);
                    throw;
                }
            }
        }

        [HttpPut("updateCostCenter")]
        public async Task<IActionResult> UpdateCostCenter(UpdateCostCenterModel objModel)
        {
            if (objModel == null)
            {
                return BadRequest(Common.Messages.InvalidData);
            }
            else
            {
                try
                {
                    using (IUowCostCenter _repo = new UowCostCenter(_httpContextAccessor))
                    {
                        string response = _sessionService.GetSession(Common.SessionVariables.Guid);
                        if (!string.IsNullOrEmpty(response))
                        {
                            var dataTable = objModel.ConvertToDataTable(objModel.InsertandUpdateCostCenterList);
                            await _auditLogService.LogAction("", "UpdateCostCenter", "");
                            var result = await _repo.CostCenterDALRepo.UpdateCostCenterAsync(objModel, dataTable);
                            _repo.Commit();
                            if (string.IsNullOrEmpty(result))
                            {
                                _logger.LogError(Environment.NewLine);
                                _logger.LogError("Bad Request occurred while accessing the updateCostCenter function in CostCenter controller");
                                return BadRequest();
                            }
                            else
                            {
                                return Ok(result);
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

        [HttpDelete("deleteCostCenter")]
        public async Task<IActionResult> DeleteCostCenter(DeleteCostCenterModel deleteLevelDetail)
        {
            try
            {
                using (IUowCostCenter _repo = new UowCostCenter(_httpContextAccessor))
                {
                    string userIdStr = _sessionService.GetSession(Common.SessionVariables.UserID);
                    long userId = !string.IsNullOrEmpty(userIdStr) ? Convert.ToInt64(userIdStr) : 0;
                    string response = _sessionService.GetSession(Common.SessionVariables.Guid);
                    if (!string.IsNullOrEmpty(response))
                    {
                        await _auditLogService.LogAction("", "DeleteCostCenter", "");
                        var dataTable = deleteLevelDetail.ConvertToDataTable(deleteLevelDetail.DeleteDataTable);
                        var result = await _repo.CostCenterDALRepo.DeleteCostCenterAsync(dataTable);
                        _repo.Commit();
                        if (string.IsNullOrEmpty(result))
                        {
                            _logger.LogError(Environment.NewLine);
                            _logger.LogError("Bad Request occurred while accessing the DeleteLevelDetail function in Entity api controller");
                            return BadRequest();
                        }
                        else
                        {
                            return Ok(result);
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
