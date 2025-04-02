using DataAccessLayer.Interface;
using DataAccessLayer.Model;
using DataAccessLayer.Uow.Implementation;
using DataAccessLayer.Uow.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SqlServer.Management.Smo;
using System.Configuration;
using WebApi.Services.Interface;


namespace WebApi.Controllers
{
    [Route("api/{region?}/[controller]")]
    [ApiController]
    public class EntityMenuController : ApiBaseController
    {
        private readonly IUowEntityMenu? _repository;
        private readonly ILogger<EntityMenuController>? _logger;
        private readonly IAuditLogService? _auditLogService;


        string token = string.Empty;
        string userGuid = string.Empty;

        public EntityMenuController(IUowEntityMenu? repository, IConfiguration configuration, ILogger<EntityMenuController>? logger, IAuditLogService auditLogService) : base(configuration)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger;
            _auditLogService = auditLogService;
        }

        [HttpGet("GetEntityMenu")]
        public async Task<IActionResult> GetEntityMenu()
        {
            try
            {
                string token = string.Empty;
                string struserGuid = HttpContext?.Session?.GetString(Common.SessionVariables.Guid) ?? string.Empty;

                string strMode = "GET"; // Fixed syntax issue

                // Retrieve session values if session exists
                var lstEntityMenu = await _repository.entityMenuRepo.GetEntityMenu(strMode, struserGuid);

                await _auditLogService.LogAction(struserGuid, "GetAllDepartment", token);

                return lstEntityMenu switch
                {
                    not null => Ok(lstEntityMenu),
                    _ => BadRequest()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message} {ex.StackTrace}");
                return StatusCode(500);
            }
        }
        [HttpGet("SelectEntityMenu")]
        public async Task<IActionResult> SelectEntityMenu([FromQuery] string LevelGUID, [FromQuery] string? LevelDetailGUID)
        {
            try
            {
               // string strUserGuid = HttpContext.Request.Headers["User-Guid"]; // Alternative to session
                string struserGuid = HttpContext?.Session?.GetString(Common.SessionVariables.Guid) ?? string.Empty;
       
                HttpContext?.Session.SetString(Common.SessionVariables.LevelGUID, LevelGUID ?? string.Empty);
                HttpContext?.Session.SetString(Common.SessionVariables.LevelDetailGUID, LevelDetailGUID ?? string.Empty);

                await _auditLogService.LogAction(struserGuid, "SelectEntityMenu", ""); // Removed unused token variable

                return Ok("Entity Selected");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error in SelectEntityMenu");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("Get-ClientMenu")]
        public async Task<IActionResult> GetMenu()
        {
            try
            {

                string UserNameGuid = HttpContext.Session.GetString(Common.SessionVariables.Guid);
                string ClientCode = "";
                int OrgID = 0;

                string token = string.Empty;
                string userGuid = string.Empty;

                // Retrieve session values if session exists

                var lsOrganisation = await _repository.entityMenuRepo.GetMenu(UserNameGuid);

                await _auditLogService.LogAction(userGuid, "GetModules", token);

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
    }
}
