namespace WebApi.Services.Interface;

public interface IAuditLogMasterService
{
    Task LogAction(string action);
}
