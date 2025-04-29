namespace DataAccessLayer.Model;

public class UserModel
{
    public int UserID { get; set; }
    public string UserName { get; set; }
    public string UserDisplayName { get; set; }
    public string UserRoleID { get; set; }
    public string UserRoleGuID { get; set; }
    public string UserRoleName { get; set; }
    public string UserRoleType { get; set; }
    public string DisplayPDPA { get; set; }
    public string UserImgPath { get; set; }
    public string UserGuid { get; set; }
    public string RegionCode { get; set; }
    public string InstanceName { get; set; }
    public string DataBaseUserName { get; set; }
    public string DataBasePassword { get; set; }
    public string DBName { get; set; }
}
