using DataAccessLayer.Model;
using DataAccessLayer.Uow.Implementation;
using DataAccessLayer.Uow.Interface;
using WebApi.Services.Interface;


namespace WebApi.Services.Implementation
{
    public class AuditLogMasterService : IAuditLogMasterService
    {
        private readonly IUnitOfWork? _uowAuditLog;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuditLogMasterService(IUnitOfWork uowAuditLog, IHttpContextAccessor httpContextAccessor)
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
                UserGuid = UserGuid ?? string.Empty,
                Action = action,
                Token = token ?? string.Empty,
                IPAddress = httpContext?.Connection?.RemoteIpAddress?.ToString(),
                DeviceInfo = httpContext?.Request?.Headers["User-Agent"].ToString(),
                CreatedDateTime = DateTime.UtcNow
            };

            await _uowAuditLog.auditLogDALRepo.LogAudit(auditLog);
            await _uowAuditLog.CompleteAsync();
        }
    }
}

