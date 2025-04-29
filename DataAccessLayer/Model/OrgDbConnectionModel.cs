namespace DataAccessLayer.Model;

public class OrgDbConnectionModel
{    public string Guid { get; set; }
    public string OrgCode { get; set; } = string.Empty;
    public string OrgName { get; set; } = string.Empty;
    public string DBName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string InstanceName { get; set; } = string.Empty;
}
