using Microsoft.AspNetCore.Mvc;
using WebApi.Services.Interface;
using WebApi.Services;
using DataAccessLayer.Model;
using DataAccessLayer.Uow.Implementation;
using DataAccessLayer.Uow.Interface;
using System;
using DataAccessLayer.Model.BusinessEntity;

namespace WebApi.Controllers;

[Route("api/{region?}/[controller]")]
[ApiController]
public class BusinessEntityController : ApiBaseController
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IAuditLogService _auditLogService;
    private SessionService _sessionService;
    private readonly ILogger<BusinessEntityController> _logger;

    public BusinessEntityController(
        ILogger<BusinessEntityController> logger,
        IConfiguration configuration,
        IHttpContextAccessor httpContextAccessor,
        SessionService sessionService,
        IAuditLogService auditLogService) : base(configuration)
    {
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
        _auditLogService = auditLogService;
        _sessionService = sessionService;
    }

    [HttpGet("getBusinessEntity")]
    public async Task<IActionResult> GetAllBusinessEntity()
    {
        try
        {
            string response = _sessionService.GetSession(Common.SessionVariables.Guid);
            if (!string.IsNullOrEmpty(response))
            {
                using (IUowEntity _repo = new UowEntity(_httpContextAccessor))
                {
                    var lstData = await _repo.EntityDALRepo.GetAllBusinessEntity();
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

    [HttpGet("getBusinessEntityByID/{id}")]
    public async Task<IActionResult> GetBusinessEntityByID([FromRoute] long id)
    {
        try
        {
            string response = _sessionService.GetSession(Common.SessionVariables.Guid);
            if (!string.IsNullOrEmpty(response))
            {
                using (IUowEntity _repo = new UowEntity(_httpContextAccessor))
                {
                    var result = await _repo.EntityDALRepo.GetBusinessEntityById(id);
                    if (result != null)
                    {
                        return Ok(result);
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

    [HttpPost("addBusinessEntity")]
    public async Task<IActionResult> AddBusinessEntity(AddBusinessEntityModel objModel)
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
                if (!string.IsNullOrEmpty(response))
                {
                    using (IUowEntity _repo = new UowEntity(_httpContextAccessor))
                    {
                        await _auditLogService.LogAction("", "AddBusinessEntity", "");
                        var result = await _repo.EntityDALRepo.AddBusinessEntityAsync(objModel);
                        _repo.Commit();
                        if (string.IsNullOrEmpty(result))
                        {
                            _logger.LogError(Environment.NewLine);
                            _logger.LogError("Bad Request occurred while accessing the InsertBusinessEntity function in BusinessEntity api controller");
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

    [HttpPut("updateBusinessEntity")]
    public async Task<IActionResult> UpdateBusinessEntity(UpdateBusinessEntityModel objModel)
    {
        if (objModel == null)
        {
            return BadRequest(Common.Messages.InvalidData);
        }
        else
        {
            try
            {
                using (IUowEntity _repo = new UowEntity(_httpContextAccessor))
                {
                    string response = _sessionService.GetSession(Common.SessionVariables.Guid);
                    if (!string.IsNullOrEmpty(response))
                    {
                        await _auditLogService.LogAction("", "UpdateBusinessEntity", "");
                        var result = await _repo.EntityDALRepo.UpdateBusinessEntityAsync(objModel);
                        _repo.Commit();
                        if (string.IsNullOrEmpty(result))
                        {
                            _logger.LogError(Environment.NewLine);
                            _logger.LogError("Bad Request occurred while accessing the updateBusinessEntity function in BusinessEntity controller");
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

    [HttpDelete("deleteBusinessEntity")]
    public async Task<IActionResult> DeleteBusinessEntity(DeleteBusinessEntityModel deleteLevelDetail)
    {
        try
        {
            using (IUowEntity _repo = new UowEntity(_httpContextAccessor))
            {
                string userIdStr = _sessionService.GetSession(Common.SessionVariables.UserID);
                long userId = !string.IsNullOrEmpty(userIdStr) ? Convert.ToInt64(userIdStr) : 0;
                string response = _sessionService.GetSession(Common.SessionVariables.Guid);
                if (!string.IsNullOrEmpty(response))
                {
                    await _auditLogService.LogAction("", "DeleteLevelDetail", "");
                    var dataTable = deleteLevelDetail.ConvertToDataTable(deleteLevelDetail.DeleteDataTable);
                    var result = await _repo.EntityDALRepo.DeleteBusinessEntityAsync(dataTable);
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

    [HttpGet("getMapParentBusinessUnit")]
    public async Task<IActionResult> GetMapParentBusinessUnit()
    {
        try
        {
            string response = _sessionService.GetSession(Common.SessionVariables.Guid);
            if (!string.IsNullOrEmpty(response))
            {
                using (IUowEntity _repo = new UowEntity(_httpContextAccessor))
                {
                    var lstData = await _repo.EntityDALRepo.GetMapParentBusinessUnit();
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

    [HttpGet("getMapEntityGroup")]
    public async Task<IActionResult> GetMapEntityGroup()
    {
        try
        {
            string response = _sessionService.GetSession(Common.SessionVariables.Guid);
            if (!string.IsNullOrEmpty(response))
            {
                using (IUowEntity _repo = new UowEntity(_httpContextAccessor))
                {
                    var lstData = await _repo.EntityDALRepo.GetMapEntityGroup();
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
}
