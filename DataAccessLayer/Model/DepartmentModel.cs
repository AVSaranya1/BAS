using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DataAccessLayer.Model
{
    public class AddDept
    {
        [Required(ErrorMessage = "Department Code required")]
        public string? DeptCode { get; set; }

        [Required(ErrorMessage = "Department Description required")]
        public string? DeptDesc { get; set; }
        [Required(ErrorMessage = "IsChild required")]
        public bool? IsChild { get; set; }
        public long? ParentID { get; set; }
        //public long? EntityGroupID { get; set; }
        //public long? BusinessEntityID { get; set; }
        //public long? CostCenterID { get; set; }
        //public long? DeptID { get; set; }
        //public long? DivisionID { get; set; }

        //public string? CreatedBy { get; set; }
        //public string? CreatedDateTime { get; set; }

        public List<DepartmentMapping>? lstDepartmap { get; set; }
    }

    public class EditDept
    {
        [Required(ErrorMessage = "Department Code required")]
        public string? DeptCode { get; set; }

        [Required(ErrorMessage = "Department Description required")]
        public string? DeptDesc { get; set; }
        [Required(ErrorMessage = "IsChild required")]
        public bool? IsChild { get; set; }
        public long? ParentID { get; set; }
        [Required(ErrorMessage = "Department Description required")]
        public Guid? DeptGuid { get; set; }
        public List<DepartmentMapping>? lstDepartmap { get; set; }
    }
    public class DepartmentMapping
    {
        [JsonIgnore]
        public long? EntityGroupID { get; set; }
        public long? BusinessUnitID { get; set; }
        public long? CostCenterID { get; set; }
        [JsonIgnore]
        public long? DeptID { get; set; }
        public long? DivisionID { get; set; }

    }

    public class GetDept
    {
        [JsonPropertyName("lstDeptDetails")]
        public List<Dept>? lstDeptDetails { get; set; }

        [JsonPropertyName("lstDepartmentDetaills")]
        public List<DepartmentDetaills>? lstdeptDetails { get; set; }
    }
    public class Dept
    {
        public Guid? Guid { get; set; }
        public string? DeptCode { get; set; }
        public string? DeptDesc { get; set; }
        public string? DivCodeDesc { get; set; }
        public string? ReferenceID { get; set; }
        public string? Function { get; set; }
        public string? Active { get; set; }
        public DateTime? LastActiveDate { get; set; }
        //public string? LevelDetailsGUID { get; set; }
        //public string? CreatedBy { get; set; }
        //public string? CreatedDateTime { get; set; }
        //public string? ModifiedBy { get; set; }
        //public string? ModifiedDateTime { get; set; }

        

    }
    public class DepartmentDetaills
    {
        public long? DepartmentDetailID { get; set; }
        public long? BusinessEntityID { get; set; }
        public string? BusinessEntityName { get; set; }
        public long? CostCenterID { get; set; }
        public string? CostCenterName { get; set; }
        public long? DeptID { get; set; }
        public string? DeptName { get; set; }
        public long? DivisionID { get; set; }
        public string? DivisionName { get; set; }
    }


    public class GetDeptView
    {
        [JsonPropertyName("lstDeptDetails")]
        public List<DeptView>? lstDeptview { get; set; }

        [JsonPropertyName("lstDepartmentDetaills")]
        public List<DepartmentDetaillsView>? lstdeptDetailsview { get; set; }
    }
    public class DeptView
    {
        public Guid? Guid { get; set; }
        public string? DeptCode { get; set; }
        public string? DeptDesc { get; set; }
        public string? DivCodeDesc { get; set; }
        public string? ReferenceID { get; set; }
        public string? Function { get; set; }
        public string? Active { get; set; }
        public DateTime? LastActiveDate { get; set; }
        //public string? LevelDetailsGUID { get; set; }
        public string? CreatedBy { get; set; }
        public string? CreatedDateTime { get; set; }
        public string? ModifiedBy { get; set; }
        public string? ModifiedDateTime { get; set; }



    }
    public class DepartmentDetaillsView
    {
        public long? DepartmentDetailID { get; set; }
        public long? BusinessEntityID { get; set; }
        public string? BusinessEntityName { get; set; }
        public long? CostCenterID { get; set; }
        public string? CostCenterName { get; set; }
        public long? DeptID { get; set; }
        public string? DeptName { get; set; }
        public long? DivisionID { get; set; }
        public string? DivisionName { get; set; }
        public string? CreatedBy { get; set; }
        public string? CreatedDateTime { get; set; }
        public string? ModifiedBy { get; set; }
        public string? ModifiedDateTime { get; set; }
    }



    public class GetDepartmentInput
    {
        public string? Mode { get; set; }
        public string? UpdatedGuidBy { get; set; }
        public string? DeptGUID { get; set; }
    }

    //public class DeleteDeptList
    //{
    //    public List<DeleteDeptItem>? lstDeleteDept { get; set; }
    //}
    public class DeleteDeptList
    {
        public Guid? Guid { get; set; }
    }

    /// <summary>
    /// /////////////////old
    /// </summary>
    public class DepartmentModel
    {
        //public List<DeptDetails>? lstDeptDetails { get; set; }

        public List<Division>? lstDivision { get; set; }
        public List<Category>? lstCategory { get; set; }
        public List<DivisionSelection>? lstDivisionSelection { get; set; }
        public List<CategorySelection>? lstCategorySelection { get; set; }
    }

    public class Division
    {
        public long? DivisionID { get; set; }
        public string? DivisionGUID { get; set; }
        public string? DivisionDesc { get; set; }
    }

    public class Category
    {
        public long? CategoryID { get; set; }
        public string? CategoryGUID { get; set; }
        public string? CategoryDesc { get; set; }
        public string? Function { get; set; }
    }

    public class DivisionSelection
    {
        public long? DivisionID { get; set; }
        public string? DivisionGUID { get; set; }
        public string? DivisionDesc { get; set; }
        public string? Selected { get; set; } // If "Selected" is a flag, consider using a bool type instead
    }

    public class CategorySelection
    {
        public long? CategoryID { get; set; }
        public string? CategoryGUID { get; set; }
        public string? DivisionGUID { get; set; }
        public string? CategoryDesc { get; set; }
        public string? Selected { get; set; }
    }

    public class DepartmentDetail
    {
        public string? DeptGUID { get; set; }
        public string? DivisionGUID { get; set; }
        public string? CategoryGUID { get; set; }
        public string? Department { get; set; }
    }

    //public class DepartmentInput
    //{
    //    public string? Mode { get; set; }
    //    public string? UpdatedGuidBy { get; set; }
    //    public string? LevelDetailGUID { get; set; }
    //    public string? DeptCode { get; set; }
    //    public string? DeptDesc { get; set; }
    //    public string? LevelGUID { get; set; }
    //    public string? DeptGUID { get; set; }
    //    public string? ColourCode { get; set; }

    //    public long? ReferenceID { get; set; }
    //    public string? Reference_ID { get; set; }
    //    public string? Function { get; set; }
    //    public long? TimeZoneID { get; set; }
    //    public List<DepartmentDetail>? lstDepart { get; set; }
    //}
    public class DeptDeleteResult
    {
        public long? SNo { get; set; }
        public string? DepartmentCode { get; set; }
        public string? RESULT { get; set; } 
        public string? REMARKS { get; set; } 
    }
}
