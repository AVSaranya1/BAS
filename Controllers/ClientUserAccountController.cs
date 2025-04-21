using DataAccessLayer.Model;
using DataAccessLayer.Uow.Implementation;
using DataAccessLayer.Uow.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using WebApi.Services;
using DataAccessLayer.Services;
using WebApi.Services.Interface;
using NLog;
using Newtonsoft.Json;

namespace WebApi.Controllers
{
    [Route("api/{region?}/[controller]")]
    [ApiController]
    public class ClientUserAccountController : ApiBaseController
    {
        private readonly ILogger<UserAccountController> _logger;
        private readonly EmailServices _emailService;
        private SessionService _sessionService;
        private readonly IAuditLogService _auditLogService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private GUID _guid;
        private UploadFileServices _uploadfile;
        private readonly IWebHostEnvironment _environment;
        private readonly string _physicalPath;
        private readonly string _virtualPath;
        private string _FolderName;

        public ClientUserAccountController(EmailServices emailServices, ILogger<UserAccountController> logger, IHttpContextAccessor httpContextAccessor, IConfiguration configuration, SessionService sessionService, GUID guid, IAuditLogService auditLogService,UploadFileServices uploadFileServices,IWebHostEnvironment environment) : base(configuration)
        {
            _emailService = emailServices;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _sessionService = sessionService;
            _guid = guid;
            _auditLogService = auditLogService;
            _uploadfile = uploadFileServices;
            _environment = environment;
            _physicalPath = Path.Combine(configuration["FileUpload:PhysicalFilePath"], _FolderName = Common.FileFolder.UserImage) ?? Common.FileFolder.img;
            _virtualPath = Path.Combine(configuration["FileUpload:VirtualFilePath"], _FolderName = Common.FileFolder.UserImage) ?? Common.FileFolder.img;
        }
        [HttpGet("getAllClientUserAccount")]
        public async Task<IActionResult> getAllClientUserAccount(string? LevelDetailGuid= "067427BF-2613-4C17-89DB-B1D00704AD15")
        {
            try
            {
                string? strLevelDetailGUID = string.Empty;
                
                string strTimeZoneID = string.Empty;
                if (HttpContext?.Session != null)
                {

                    if (string.IsNullOrEmpty(LevelDetailGuid))
                    {
                        strLevelDetailGUID = LevelDetailGuid;

                    }
                    else
                    {
                        strLevelDetailGUID = HttpContext.Session.GetString(Common.SessionVariables.LevelDetailGUID) ?? string.Empty;
                    }
                    

                    strTimeZoneID = HttpContext.Session.GetString(Common.SessionVariables.TimeZoneID) ?? string.Empty;
                }
                using (IUowClientUser _repo = new UowClientUserAccount(_httpContextAccessor))
                {
                    string userIdStr = _sessionService.GetSession(Common.SessionVariables.UserID);
                    long userId = !string.IsNullOrEmpty(userIdStr) ? Convert.ToInt64(userIdStr) : 0;
                    string response = _sessionService.GetSession(Common.SessionVariables.Guid);
                    if (!string.IsNullOrEmpty(response))
                    {
                        await _auditLogService.LogAction("", "getAllClientUserAccount", "");
                        
                        var lstUserAccountModel = await _repo.UserAccountDALRepo.GetAllUserAccount(userId, LevelDetailGuid);
                        if (lstUserAccountModel != null && lstUserAccountModel.Count > 0)
                        {
                            string filePath = string.Empty;
                            foreach (var org in lstUserAccountModel)
                            {
                                filePath = _uploadfile.GetFile(org.ProfileImg, _virtualPath);
                                org.ProfileImgUrl = filePath;
                            }
                            return Ok(lstUserAccountModel);
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

        //Get User Policy in DropDown
        [HttpGet("getAllClientUserPolicyInDropdown")]
        public async Task<IActionResult> getAllClientUserPolicyInDropdown()
        {
            try
            {
                using (IUowClientUser _repo = new UowClientUserAccount(_httpContextAccessor))
                {
                    string response = _sessionService.GetSession(Common.SessionVariables.Guid);
                    if (!string.IsNullOrEmpty(response))
                    {
                        await _auditLogService.LogAction("", "getAllClientUserPolicyInDropdown", "");
                        string ClientDBName = _sessionService.GetSession(Common.SessionVariables.DBName);
                        var lstUserPolicyModel = await _repo.UserAccountDALRepo.getAllUserPolicyInDropdown(ClientDBName);
                        if (lstUserPolicyModel != null && lstUserPolicyModel.Count > 0)
                        {
                            return Ok(lstUserPolicyModel);
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

        //Get User Language in Dropdown
        [HttpGet("getAllClientUserLanguageInDropdown")]
        public async Task<IActionResult> getAllClientUserLanguageInDropdown()
        {
            try
            {
                using (IUowClientUser _repo = new UowClientUserAccount(_httpContextAccessor))
                {
                    string response = _sessionService.GetSession(Common.SessionVariables.Guid);
                    if (!string.IsNullOrEmpty(response))
                    {
                        await _auditLogService.LogAction("", "getAllClientUserLanguageInDropdown", "");
                        var lstUserLanguageModel = await _repo.UserAccountDALRepo.getAllUserLanguageInDropdown();
                        if (lstUserLanguageModel != null && lstUserLanguageModel.Count > 0)
                        {
                            return Ok(lstUserLanguageModel);
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
        //Get User Language in Dropdown
        [HttpGet("getAllUserTimeZoneInDropdown")]
        public async Task<IActionResult> getAllUserTimeZoneInDropdown()
        {
            try
            {
                using (IUowClientUser _repo = new UowClientUserAccount(_httpContextAccessor))
                {
                    string response = _sessionService.GetSession(Common.SessionVariables.Guid);
                    if (!string.IsNullOrEmpty(response))
                    {
                        await _auditLogService.LogAction("", "getAllUserTimeZoneInDropdown", "");
                        var lstUserLanguageModel = await _repo.UserAccountDALRepo.getAllUserTimeZoneInDropdown();
                        if (lstUserLanguageModel != null && lstUserLanguageModel.Count > 0)
                        {
                            return Ok(lstUserLanguageModel);
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
        // Get User Role in Dropdown
        [HttpGet("getAllClientUserRoleInDropdown")]
        public async Task<IActionResult> getAllClientUserRoleInDropdown()
        {
            try
            {
                using (IUowClientUser _repo = new UowClientUserAccount(_httpContextAccessor))
                {
                    string response = _sessionService.GetSession(Common.SessionVariables.Guid);
                    if (!string.IsNullOrEmpty(response))
                    {
                        await _auditLogService.LogAction("", "getAllClientUserRoleInDropdown", "");
                        string ClientDBName = _sessionService.GetSession(Common.SessionVariables.DBName);
                        var lstUserPolicyModel = await _repo.UserAccountDALRepo.getAllUserRoleInDropdown(ClientDBName);
                        if (lstUserPolicyModel != null && lstUserPolicyModel.Count > 0)
                        {
                            return Ok(lstUserPolicyModel);
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

        //Getting Specific User ID from User Account Creation
        [HttpGet("GetClientUserAccountByGUId/{guid}")]

        public async Task<IActionResult> GetClientUserAccountByGUId(string guid)
        {
            try
            {
                using (IUowClientUser _repo = new UowClientUserAccount(_httpContextAccessor))
                {
                    string userIdStr = _sessionService.GetSession(Common.SessionVariables.UserID);
                    long userId = !string.IsNullOrEmpty(userIdStr) ? Convert.ToInt64(userIdStr) : 0;
                    string responseGUId = _sessionService.GetSession(Common.SessionVariables.Guid);

                    if (!string.IsNullOrEmpty(responseGUId))
                    {
                        await _auditLogService.LogAction("", "GetClientUserAccountByGUId", "");
                        string? guidresp = await _guid.GetGUIDBasedOnClientUserGuid(guid);
                        if (guid.Equals(guidresp))
                        {
                            var objuseraccountModel = await _repo.UserAccountDALRepo.GetUserAccountByGUId(guid, userId);
                            string filePath = string.Empty;
                            filePath = _uploadfile.GetFile(objuseraccountModel.userAccounts.ProfileImg, _virtualPath);
                            objuseraccountModel.userAccounts.ProfileImgUrl = filePath;
                            var response = new ClientUserAccountResponse();
                            if (objuseraccountModel.userAccounts != null || objuseraccountModel.UserRoles != null)
                            {
                                if (objuseraccountModel.userAccounts?.UserID is not null and 0)
                                {
                                    // Return 204 No Content
                                    return NoContent();
                                }
                                if (objuseraccountModel.userAccounts?.UserID != null && objuseraccountModel.userAccounts.UserID != 0)
                                {
                                    response = new ClientUserAccountResponse
                                    {
                                        User = objuseraccountModel.userAccounts,
                                        Roles = objuseraccountModel.UserRoles,
                                        //Modules = objuseraccountModel.Modules
                                    };

                                }
                                else
                                {
                                    return BadRequest(Common.Messages.NoRecordsFound);

                                }
                                return Ok(response);
                            }
                        }
                        else
                        {
                            return BadRequest("Please Check GUID");
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

        [HttpGet("ViewClientUserAccountByGUId/{guid}")]

        public async Task<IActionResult> ViewClientUserAccountByGUId(string guid)
        {
            try
            {
                using (IUowClientUser _repo = new UowClientUserAccount(_httpContextAccessor))
                {
                    string userIdStr = _sessionService.GetSession(Common.SessionVariables.UserID);
                    long userId = !string.IsNullOrEmpty(userIdStr) ? Convert.ToInt64(userIdStr) : 0;
                    string responseGUId = _sessionService.GetSession(Common.SessionVariables.Guid);

                    if (!string.IsNullOrEmpty(responseGUId))
                    {
                        await _auditLogService.LogAction("", "ViewClientUserAccountByGUId", "");
                        string? guidresp = await _guid.GetGUIDBasedOnClientUserGuid(guid);
                        if (guid.Equals(guidresp))
                        {
                            var objuseraccountModel = await _repo.UserAccountDALRepo.ViewUserAccountByGUId(guid, userId);
                            string filePath = string.Empty;
                            filePath = _uploadfile.GetFile(objuseraccountModel.userAccounts.ProfileImg, _virtualPath);
                            objuseraccountModel.userAccounts.ProfileImgUrl = filePath;
                            var response = new ClientUserAccountResponse();
                            if (objuseraccountModel.userAccounts != null || objuseraccountModel.UserRoles != null)
                            {
                                if (objuseraccountModel.userAccounts?.UserID is not null and 0)
                                {
                                    // Return 204 No Content
                                    return NoContent();
                                }
                                if (objuseraccountModel.userAccounts?.UserID != null && objuseraccountModel.userAccounts.UserID != 0)
                                {
                                    response = new ClientUserAccountResponse
                                    {
                                        User = objuseraccountModel.userAccounts,
                                        Roles = objuseraccountModel.UserRoles,
                                        //Modules = objuseraccountModel.Modules
                                    };

                                }
                                else
                                {
                                    return BadRequest(Common.Messages.NoRecordsFound);

                                }
                                return Ok(response);
                            }
                        }
                        else
                        {
                            return BadRequest("Please Check GUID");
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
        [HttpPost("CheckClientUserAccountBeforeInsert")]
        public async Task<IActionResult> CheckClientUserAccountBeforeInsert(string UserName)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                string responseMsg = string.Empty;
                string userIdStr = _sessionService.GetSession(Common.SessionVariables.UserID);
                long userId = !string.IsNullOrEmpty(userIdStr) ? Convert.ToInt64(userIdStr) : 0;
                string response = _sessionService.GetSession(Common.SessionVariables.Guid);
                if (!string.IsNullOrEmpty(response))
                {
                    await _auditLogService.LogAction("", "CheckClientUserAccountBeforeInsert", "");
                    using (IUowClientUser _repo = new UowClientUserAccount(_httpContextAccessor))
                    {
                        var insertCheckResult = await _repo.UserAccountDALRepo.InsertCheckUserAccount(UserName);
                        if (insertCheckResult.InsertedUsers != null)
                        {
                            switch (insertCheckResult.RetVal)
                            {
                                case 0:// Success
                                    return Ok(insertCheckResult.Msg);

                                case 1:// Already Exists
                                    _logger.LogError("UserName Already exists: " + (UserName ?? "Already Exists"));
                                    return BadRequest("UserName " + (UserName ?? "Already Exists") + " Already Exists");
                            }
                        }

                        return Ok(responseMsg);

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
        [HttpPost("insertClientUserAccount")]
        
        public async Task<IActionResult> insertClientUserAccount(IFormFile? ProfileImage, [FromQuery] ClientUserAccountModel objModel,
        [FromQuery] string RoleNameList = "[{\"roleID\": 0,\"roleNameEffectiveDate\": \"2025-04-09\"}]")
        {
            try
            {
                List<ClientRoleNameInUserAccount?> roleDataList = new List<ClientRoleNameInUserAccount>();
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                string responseMsg = string.Empty;
                if (objModel == null)
                {
                    return BadRequest("Invalid input data.");
                }
                roleDataList = JsonConvert.DeserializeObject<List<ClientRoleNameInUserAccount>>(RoleNameList);

                string userIdStr = _sessionService.GetSession(Common.SessionVariables.UserID);
                long userId = !string.IsNullOrEmpty(userIdStr) ? Convert.ToInt64(userIdStr) : 0;
                string response = _sessionService.GetSession(Common.SessionVariables.Guid);
                objModel.CreatedBy = userId;
                objModel.DBName = _sessionService.GetSession(Common.SessionVariables.DBName);
                if (!Request.HasFormContentType)
                {
                    return BadRequest("Unsupported media type, expected multipart/form-data.");
                }

                var form = await Request.ReadFormAsync();
                var file = form.Files.FirstOrDefault();
                if (!string.IsNullOrEmpty(response))
                {
                    await _auditLogService.LogAction("", "insertClientUserAccount", "");
                    string? FileName = ProfileImage?.FileName.Trim() ?? string.Empty;
                    string ImageUpdated = await _uploadfile.InsertandUpdateFileName(FileName, ProfileImage, _physicalPath);
                    DataTable dataTableRole = objModel.ConvertToDataTable(roleDataList, userId, 0);
                    using (IUowClientUser _repo = new UowClientUserAccount(_httpContextAccessor))
                    {
                        var result = await _repo.UserAccountDALRepo.InsertUpdateUserAccount(objModel, ImageUpdated);
                        _repo.Commit();

                        if (result.InsertedUsers != null || result.OrgDetails == null || result.OrgDetails != null)
                        {
                            if ((result.InsertedUsers != null && result.InsertedUsers.Count > 0) &&
                                 (result.OrgDetails != null))
                            {
                                switch (result.RetVal)
                                {
                                    case 0:
                                    case -1://Already Exists
                                        responseMsg = result.Msg ?? string.Empty;
                                        break;

                                    case >= 1:
                                        result.Msg = "User Account Created Successfully";
                                        responseMsg = result.Msg ?? string.Empty;
                                        await _emailService.SendMailMessage(EmailTemplateCode.USER_ACCOUNT_CREATED, -1,
                                                                            result.RetVal,
                                                                            objModel.UserPassword);
                                        break;

                                    default:
                                        _logger.LogError(Environment.NewLine);
                                        _logger.LogError("Bad Request occurred while accessing the InsertUpdateUserAccount function in User Account api controller");
                                        return NotFound("User Account Already Exists" + BadRequest());
                                }
                            }
                            
                        }
                    }
                }
                else
                {
                    return BadRequest(Common.Messages.Login);
                }
                return Ok(responseMsg);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "  " + ex.StackTrace);

                throw;
            }
        }
        
        
        // Update User Account
        [HttpPut("ClientUpdateUserAccount")]
        
        public async Task<IActionResult> ClientUpdateUserAccount(IFormFile? ProfileImage, [FromQuery] UpdateClientUserAccountModel userAccount,
        [FromQuery] string RoleNameList = "[{\"roleID\": 0,\"roleNameEffectiveDate\": \"2025-04-09\"}]")
        {
            List<ClientRoleNameInUserAccount?> roleDataList = new List<ClientRoleNameInUserAccount>();
            if (userAccount == null)
            {
                return BadRequest("Invalid input data.");
            }
            
            else
            {
                try
                {
                    if (!Request.HasFormContentType)
                    {
                        return BadRequest("Unsupported media type, expected multipart/form-data.");
                    }
                    roleDataList = JsonConvert.DeserializeObject<List<ClientRoleNameInUserAccount>>(RoleNameList);
                    var form = await Request.ReadFormAsync();
                    var file = form.Files.FirstOrDefault();
                    string responseMsg = string.Empty;
                    string userIdStr = _sessionService.GetSession(Common.SessionVariables.UserID);
                    long userId = !string.IsNullOrEmpty(userIdStr) ? Convert.ToInt64(userIdStr) : 0;
                    userAccount.CreatedBy = userId;
                    string response = _sessionService.GetSession(Common.SessionVariables.Guid);
                    if (!string.IsNullOrEmpty(response))
                    {
                        await _auditLogService.LogAction("", "ClientUpdateUserAccount", "");
                        string? guidresp = await _guid.GetGUIDBasedOnClientUserGuid(userAccount.MasterGuid);
                        if (userAccount.MasterGuid == guidresp)
                        {
                            string? FileName = ProfileImage?.FileName.Trim() ?? string.Empty;
                            string ImageUpdated = await _uploadfile.InsertandUpdateFileName(FileName, ProfileImage, _physicalPath);
                            DataTable dataTableRole = userAccount.ConvertToDataTable(roleDataList, userId, userAccount.MasterGuid);
                            using (IUowClientUser _repo = new UowClientUserAccount(_httpContextAccessor))
                            {
                                var result = await _repo.UserAccountDALRepo.UpdateUserAccountAsync(userAccount, ImageUpdated);
                                _repo.Commit();
                                if (result.updateuseraccount != null || result.OrgDetails == null || result.OrgDetails != null)
                                {
                                    if ((result.updateuseraccount != null && result.updateuseraccount.Count > 0))
                                    {
                                        switch (result.RetVal)
                                        {
                                            case 0:
                                            case -1://Already Exists
                                                responseMsg = result.Msg ?? string.Empty;
                                                break;
                                            case >= 1:
                                                responseMsg = result.Msg ?? string.Empty;
                                                break;
                                            default:
                                                _logger.LogError(Environment.NewLine);
                                                _logger.LogError("Bad Request occurred while accessing the InsertUpdateUserAccount function in User Account api controller");
                                                return NotFound("User Account Already Exists" + BadRequest());
                                        }
                                    }
                                    

                                }
                            }
                        }
                        else
                        {
                            return BadRequest("Please Check Master Guid");
                        }
                    }
                    else
                    {
                        return BadRequest(Common.Messages.Login);
                    }
                    return Ok(responseMsg);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message + "  " + ex.StackTrace);
                    throw;
                }
            }
        }
        // Delete User Account
        [HttpDelete("ClientDeleteUserAccount")]
        public async Task<IActionResult> ClientDeleteUserAccount(ClientDeleteUserAccount deleteUserAccount)
        {
            try
            {
                using (IUowClientUser _repo = new UowClientUserAccount(_httpContextAccessor))
                {
                    string userIdStr = _sessionService.GetSession(Common.SessionVariables.UserID);
                    long userId = !string.IsNullOrEmpty(userIdStr) ? Convert.ToInt64(userIdStr) : 0;
                    string response = _sessionService.GetSession(Common.SessionVariables.Guid);
                    if (!string.IsNullOrEmpty(response))
                    {
                        await _auditLogService.LogAction("", "ClientDeleteUserAccount", "");
                        foreach (var UserGuid in deleteUserAccount.DeleteDataTable)
                        {
                            var GuidResp = await _guid.GetGUIDBasedOnClientUserGuid(UserGuid.UserGUID);
                            if (GuidResp == UserGuid.UserGUID)
                            {
                                var dataTable = deleteUserAccount.ConvertToDataTable(deleteUserAccount.DeleteDataTable);
                                var result = await _repo.UserAccountDALRepo.DeleteUserAccount(Convert.ToInt64(userId), deleteUserAccount);

                                _repo.Commit();
                                if (result.deleteuseraccount == true || result.deleteuseraccount == false)
                                {
                                    if (result.deleteResults.Count > 0)
                                    {
                                        return Ok(result.deleteResults);
                                    }
                                }
                                else
                                {
                                    _logger.LogError(Environment.NewLine);
                                    _logger.LogError("Bad Request occurred while accessing the DeleteUserAccount function in User Account api controller");
                                    return BadRequest();
                                }
                            }
                            else
                            {
                                return BadRequest("Please Check User Guid");
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
        
        
        // Reset Password in User Account
        [HttpPost("ClientResetPasswordInUserAccount")]
        public async Task<IActionResult> ClientResetPasswordInUserAccount(ResetPassword objModel)
        {
            try
            {
                string responseMsg = string.Empty;
                if (objModel == null)
                {
                    return BadRequest("Invalid input data.");
                }
                else
                {
                    using (IUowClientUser _repo = new UowClientUserAccount(_httpContextAccessor))
                    {
                        string? userIdStr = _httpContextAccessor?.HttpContext?.Session?.GetString("strUserID");
                        long userId = !string.IsNullOrEmpty(userIdStr) ? Convert.ToInt64(userIdStr) : 0;
                        string response = _sessionService.GetSession(Common.SessionVariables.Guid);
                        if (!string.IsNullOrEmpty(response))
                        {
                            objModel.CreatedBy = userId;
                            await _auditLogService.LogAction("", "ResetPasswordInUserAccount", "");
                            var result = await _repo.UserAccountDALRepo.ResetPasswordInUserAccount(objModel);
                            _repo.Commit();
                            if (result.PasswordReset != null)
                            {
                                switch (result.RetVal)
                                {
                                    case -4:// Null Value Return
                                        responseMsg = result.Msg ?? string.Empty;
                                        break;
                                    case 1:// Success
                                        responseMsg = result.Msg ?? string.Empty;

                                        await _emailService.SendMailMessage(EmailTemplateCode.RESET_PASSWORD,
                                            -1,
                                            userId,
                                            objModel.Password);
                                        break;

                                    case -1://
                                        responseMsg = result.Msg ?? string.Empty;
                                        break;
                                    default:
                                        _logger.LogError(Environment.NewLine);
                                        _logger.LogError("Bad Request occurred while accessing the AddRoleInUserAccount function in User Account api controller");
                                        return BadRequest();
                                }
                            }
                        }
                        else
                        {
                            return BadRequest(Common.Messages.Login);
                        }
                    }
                }
                return Ok(responseMsg);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "  " + ex.StackTrace);
                throw;
            }
        }
        // Unlock User
        [HttpPut("ClientUnlockUser")]
        public async Task<IActionResult> ClientUnlockUser([FromBody] ClientUnlockUser? listOfUnlock)
        {
            if (listOfUnlock == null)
            {
                return BadRequest("Invalid input data.");
            }

            else
            {
                try
                {
                    var responseMsg = string.Empty;
                    using (IUowClientUser _repo = new UowClientUserAccount(_httpContextAccessor))
                    {
                        string? userIdStr = _httpContextAccessor?.HttpContext?.Session?.GetString("strUserID");
                        long userId = !string.IsNullOrEmpty(userIdStr) ? Convert.ToInt64(userIdStr) : 0;
                        string response = _sessionService.GetSession(Common.SessionVariables.Guid);
                        if (!string.IsNullOrEmpty(response))
                        {
                            await _auditLogService.LogAction("", "UnlockUser", "");
                            foreach (var UserGuid in listOfUnlock.Users ?? new List<ClientUnlockUserList?>())
                            {
                                string? guidUser = await _guid.GetGUIDBasedOnClientUserGuid(UserGuid?.UserGuId);
                                if (guidUser == UserGuid?.UserGuId)
                                {
                                    DataTable? dataTable = listOfUnlock?.ConvertToDataTable(listOfUnlock.Users ?? new List<ClientUnlockUserList?>()) ?? new DataTable();
                                    listOfUnlock.UpdatedBy = userId;
                                    var result = await _repo.UserAccountDALRepo.UnlockUserAsync(listOfUnlock);
                                    var msg = "Unlock User successfully.";
                                    _repo.Commit();
                                    if (result.unlockuser != null)
                                    {
                                        switch (result.RetVal)
                                        {
                                            case 1:// Success
                                                responseMsg = msg;
                                                break;

                                            case -1:// Failure or Not exists
                                                responseMsg = result.Msg;
                                                break;
                                            default:
                                                _logger.LogError(Environment.NewLine);
                                                _logger.LogError("Bad Request occurred while accessing the updateUserAccount function in User Account api controller");
                                                return BadRequest();
                                        }

                                    }

                                }

                            }
                        }
                        else
                        {
                            return BadRequest(Common.Messages.Login);
                        }
                        return Ok(responseMsg);
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
}
