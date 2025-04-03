using System.Data;

namespace DataAccessLayer.Model;

public class EntityModel
{
    public string? LevelGUID { get; set; }
    public string? LevelDetailGUID { get; set; }
    public string? LevelDetailCode { get; set; }
    public string? LevelDetailName { get; set; }
    public string? CountryGUID { get; set; }
    public string? TimeZoneGUID { get; set; }
    public string? CurrencyGUID { get; set; }
    public string? Logo { get; set; }
    public string? UpdatedBy { get; set; }
    public string? Latitude { get; set; }
    public string? Longitude { get; set; }
    public string? Radius { get; set; }
    public string? Active { get; set; }
    public string? ProjectAddress { get; set; }
    public string? BuilderUEN_No { get; set; }
    public string? ProjectBP_No { get; set; }
    public string? ProjectName { get; set; }
    public string? BuilderName { get; set; }
    public string? ProjectStartDate { get; set; }
    public string? ProjectEndDate { get; set; }
    public string? SiteCode { get; set; }
    public string? CreatedBy { get; set; }
    public string? ModifiedBy { get; set; }
    public string? Country { get; set; }
    public string? Currency { get; set; }
    public string? CurrencyDescription { get; set; }
    public string? TimeZoneLocation { get; set; }
    public string? TimeZoneGMT { get; set; }
    }

public class DeleteLevelDetail
{
    public DataTable LevelDetailDeleteTable = new DataTable();
    public DataTable ConvertToDataTable(List<DeleteLevelDetailList> models)
    {
        // Define columns dynamically based on the model's properties
        var properties = typeof(DeleteLevelDetailList).GetProperties();

        foreach (var property in properties)
        {
            LevelDetailDeleteTable.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
        }
        // Add rows dynamically based on the model data
        foreach (var model in models)
        {
            DataRow row = LevelDetailDeleteTable.NewRow();
            foreach (var property in properties)
            {

                row[property.Name] = property.GetValue(model) ?? DBNull.Value;
            }
            LevelDetailDeleteTable.Rows.Add(row);
        }

        return LevelDetailDeleteTable;
    }
    public List<DeleteLevelDetailList>? DeleteDataTable { get; set; }
}

public class DeleteLevelDetailList
{   
    public string? Guid { get; set; }
}

public class DeleteLevelDetailResult
{
    public long? SNo { get; set; }
    public string? LevelDetailGUID { get; set; }
    public string? Result { get; set; }
    public string? Remarks { get; set; }
}

