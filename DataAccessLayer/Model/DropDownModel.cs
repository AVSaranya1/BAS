namespace DataAccessLayer.Model;
public class DropDownModel
{
    public Guid Id { get; set; }
    public string value { get; set; } = string.Empty;
    public string text { get; set; } = string.Empty;   
}
public class MasterDropDownModel
{
    public long? Id { get; set; }
    public string value { get; set; } = string.Empty;
    public string text { get; set; } = string.Empty;
}
public class MultiSelectionDropDownModel
{
    public string? Id { get; set; }
    public string value { get; set; } = string.Empty;
    public string text { get; set; } = string.Empty;
}
