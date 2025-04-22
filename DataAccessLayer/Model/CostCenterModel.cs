
using DataAccessLayer.Model.BusinessEntity;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text.Json.Serialization;

namespace DataAccessLayer.Model
{
    public class CostCenterModel
    {
        public DataTable CostCenterDeleteTable = new DataTable();
        public DataTable ConvertToDataTable(List<InsertandUpdateCostCenterModelList> dataList)
        {
            CostCenterDeleteTable.Columns.Add("BusinessEntityGuid", typeof(Guid));
            CostCenterDeleteTable.Columns.Add("DivisionGuid", typeof(Guid));
            CostCenterDeleteTable.Columns.Add("DeptGuid", typeof(Guid));

            foreach (var item in dataList)
            {
                foreach (var business in item.BusinessEntityGuidList)
                {
                    foreach (var division in item.DivisionGuidList)
                    {
                        foreach (var deptGuid in item.DeptGuidList)
                        {
                            var row = CostCenterDeleteTable.NewRow();
                            row["BusinessEntityGuid"] = business;
                            row["DivisionGuid"] = division;
                            row["DeptGuid"] = deptGuid;
                            CostCenterDeleteTable.Rows.Add(row);
                        }
                    }
                }
            }
            return CostCenterDeleteTable;
        }
        [JsonIgnore]
        public long ID { get; set; }
        [Required (ErrorMessage =Common.Messages.ErrCostCenterCode)]
        public string? CostCenterCode { get; set; }
        public string? CostCenterDesc { get; set; }
        [JsonIgnore]
        public string? IntegrationCode { get; set; }
        public bool? IsChild { get; set; }
        public Guid ParentCostCenterGuID { get; set; }
        [JsonIgnore]
        public long? ParentCostCenterID { get; set; }
        [JsonIgnore]
        public  string? CreatedBy { get; set; }
        [JsonIgnore]
        public long? ModifiedBy { get; set; }

        public List<InsertandUpdateCostCenterModelList>? InsertandUpdateCostCenterList { get; set; }
    }
    public class UpdateCostCenterModel
    {
        public DataTable CostCenterDeleteTable = new DataTable();
        public DataTable ConvertToDataTable(List<InsertandUpdateCostCenterModelList> dataList)
        {
            CostCenterDeleteTable.Columns.Add("BusinessEntityGuid", typeof(Guid));
            CostCenterDeleteTable.Columns.Add("DivisionGuid", typeof(Guid));
            CostCenterDeleteTable.Columns.Add("DeptGuid", typeof(Guid));

            foreach (var item in dataList)
            {
                foreach (var business in item.BusinessEntityGuidList)
                {
                    foreach (var division in item.DivisionGuidList)
                    {
                        foreach (var deptGuid in item.DeptGuidList)
                        {
                            var row = CostCenterDeleteTable.NewRow();
                            row["BusinessEntityGuid"] = business;
                            row["DivisionGuid"] = division;
                            row["DeptGuid"] = deptGuid;
                            CostCenterDeleteTable.Rows.Add(row);
                        }
                    }
                }
            }
            return CostCenterDeleteTable;
        }
        public Guid GUID {  get; set; }
        [JsonIgnore]
        public long ID { get; set; }
        [Required(ErrorMessage = Common.Messages.ErrCostCenterCode)]
        public string? CostCenterCode { get; set; }
        public string? CostCenterDesc { get; set; }
        public string? IntegrationCode { get; set; }
        public bool? IsChild { get; set; }
        public Guid ParentCostCenterGuID { get; set; }
        [JsonIgnore]
        public long? ParentCostCenterID { get; set; }
        public bool? Active { get; set; }

        [JsonIgnore]
        public string? CreatedBy { get; set; }
        [JsonIgnore]
        public long? ModifiedBy { get; set; }
        public List<InsertandUpdateCostCenterModelList?> InsertandUpdateCostCenterList { get; set; }
    }
    public class GetCostCenterModel
    {
    public Guid GUID { get; set; }
        
        public long ID { get; set; }
        public required string CostCenterCode { get; set; }
        public string? CostCenterDesc { get; set; }
        public string? IntegrationCode { get; set; }
        public bool? IsChild { get; set; }
        public long? ParentCostCenterID { get; set; }
        public bool? Active { get; set; }

        
        public string? CreatedBy { get; set; }
        
        public string? ModifiedBy { get; set; }

        public DateTime? ModifiedLocalDateTime { get; set; }
    }
    public class DeleteCostCenterModel {
        public DataTable CostCenterDeleteTable = new DataTable();
        public DataTable ConvertToDataTable(List<DeleteCostCenterModelList> models)
        {
            // Define columns dynamically based on the model's properties
            var properties = typeof(DeleteCostCenterModelList).GetProperties();
            foreach (var property in properties)
            {
                CostCenterDeleteTable.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
            }

            // Add rows dynamically based on the model data
            foreach (var model in models)
            {
                DataRow row = CostCenterDeleteTable.NewRow();
                foreach (var property in properties)
                {
                    row[property.Name] = property.GetValue(model) ?? DBNull.Value;
                }
                CostCenterDeleteTable.Rows.Add(row);
            }

            return CostCenterDeleteTable;
        }
        public List<DeleteCostCenterModelList>? DeleteDataTable { get; set; }
    }
    public class DeleteCostCenterModelList
    {
        public Guid? Guid { get; set; }
    }
    public class InsertandUpdateCostCenterModelList
    {
        public string? BusinessEntityGuid { get; set; }
        public string? DivisionGuid { get; set; }

        public string? DeptGuid { get; set; }
        public List<Guid> BusinessEntityGuidList => BusinessEntityGuid?
        .Split(',', StringSplitOptions.RemoveEmptyEntries)
        .Select(g => g.Trim())
        .Where(g => Guid.TryParse(g, out _))
        .Select(Guid.Parse)
        .ToList() ?? new List<Guid>();
        public List<Guid> DivisionGuidList => DivisionGuid?
        .Split(',', StringSplitOptions.RemoveEmptyEntries)
        .Select(g => g.Trim())
        .Where(g => Guid.TryParse(g, out _))
        .Select(Guid.Parse)
        .ToList() ?? new List<Guid>();
        public List<Guid> DeptGuidList => DeptGuid?
        .Split(',', StringSplitOptions.RemoveEmptyEntries)
        .Select(g => g.Trim())
        .Where(g => Guid.TryParse(g, out _))
        .Select(Guid.Parse)
        .ToList() ?? new List<Guid>();
    }
    public class GetSelectedCostCenterGuidList
    {
        public GetCostCenterModel CostCenterModel { get; set; }
        public List<MultiSelectionDropDownModel?> BusinessEntityList { get; set; }
        public List<MultiSelectionDropDownModel?> DivisionList { get; set; }
        public List<MultiSelectionDropDownModel?> DepartmentList { get; set; }
    }
    public class GetCostCenterGuidList
    {
        public List<DropDownModel?> BusinessEntityList { get; set; }
        public List<DropDownModel?> DivisionList { get; set; }
        public List<DropDownModel?> DepartmentList { get; set; }
    }
}
