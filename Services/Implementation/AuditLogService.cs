using DataAccessLayer.Implementation;
using DataAccessLayer.Interface;
using DataAccessLayer.Model;
using DataAccessLayer.Uow.Interface;
using WebApi.Services;
using WebApi.Services.Interface;


namespace WebApi.Services.Implementation
{
    public class AuditLogService : IAuditLogService
    {
        private readonly IUnitOfWork _uowAuditLog;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuditLogService(IUnitOfWork uowAuditLog, IHttpContextAccessor httpContextAccessor)
        {
            _uowAuditLog = uowAuditLog;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task LogAction(string action)
        {
            _uowAuditLog.BeginTransaction();
            string? token = null;
            var httpContext = _httpContextAccessor.HttpContext;
            
            var UserGuid = httpContext!.User.FindFirst(JwtClaimType.uGuid)?.Value;
            string authHeader = httpContext.Request.Headers["Authorization"];
                        
            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                token = authHeader.Substring("Bearer ".Length).Trim();
            }

            var auditLog = new AuditLog
            {
                UserGuid = UserGuid,
                Action = action,
                Token = token,
                IPAddress = httpContext?.Connection?.RemoteIpAddress?.ToString(),
                DeviceInfo = httpContext?.Request?.Headers["User-Agent"].ToString(),
                CreatedDateTime = DateTime.UtcNow
            };

            await _uowAuditLog.auditLogDALRepo.LogAudit(auditLog);
            await _uowAuditLog.CompleteAsync();
        }

        public async Task LogAction(string UserGuid, string action, string? token = null)
        {

            _uowAuditLog.BeginTransaction();
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null && httpContext.Session != null)
            {
                if (String.IsNullOrEmpty(token))
                {
                    httpContext.Session.TryGetValue(Common.SessionVariables.Token, out var tokenBytes);
                    token = tokenBytes != null ? System.Text.Encoding.UTF8.GetString(tokenBytes) : null;
                }
                if (String.IsNullOrEmpty(UserGuid))
                {
                    httpContext.Session.TryGetValue(Common.SessionVariables.Guid, out var guidBytes);
                    UserGuid = guidBytes != null ? System.Text.Encoding.UTF8.GetString(guidBytes) : string.Empty;
                }
            }


            var auditLog = new AuditLog
            {
                UserGuid = UserGuid,
                Action = action,
                Token = token,
                IPAddress = httpContext?.Connection?.RemoteIpAddress?.ToString(),
                DeviceInfo = httpContext?.Request?.Headers["User-Agent"].ToString(),
                CreatedDateTime = DateTime.UtcNow
            };

            await _uowAuditLog.auditLogDALRepo.LogAudit(auditLog);
            await _uowAuditLog.CompleteAsync();
        }
    }
}

