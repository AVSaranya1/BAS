namespace WebApi.Services.Interface
{
    public interface IAuditLogService
    {
        Task LogAction(string action);
        Task LogAction(string UserGuid, string action, string? token = null);
    }
}