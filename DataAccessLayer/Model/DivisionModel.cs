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
        public DataTable ConvertToDataTable(List<DepartmentCategoryDatatable?> models)
        {

            // Define columns dynamically based on the model's properties
            var properties = typeof(DepartmentCategoryDatatable).GetProperties();
            
            
            foreach (var property in properties)
            {
                DepartmentCatTable.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
            }
            // Add rows dynamically based on the model data
            foreach (var model in models)
            {
                DataRow row = DepartmentCatTable.NewRow();
                foreach (var property in properties)
                {
                    
                    row[property.Name] = property.GetValue(model) ?? DBNull.Value;
                }
                DepartmentCatTable.Rows.Add(row);
            }
            return DepartmentCatTable;
        }
        public DataTable ConvertToDataTable(List<ShiftMapDivisionDatatable?> models)
        {

            // Define columns dynamically based on the model's properties
            var properties = typeof(ShiftMapDivisionDatatable).GetProperties();


            foreach (var property in properties)
            {
                ShiftTable.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
            }
            // Add rows dynamically based on the model data
            foreach (var model in models)
            {
                DataRow row = ShiftTable.NewRow();
                foreach (var property in properties)
                {

                    row[property.Name] = property.GetValue(model) ?? DBNull.Value;
                }
                ShiftTable.Rows.Add(row);
            }
            return ShiftTable;
        }
        [JsonIgnore]
        public long CreatedBy { get; set; }
        public string? Division_Code { get; set; }
        public string? Division_Desc { get; set; }
        [JsonIgnore]
        public string? Active { get; set; }
        public string? Reference_ID { get; set; }
        [JsonIgnore]
        public string? Level_Detail_GuID { get; set; }
        [JsonIgnore]
        public string? LevelGuID { get; set; }
        public string? MOM_Integration { get; set; }
    }
    public class GetDivisionModel {

        [JsonIgnore]
        public Int64 CreatedBy { get; set; }
        public string? Division_Code { get; set; }
        public string? Division_Desc { get; set; }
        public string? Active { get; set; }
        public string? Reference_ID { get; set; }
        
        public DateTime? LastDateActive {  get; set; }
        [JsonIgnore]
        public string? TimeZoneID { get; set; }
        public string? DeptCodeDesc { get; set; }
        public string? Function { get; set; }
        public string? LevelGuid { get; set; }
        public string? LevelDetailGuid { get; set; }
        public string? IsMasterDivision { get; set; }
        public string? MomIntegration { get; set; }
        public string? DivisionGuid { get; set; }
        public string? CreatedName { get; set; }
        public string? ModifiedName { get; set; }

   
    }
    public class UpdateDivision
    {
        [JsonIgnore]
        public long CreatedBy { get; set; }
        public string? Division_Code { get; set; }
        public string? Division_Desc { get; set; }
        [JsonIgnore]
        public string? Active { get; set; }
        public string? Reference_ID { get; set; }
        public string? DivisionGuid { get; set; }
        [JsonIgnore]
        public string? Level_Detail_GuID { get; set; }
        [JsonIgnore]
        public string? LevelGuID { get; set; }
        public string? MOM_Integration { get; set; }
        public List<UpdateDepartmentCategoryDatatable?> DepartmentCategoryDatatable { get; set; }
        public List<UpdateShiftMapDivisionDatatable?> ShiftMapDivisionDatatable { get; set; }
    }
    public class DeleteDivisionResult {
        public long? SNo { get; set; }
        public string? Result { get; set; }
        public string? Remarks { get; set; }
        public string? DivisionName { get; set; }
    }
    public class DeleteDivision
    {
     public List<DeleteDivisionList>? DeleteDataTable { get; set; }
        
        public class DeleteDivisionList
        {
            public string? DivisionGuid { get; set; }
        }
    }

    public class InsertDivisionList
    {
        public DivisionModel? InsertDivision { get; set; }
        public List<DepartmentCategoryDatatable?> DepartmentCategoryDatatable { get; set; }
        public List<ShiftMapDivisionDatatable?> ShiftMapDivisionDatatable { get; set; }
    }
    public class GetDivisionList
    {
        public GetDivisionModel DivisionModel { get; set; }
        public List<GetDepartmentDatatable?> DepartmentDatatable { get; set; }
        public List<GetCategoryDatatable?> CategoryDatatable { get; set; }
        public List<GetShiftMapDivisionDatatable?> ShiftMapDivisionDatatable { get; set; }
    }
    

    public class GetShiftMapDivisionDatatable
    {
        public string? ShiftGuid { get; set; }
        public string? ShiftDesc { get; set; }
    }
    public class GetSelectedShiftMapDivisionDatatable
    {
        public string? ShiftGuid { get; set; }
        public string? ShiftDesc { get; set; }
    }

    public class GetDepartmentDatatable
    {
        public string? DeptGuid { get; set; }
        public string? DeptDesc { get; set; }
        public string? selected { get; set; }
    }
    
    public class GetCategoryDatatable 
    {
        public string? CategoryGuid { get; set; }
        public string? CategoryDesc { get; set; }
        public string? selected { get; set; }
        public string? parentGuid { get; set; }
        
    }
    public class DepartmentCategoryDatatable
    {
        public string? DeptGuid {  get; set; }
        public string? CategoryGuid {  get; set; }
    }
    public class UpdateDepartmentCategoryDatatable
    {
        public string? DeptGuid { get; set; }
        public string? CategoryGuid { get; set; }
    }
    public class ShiftMapDivisionDatatable
    {
        public string? ShiftGuid { get; set; }
    }
    public class UpdateShiftMapDivisionDatatable
    {
        public string? ShiftGuid { get; set; }
    }
}
