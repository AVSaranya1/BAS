using Dapper;
using DataAccessLayer.Model;
using DataAccessLayer.Services;
using DataAccessLayer.Uow.Implementation;
using DataAccessLayer.Uow.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Formats.Asn1;
using System.Net;
using WebApi.Services;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ForgotPasswordController : ApiBaseController
    {
        private readonly ILogger<ForgotPasswordController> _logger;
        private readonly EmailServices _emailService;
        private readonly SessionService _SessionService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ForgotPasswordController(EmailServices emailService,SessionService sessionService,
            ILogger<ForgotPasswordController> logger, IConfiguration configuration, IHttpContextAccessor httpContextAccessor) : base(configuration)
        {
            _logger = logger;
            _emailService= emailService;
            _SessionService= sessionService;
            _httpContextAccessor= httpContextAccessor;
        }
        [HttpPost("InsertUpdateForgotPassword")]
        public async Task<IActionResult> InsertUpdateForgotPassword([FromForm] ForgotPasswordModel objModel)
        {
            try
            {
                if (objModel == null)
                {
                    return BadRequest("Invalid input data.");
                }

                else
                {
                    using (IUowForgotPassword _repo = new UowForgotPassword(_httpContextAccessor))
                    {

                        var result = await _repo.ForgotPasswordDALRepo.InsertUpdateForgotPassword(objModel, Guid.NewGuid().ToString("N"));
                        _repo.Commit();
                        if(result.forgotPasswordModels != null)
                        {
                            switch (result.RetVal)
                            {
                                case >= 1:// success

                                    await _emailService.SendMailMessage(EmailTemplateCode.FORGOT_PASSWORD,
                                                                        Convert.ToInt32(result.RetVal.ToString()),
                                                                        Convert.ToInt64(result.Msg.ToString()),
                                                                        string.Empty);
                                    var emailTemplates = await _emailService.GetForgotPassword(EmailTemplateCode.FORGOT_PASSWORD,
                                                                        Convert.ToInt32(result.RetVal.ToString()),
                                                                        Convert.ToInt64(result.Msg.ToString()),
                                                                        string.Empty);
                                    if (emailTemplates == null || emailTemplates.Count == 0)
                                    {
                                        return NoContent(); // Returns HTTP 204 No Content
                                    }

                                    return Ok(emailTemplates);
                                    
                                    
                                case -1:// User Not Exists
                                    return Ok(result.Msg);
                                
                                default:
                                    _logger.LogError(Environment.NewLine);
                                    _logger.LogError("Bad Request occurred while accessing the InsertUpdateRole function in Role API controller");
                                    return BadRequest();
                            }
                        }
                    }
                }
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "  " + ex.StackTrace);
                throw;
            }
        }
        //Validating the token from email
        [HttpGet("ValidateTokenForgotPassword/{token}")]
        public async Task<IActionResult> ValidateTokenForgotPassword(string? token)
        {
            try
            {
                using (IUowForgotPassword _repo = new UowForgotPassword(_httpContextAccessor))
                {
                    var objvalidatetoken = await _repo.ForgotPasswordDALRepo.ValidateTokenForgotPassword(token);
                    if (objvalidatetoken != null)
                    {
                        _SessionService.SetSession(Common.SessionVariables.UserName, objvalidatetoken.UserName??string.Empty);
                        _SessionService.SetSession(Common.SessionVariables.UserID, Convert.ToString(objvalidatetoken.UserID) ?? "0");
                        _SessionService.SetSession(Common.SessionVariables.Token,Convert.ToString(objvalidatetoken.ID) ??"0");
                        return Ok(objvalidatetoken);
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "  " + ex.StackTrace);
                throw;
            }
        }
        //Reset Password
        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromForm] ForgotPassword objModel)
        {
            try
            {
                if (objModel == null)
                {
                    return BadRequest("Invalid input data.");
                }

                else
                {
                    using (IUowForgotPassword _repo = new UowForgotPassword(_httpContextAccessor))
                    {
                        if (objModel.Password == objModel.ConfirmPassword) {
                            var result = await _repo.ForgotPasswordDALRepo.ResetPassword(objModel);
                            _repo.Commit();
                            if (result.forgotPasswordModels != null)
                            {
                                switch (result.RetVal)
                                {
                                    case >= 1:// success

                                        await _emailService.SendMailMessage(EmailTemplateCode.FORGOT_PASSWORD,
                                                                            Convert.ToInt32(result.RetVal.ToString()),
                                                                            Convert.ToInt64(result.Msg.ToString()),
                                                                            string.Empty);
                                        var emailTemplates = await _emailService.GetForgotPassword(EmailTemplateCode.FORGOT_PASSWORD,
                                                                            Convert.ToInt32(result.RetVal.ToString()),
                                                                            Convert.ToInt64(result.Msg.ToString()),
                                                                            string.Empty);
                                        if (emailTemplates == null || emailTemplates.Count == 0)
                                        {
                                            return NoContent(); // Returns HTTP 204 No Content
                                        }

                                        return Ok(emailTemplates);


                                    case -1:// User Not Exists
                                        return Ok(result.Msg);

                                    default:
                                        _logger.LogError(Environment.NewLine);
                                        _logger.LogError("Bad Request occurred while accessing the InsertUpdateRole function in Role API controller");
                                        return BadRequest();
                                }
                            }

                        }
                        else
                        {
                            return BadRequest(Common.Messages.ConfirmPasswordNotSame);
                        }
                        
                    }
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
}        