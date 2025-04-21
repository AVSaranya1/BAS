using DataAccessLayer.Model;
using DataAccessLayer.Uow.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using WebApi.Services.Interface;

namespace WebApi.Controllers
{
    [Route("api/{region?}/[controller]")]
    [ApiController]
    public class DepartmentController : ApiBaseController
    {
        public readonly IUowDepartment _repository;
        private readonly ILogger<DepartmentController> _logger;
        private readonly IAuditLogService _auditLogService;

        string token = string.Empty;
        string userGuid = string.Empty;

        public DepartmentController(ILogger<DepartmentController> logger, IConfiguration configuration, IUowDepartment repository, IAuditLogService auditLogService) : base(configuration)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger;
            _auditLogService = auditLogService;
        }
        
        [HttpGet("GetAllDepartment")]
        public async Task<IActionResult> GetAllDepartment()
        {
            try
            {

                string token = string.Empty;
                string userGuid = string.Empty;

                string struserGuid = string.Empty;
                string strLevelDetailGUID = string.Empty;
                string strLevelGUID = string.Empty;
                string strTimeZoneID = string.Empty;

                if (HttpContext?.Session != null)
                {
                    struserGuid = HttpContext.Session.GetString(Common.SessionVariables.Guid) ?? string.Empty;
                    //if (string.IsNullOrEmpty(LevelDetailGUID))
                    //{
                    //    strLevelDetailGUID = HttpContext.Session.GetString(Common.SessionVariables.LevelDetailGUID) ?? string.Empty;
                    //}
                    //else
                    //{
                    //    strLevelDetailGUID = LevelDetailGUID;
                    //}
                    strLevelGUID = HttpContext.Session.GetString(Common.SessionVariables.LevelGUID) ?? string.Empty;
                    strTimeZoneID = HttpContext.Session.GetString(Common.SessionVariables.TimeZoneID) ?? string.Empty;
                }

                var departmentInput = new GetDepartmentInput
                {
                    Mode = "GET",
                    UpdatedGuidBy = struserGuid,

                };
                // Retrieve session values if session exists
                var lsOrganisation = await _repository.DepartmentDALRepo.GetDepartment(departmentInput);

                await _auditLogService.LogAction(userGuid, "GetAllDepartment", token);

                if (lsOrganisation != null)
                {
                    return Ok(lsOrganisation);
                }
                else
                {
                    return BadRequest();
                }
                _logger.LogError("test");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.StackTrace);
                return StatusCode(500);
            }
        }

        [HttpGet("GetDepartmentDetails")]
        public async Task<IActionResult> GetDepartmentDetails([FromQuery] string DepartmentGuid)
        {
            try
            {

                string token = string.Empty;
                string userGuid = string.Empty;

                string struserGuid = string.Empty;
                string strLevelDetailGUID = string.Empty;
                string strLevelGUID = string.Empty;
                string strTimeZoneID = string.Empty;

                if (HttpContext?.Session != null)
                {
                    struserGuid = HttpContext.Session.GetString(Common.SessionVariables.Guid) ?? string.Empty;
                    strTimeZoneID = HttpContext.Session.GetString(Common.SessionVariables.TimeZoneID) ?? string.Empty;
                }

                var departmentInput = new GetDepartmentInput
                {
                    Mode = "GET_DETAIL",
                    UpdatedGuidBy = struserGuid,
                    DeptGUID = DepartmentGuid,

                };
                // Retrieve session values if session exists
                var lsOrganisation = await _repository.DepartmentDALRepo.GetDepartment(departmentInput);

                await _auditLogService.LogAction(userGuid, "GetAllDepartment", token);

                if (lsOrganisation != null)
                {
                    return Ok(lsOrganisation);
                }
                else
                {
                    return BadRequest();
                }
                _logger.LogError("test");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.StackTrace);
                return StatusCode(500);
            }
        }
        [HttpGet("ViewDepartmentDetails")]
        public async Task<IActionResult> ViewDepartmentDetails([FromQuery] string DepartmentGuid)
        {
            try
            {

                string token = string.Empty;
                string userGuid = string.Empty;

                string struserGuid = string.Empty;
                string strLevelDetailGUID = string.Empty;
                string strLevelGUID = string.Empty;
                string strTimeZoneID = string.Empty;

                if (HttpContext?.Session != null)
                {
                    struserGuid = HttpContext.Session.GetString(Common.SessionVariables.Guid) ?? string.Empty;
                    strLevelGUID = HttpContext.Session.GetString(Common.SessionVariables.LevelGUID) ?? string.Empty;
                    strTimeZoneID = HttpContext.Session.GetString(Common.SessionVariables.TimeZoneID) ?? string.Empty;
                }

                var departmentInput = new GetDepartmentInput
                {
                    Mode = "VIEW",
                    UpdatedGuidBy = struserGuid,
                    DeptGUID = DepartmentGuid,

                };
                // Retrieve session values if session exists
                var lsOrganisation = await _repository.DepartmentDALRepo.ViewDepartment(departmentInput);

                await _auditLogService.LogAction(userGuid, "GetAllDepartment", token);

                if (lsOrganisation != null)
                {
                    return Ok(lsOrganisation);
                }
                else
                {
                    return BadRequest();
                }
                _logger.LogError("test");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.StackTrace);
                return StatusCode(500);
            }
        }

        [HttpPost("InsertDepartmentDetails")]
        public async Task<IActionResult> InsertDepartmentDetails(AddDept addDept)
        {
            try
            {
     
                string struserGuid = string.Empty;
                string strLevelDetailGUID = string.Empty;
                string strLevelGUID = string.Empty;
                string strTimeZoneID = string.Empty;

                if (HttpContext?.Session != null)
                {
                    struserGuid = HttpContext.Session.GetString(Common.SessionVariables.Guid) ?? string.Empty;
                    strLevelDetailGUID = HttpContext.Session.GetString(Common.SessionVariables.LevelDetailGUID) ?? string.Empty;
                    strLevelGUID = HttpContext.Session.GetString(Common.SessionVariables.LevelGUID) ?? string.Empty;
                    strTimeZoneID = HttpContext.Session.GetString(Common.SessionVariables.TimeZoneID) ?? string.Empty;
                }
             
                var result = await _repository.DepartmentDALRepo.InsertDepartmentDetails(addDept,struserGuid);
                return result switch
                {
                    "1" => Ok(new { Message = "Department details inserted successfully." }),
                    "0" => Conflict(new { Message = "Department Code already exists." }),
                    _ => BadRequest(new { Message = "Insertion failed." })
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inserting department details.");
                return StatusCode(500, new { Message = "Internal Server Error", Error = ex.Message });
            }
        }


        [HttpPost("UpdateDepartmentDetails")]
        public async Task<IActionResult> UpdateDepartmentDetails(EditDept editDept)
        {
            try
            {
                string struserGuid = string.Empty;
                string strLevelDetailGUID = string.Empty;
                string strLevelGUID = string.Empty;
                string strTimeZoneID = string.Empty;

                if (HttpContext?.Session != null)
                {
                    struserGuid = HttpContext.Session.GetString(Common.SessionVariables.Guid) ?? string.Empty;
                    strLevelDetailGUID = HttpContext.Session.GetString(Common.SessionVariables.LevelDetailGUID) ?? string.Empty;
                    strLevelGUID = HttpContext.Session.GetString(Common.SessionVariables.LevelGUID) ?? string.Empty;
                    strTimeZoneID = HttpContext.Session.GetString(Common.SessionVariables.TimeZoneID) ?? string.Empty;
                }
               
                var result = await _repository.DepartmentDALRepo.UpdateDepartmentDetails(editDept);

                return result switch
                {
                    "1" => Ok(new { Message = "Department details Updated successfully." }),
                    "0" => Conflict(new { Message = "Record is not exists." }),
                    _ => BadRequest(new { Message = "Update failed." })
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inserting department details.");
                return StatusCode(500, new { Message = "Internal Server Error", Error = ex.Message });
            }
        }


        [HttpPost("DeleteDepartmentDetails")]
        public async Task<IActionResult> DeleteDepartmentDetails(List<DeleteDeptList> lstDeleteDept)
        {
            try
            {

                string struserGuid = string.Empty;
                string strLevelDetailGUID = string.Empty;
                string strLevelGUID = string.Empty;
                string strTimeZoneID = string.Empty;

                if (HttpContext?.Session != null)
                {
                    struserGuid = HttpContext.Session.GetString(Common.SessionVariables.Guid) ?? string.Empty;
                    strLevelDetailGUID = HttpContext.Session.GetString(Common.SessionVariables.LevelDetailGUID) ?? string.Empty;
                    strLevelGUID = HttpContext.Session.GetString(Common.SessionVariables.LevelGUID) ?? string.Empty;
                    strTimeZoneID = HttpContext.Session.GetString(Common.SessionVariables.TimeZoneID) ?? string.Empty;
                }
         

                var result = await _repository.DepartmentDALRepo.DeleteDepartmentDetails(lstDeleteDept);

                // Convert to ArrayList for flexibility
                ArrayList resultList = new ArrayList(result);

                switch (resultList.Count)
                {
                    case > 0:
                        return Ok(new { Message = "Department details deleted successfully.", Details = result });
                    default:
                        return BadRequest(new { Message = "Deletion failed." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting department details.");
                return StatusCode(500, new { Message = "Internal Server Error", Error = ex.Message });
            }
        }



    }
}
