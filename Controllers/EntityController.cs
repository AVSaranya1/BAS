using Microsoft.AspNetCore.Mvc;
using WebApi.Services.Interface;
using WebApi.Services;
using DataAccessLayer.Model;
using DataAccessLayer.Uow.Implementation;
using DataAccessLayer.Uow.Interface;
using System;

namespace WebApi.Controllers;

[Route("api/{region?}/[controller]")]
[ApiController]
public class EntityController : ApiBaseController
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IAuditLogService _auditLogService;
    private SessionService _sessionService;
    private readonly ILogger<EntityController> _logger;

    public EntityController(
        ILogger<EntityController> logger,
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

    [HttpGet("getAllLevelDetail")]
    public async Task<IActionResult> GetAllLevelDetail()
    {
        try
        {
            string response = _sessionService.GetSession(Common.SessionVariables.Guid);
            if (!string.IsNullOrEmpty(response))
            {
                using (IUowEntity _repo = new UowEntity(_httpContextAccessor))
                {
                    var lstData = await _repo.EntityDALRepo.GetAllLevelDetail();
                    if (lstData != null)
                    {
                        switch (lstData.Count)
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

    [HttpGet("getLevelDetailByGuid/{Guid}")]
    public async Task<IActionResult> GetLevelDetailById(string Guid)
    {
        try
        {
            string response = _sessionService.GetSession(Common.SessionVariables.Guid);
            if (!string.IsNullOrEmpty(response))
            {
                using (IUowEntity _repo = new UowEntity(_httpContextAccessor))
                {
                    var result = await _repo.EntityDALRepo.GetLevelDetailByGuid(Guid);
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

    [HttpPost("insertLevelDetail")]
    public async Task<IActionResult> InsertLevelDetail(EntityModel objModel)
    {
        try
        {
            string response = _sessionService.GetSession(Common.SessionVariables.Guid);
            if (!string.IsNullOrEmpty(response))
            {
                using (IUowEntity _repo = new UowEntity(_httpContextAccessor))
                {
                    //string userIdStr = _sessionService.GetSession(Common.SessionVariables.UserID);
                    //long userId = !string.IsNullOrEmpty(userIdStr) ? Convert.ToInt64(userIdStr) : 0;


                    await _auditLogService.LogAction("", "InsertLevelDetail", "");
                    var result = await _repo.EntityDALRepo.InsertLevelDetailAsync(objModel);
                    var msg = "LevelDetail created successfully.";
                    _repo.Commit();
                    if (result)
                    {
                        return Ok(msg);
                    }
                    else
                    {
                        _logger.LogError(Environment.NewLine);
                        _logger.LogError("Bad Request occurred while accessing the InsertLevelDetail function in LevelDetail api controller");
                        return BadRequest();
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

    [HttpPut("updateLevelDetail")]
    public async Task<IActionResult> UpdateLevelDetail(EntityModel objModel)
    {
        if (objModel == null )
        {
            return BadRequest(Common.Messages.InvalidData);
        }
        else
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
                        await _auditLogService.LogAction("", "UpdateLevelDetail", "");
                        var result = await _repo.EntityDALRepo.UpdateLevelDetailAsync(objModel);
                        var msg = "Entity updated successfully.";
                        _repo.Commit();
                        if (result)
                        {
                            return Ok(msg);
                        }
                        else
                        {
                            _logger.LogError(Environment.NewLine);
                            _logger.LogError("Bad Request occurred while accessing the updateEntity function in Entity controller");
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
       

    [HttpDelete("deleteLevelDetail")]
    public async Task<IActionResult> DeleteLevelDetail(DeleteLevelDetail deleteLevelDetail)
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
                    var result = await _repo.EntityDALRepo.DeleteLevelDetailAsync(dataTable);
                    _repo.Commit();

                    if (result.deleteLevelDetail)
                    {
                        if (result.deleteResults.Count > 0)
                        {
                            return Ok(result.deleteResults);
                        }
                    }
                    else
                    {
                        _logger.LogError(Environment.NewLine);
                        _logger.LogError("Bad Request occurred while accessing the DeleteLevelDetail function in Entity api controller");
                        return BadRequest();
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
}
