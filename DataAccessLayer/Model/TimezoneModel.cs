namespace DataAccessLayer.Model;

public class TimezoneModel : DropDownModel
{
    public string? GMTOffset { get; set; }
    public string? TimeOffset { get; set; }
}
