using Microsoft.AspNetCore.Mvc;
using WebApi.Services.Interface;
using WebApi.Services;
using DataAccessLayer.Uow.Implementation;
using DataAccessLayer.Uow.Interface;
using DataAccessLayer.Model;

namespace WebApi.Controllers;

[Route("api/{region?}/[controller]")]
[ApiController]
public class DropdownController : ApiBaseController
{
    private readonly IUnitOfWork _repo;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IAuditLogMasterService _auditLogService;
    private SessionService _sessionService;
    private GUID _guid;
    private readonly ILogger<DropdownController> _logger;

    public DropdownController(
        ILogger<DropdownController> logger,
        IConfiguration configuration,
        IUnitOfWork repository,
        IHttpContextAccessor httpContextAccessor,
        IAuditLogMasterService auditLogService,
        GUID gUID,
        SessionService sessionService
    ) : base(configuration)
    {
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
        _auditLogService = auditLogService;
        _guid = gUID;
        _sessionService = sessionService;
        _repo = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    [HttpGet("ParentEntity")]
    public async Task<IActionResult> getParentEntity([FromQuery] string RefID1 = "", [FromQuery] string RefID2 = "")
    {
        try
        {
            string userIdStr = _sessionService.GetSession(Common.SessionVariables.UserID);
            long userId = !string.IsNullOrEmpty(userIdStr) ? Convert.ToInt64(userIdStr) : 0;
            string response = _sessionService.GetSession(Common.SessionVariables.Guid);

            if (!string.IsNullOrEmpty(response))
            {
                await _auditLogService.LogAction("getParentEntity");
               
                var lstMasterModel = await _repo.DropdownDALRepo.getParentEntity(userId, RefID1, RefID2);

                if (lstMasterModel != null && lstMasterModel.Count() > 0)
                {
                    return Ok(lstMasterModel);
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
        catch (Exception ex)
        {
            _logger.LogError(ex.Message + "  " + ex.StackTrace);
            throw;
        }
    }

    [HttpGet("Country")]
    public async Task<IActionResult> getCountry()
    {
        try
        {
            string response = _sessionService.GetSession(Common.SessionVariables.Guid);

            if (!string.IsNullOrEmpty(response))
            {
                await _auditLogService.LogAction("getCountry");
                
                var lstMasterModel = await _repo.DropdownDALRepo.getCountry();

                if (lstMasterModel != null && lstMasterModel.Count() > 0)
                {
                    return Ok(lstMasterModel);
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
        catch (Exception ex)
        {
            _logger.LogError(ex.Message + "  " + ex.StackTrace);
            throw;
        }
    }

    [HttpGet("Currency")]
    public async Task<IActionResult> getCurrency()
    {
        try
        {
            string response = _sessionService.GetSession(Common.SessionVariables.Guid);

            if (!string.IsNullOrEmpty(response))
            {
                await _auditLogService.LogAction("getCurrency");
                
                var lstMasterModel = await _repo.DropdownDALRepo.getCurrency();

                if (lstMasterModel != null && lstMasterModel.Count() > 0)
                {
                    return Ok(lstMasterModel);
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
        catch (Exception ex)
        {
            _logger.LogError(ex.Message + "  " + ex.StackTrace);
            throw;
        }
    }

    [HttpGet("Industry")]
    public async Task<IActionResult> getIndustry()
    {
        try
        {
            string response = _sessionService.GetSession(Common.SessionVariables.Guid);

            if (!string.IsNullOrEmpty(response))
            {
                await _auditLogService.LogAction("getIndustry");
                
                var lstMasterModel = await _repo.DropdownDALRepo.getIndustry();

                if (lstMasterModel != null && lstMasterModel.Count() > 0)
                {
                    return Ok(lstMasterModel);
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
        catch (Exception ex)
        {
            _logger.LogError(ex.Message + "  " + ex.StackTrace);
            throw;
        }
    }

    [HttpGet("TimeZone")]
    public async Task<IActionResult> getTimeZone()
    {
        try
        {
            string response = _sessionService.GetSession(Common.SessionVariables.Guid);

            if (!string.IsNullOrEmpty(response))
            {
                await _auditLogService.LogAction("getTimeZone");
                
                var lstDropdownModel = await _repo.DropdownDALRepo.getTimeZone();

                if (lstDropdownModel != null && lstDropdownModel.Count() > 0)
                {
                    return Ok(lstDropdownModel);
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
        catch (Exception ex)
        {
            _logger.LogError(ex.Message + "  " + ex.StackTrace);
            throw;
        }
    }

    [HttpGet("Marital")]
    public async Task<IActionResult> getMarital()
    {
        try
        {
            string response = _sessionService.GetSession(Common.SessionVariables.Guid);

            if (!string.IsNullOrEmpty(response))
            {
                await _auditLogService.LogAction("getMarital");
                
                var lstDropdownModel = await _repo.DropdownDALRepo.getMarital();

                if (lstDropdownModel != null && lstDropdownModel.Count() > 0)
                {
                    return Ok(lstDropdownModel);
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
        catch (Exception ex)
        {
            _logger.LogError(ex.Message + "  " + ex.StackTrace);
            throw;
        }
    }
}
