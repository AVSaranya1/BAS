using Microsoft.AspNetCore.Mvc;
using WebApi.Services.Interface;
using WebApi.Services;
using DataAccessLayer.Model;
using DataAccessLayer.Uow.Implementation;
using DataAccessLayer.Uow.Interface;
using System;
using DataAccessLayer.Model.BusinessEntity;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebApi.Controllers;

[Route("api/{region?}/[controller]")]
[ApiController]
public class BusinessEntityController : ApiBaseController
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IAuditLogService _auditLogService;
    private SessionService _sessionService;
    private UploadFileServices _uploadfile;
    private readonly ILogger<BusinessEntityController> _logger;
    private readonly string _physicalPath;
    private readonly string _virtualPath;
    private string _FolderName;

    public BusinessEntityController(
        ILogger<BusinessEntityController> logger,
        IConfiguration configuration,
        IHttpContextAccessor httpContextAccessor,
        SessionService sessionService,
        IAuditLogService auditLogService,
        UploadFileServices uploadfile
        ) : base(configuration)
    {
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
        _auditLogService = auditLogService;
        _sessionService = sessionService;
        _uploadfile = uploadfile;
        _physicalPath = Path.Combine(configuration["FileUpload:PhysicalFilePath"], _FolderName = Common.FileFolder.Logo) ?? Common.FileFolder.img;
        _virtualPath = Path.Combine(configuration["FileUpload:VirtualFilePath"], _FolderName = Common.FileFolder.Logo) ?? Common.FileFolder.img;
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
                                foreach (var data in lstData)
                                {
                                    data.LogoPath = _uploadfile.GetFile(data.Logo, _virtualPath);
                                }
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
                        result.LogoPath = _uploadfile.GetFile(result.Logo, _virtualPath);
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
    public async Task<IActionResult> AddBusinessEntity([FromForm] AddBusinessEntityModel objModel)
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
                        string? FileName = objModel.LogoFile?.FileName.Trim() ?? string.Empty;
                        string ImageUpdated = await _uploadfile.InsertandUpdateFileName(FileName, objModel.LogoFile, _physicalPath);

                        if (string.IsNullOrEmpty(ImageUpdated))
                        {
                            return BadRequest(Common.Messages.InvalidData);
                        }
                        else
                        {
                            objModel.Logo = ImageUpdated;
                        }

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
    public async Task<IActionResult> UpdateBusinessEntity([FromForm] UpdateBusinessEntityModel objModel)
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
                        string? FileName = objModel.LogoFile?.FileName.Trim() ?? string.Empty;
                        
                        string ImageUpdated = await _uploadfile.InsertandUpdateFileName(FileName, objModel.LogoFile, _physicalPath);
                        
                        if (!string.IsNullOrEmpty(ImageUpdated))
                        {
                            objModel.Logo = ImageUpdated;
                        }                        

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
