using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DataAccessLayer.Model
{
    public class DivisionModel
    {
        public DataTable DepartmentCatTable= new DataTable();
        public DataTable ShiftTable = new DataTable();
        public DataTable ConvertToDataTable(List<DepartmentBusinessDatatable> dataList)
        {
            DepartmentCatTable.Columns.Add("DeptGuid", typeof(Guid));
            DepartmentCatTable.Columns.Add("BusinessEntityGuid", typeof(Guid));
            DepartmentCatTable.Columns.Add("CostCenterGuid", typeof(Guid));

            foreach (var item in dataList)
            {
                foreach (var deptGuid in item.DeptGuidList)
                {
                    var row = DepartmentCatTable.NewRow();
                    row["DeptGuid"] = deptGuid;
                    row["BusinessEntityGuid"] = item.BusinessEntityGuid;
                    row["CostCenterGuid"] = item.CostCenterGuid;
                    DepartmentCatTable.Rows.Add(row);
                }
            }

            return DepartmentCatTable;
        }

        [JsonIgnore]
        public long CreatedBy { get; set; }
        [JsonIgnore]
        public Guid? DivisionGuid { get; } = Guid.NewGuid();
        public string? Division_Code { get; set; }
        public string? Division_Desc { get; set; }
        public bool? IsChild { get; set; }
        public Guid? ParentDivisionGuid { get; set; }
        public List<DepartmentBusinessDatatable?> DepartmentBusinessDatatable { get; set; }
    }
    public class GetDivisionModel 
    {
        [JsonIgnore]
        public Int64 CreatedBy { get; set; }
        public string? Division_Code { get; set; }
        public string? Division_Desc { get; set; }
        public bool? Active { get; set; }
        public DateTime? LastDateActive {  get; set; }
        [JsonIgnore]
        public string? TimeZoneID { get; set; }
        public string? DeptCodeDesc { get; set; }
        public bool? IsChild { get; set; }
        public long? ParentID { get; set; }
        public Guid? DivisionGuid { get; set; }
        public string? CreatedName { get; set; }
        public string? ModifiedName { get; set; }
    }
    public class UpdateDivision
    {
        public DataTable DepartmentCatTable = new DataTable();

        public DataTable ConvertToDataTable(List<DepartmentBusinessDatatable> dataList)
        {
            DepartmentCatTable.Columns.Add("DeptGuid", typeof(Guid));
            DepartmentCatTable.Columns.Add("BusinessEntityGuid", typeof(Guid));
            DepartmentCatTable.Columns.Add("CostCenterGuid", typeof(Guid));

            foreach (var item in dataList)
            {
                foreach (var deptGuid in item.DeptGuidList)
                {
                    var row = DepartmentCatTable.NewRow();
                    row["DeptGuid"] = deptGuid;
                    row["BusinessEntityGuid"] = item.BusinessEntityGuid;
                    row["CostCenterGuid"] = item.CostCenterGuid;
                    DepartmentCatTable.Rows.Add(row);
                }
            }

            return DepartmentCatTable;
        }

        public string? DivisionGuid { get; set; }
        [JsonIgnore]
        public long CreatedBy { get; set; }
        public string? Division_Code { get; set; }
        public string? Division_Desc { get; set; }
        public bool? Active { get; set; }
        public bool? IsChild { get; set; }
        public Guid? ParentDivisionGuid { get; set; }
        public List<DepartmentBusinessDatatable?> DepartmentBusinessDatatable { get; set; }
    }
    public class DeleteDivisionResult {
        public long? SNo { get; set; }
        public string? Result { get; set; }
        public string? Remarks { get; set; }
        public string? DivisionName { get; set; }
    }
    public class DeleteDivision
    {
        public DataTable DeleteDivisionDataTable = new DataTable();
        public DataTable ConvertToDataTable(List<DeleteDivisionList?> dataList)
        {
            DeleteDivisionDataTable.Columns.Add("Guid", typeof(Guid));
            foreach (var item in dataList)
            {
                foreach (var deptGuid in item.DivisionGuidList)
                {
                    var row = DeleteDivisionDataTable.NewRow();
                    row["Guid"] = deptGuid;

                    DeleteDivisionDataTable.Rows.Add(row);
                }
            }

            return DeleteDivisionDataTable ;
        }

        public List<DeleteDivisionList?> DeleteDataTable { get; set; }
        public class DeleteDivisionList
        {
            public string? DivisionGuid { get; set; }
            public List<Guid> DivisionGuidList => DivisionGuid?
        .Split(',', StringSplitOptions.RemoveEmptyEntries)
        .Select(g => g.Trim())
        .Where(g => Guid.TryParse(g, out _))
        .Select(Guid.Parse)
        .ToList() ?? new List<Guid>();
        }
    }
    public class GetDivisionList
    {
        public GetDivisionModel DivisionModel { get; set; }
        public List<DropDownModel?> BusinessEntityDivMapDatatable { get; set; }
        public List<DropDownModel?> CostCenterDivisionDatatable { get; set; }
        public List<DropDownModel?> DepartmentDatatable { get; set; }
    }
    public class DepartmentBusinessDatatable
    {
        public string? DeptGuid {  get; set; }
        public Guid BusinessEntityGuid {  get; set; }
        public Guid CostCenterGuid { get; set; }
        public List<Guid> DeptGuidList => DeptGuid?
        .Split(',', StringSplitOptions.RemoveEmptyEntries)
        .Select(g => g.Trim())
        .Where(g => Guid.TryParse(g, out _))
        .Select(Guid.Parse)
        .ToList() ?? new List<Guid>();

    }
}
