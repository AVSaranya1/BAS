using System.Data;

namespace DataAccessLayer.Model.BusinessEntity;

public class DeleteBusinessEntityModel
{
    public DataTable BusinessEntityDeleteTable = new DataTable();
    public DataTable ConvertToDataTable(List<DeleteBusinessEntityModelList> models)
    {
        // Define columns dynamically based on the model's properties
        var properties = typeof(DeleteBusinessEntityModelList).GetProperties();
        foreach (var property in properties)
        {
            BusinessEntityDeleteTable.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
        }

        // Add rows dynamically based on the model data
        foreach (var model in models)
        {
            DataRow row = BusinessEntityDeleteTable.NewRow();
            foreach (var property in properties)
            {
                row[property.Name] = property.GetValue(model) ?? DBNull.Value;
            }
            BusinessEntityDeleteTable.Rows.Add(row);
        }

        return BusinessEntityDeleteTable;
    }
    public List<DeleteBusinessEntityModelList>? DeleteDataTable { get; set; }
}

public class DeleteBusinessEntityModelList
{
    public long? ID { get; set; }
}

//public class DeleteBusinessEntityModelResult
//{
//    public long? SNo { get; set; }
//    public string? LevelDetailGUID { get; set; }
//    public string? Result { get; set; }
//    public string? Remarks { get; set; }
//}