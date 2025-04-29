using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Model
{


    public class EntityGroupModel
    {
        [SwaggerSchema(ReadOnly=true)]
        public string? Mode { get; set; }
        [SwaggerSchema(ReadOnly = true)]
        public long? ID { get; set; }
        [SwaggerSchema(ReadOnly = true)]
        public long? ParentID { get; set; }
        [SwaggerSchema(ReadOnly =true)]
        public Guid? Guid { get; set; }
        [Required(ErrorMessage = Common.Messages.ErrEntityGroupCode)]
        public string? EntityGroupCode { get; set; }
        public string? EntityGroupDesc { get; set; }
        [Required(ErrorMessage = Common.Messages.ErrEntityGroupName)]
        public string? EntityGroupName { get; set; }
        [SwaggerSchema(ReadOnly = true)]
        public string? Logo { get; set; }
        public bool? IsChild { get; set; }
        public Guid? ParentEntityGroupGuid { get; set; }
        public string? IsProject { get; set; }
        
        [SwaggerSchema(ReadOnly = true)]
        public string? CreatedBy { get; set; }
    }
    public class UpdateEntityGroupModel
    {
        [SwaggerSchema(ReadOnly = true)]
        public string? Mode { get; set; }
        public Guid? Guid { get; set; }
        [Required(ErrorMessage = Common.Messages.ErrEntityGroupCode)]
        public string? EntityGroupCode { get; set; }
        public string? EntityGroupDesc { get; set; }
        [Required(ErrorMessage = Common.Messages.ErrEntityGroupName)]
        public string? EntityGroupName { get; set; }
        [SwaggerSchema(ReadOnly = true)]
        public string? Logo { get; set; }
        
        [Required]
        public bool? IsChild { get; set; }
        public Guid? ParentEntityGroupGuid { get; set; }
        public string? IsProject { get; set; }
        public string? ModifiedDateTime { get; set; }
        public string? CreatedUTCDateTime { get; set; }
        public string? ModifiedUTCDateTime { get; set; }
        [SwaggerSchema(ReadOnly = true)]
        public string? CreatedBy { get; set; }
        public string? CreatedDateTime { get; set; }
    }
    public class GetEntityGroupModel
    {
        [JsonIgnore]
        public string? Mode { get; set; }
       
        public long? ID { get; set; }
        
        public long? ParentID { get; set; }
        public Guid? Guid { get; set; }
        
        public string? EntityGroupCode { get; set; }
        public string? EntityGroupDesc { get; set; }
        
        public string? EntityGroupName { get; set; }
        public string? Logo { get; set; }
        public string? LogoUrl { get; set; }
        public bool? IsChild { get; set; }
        public Guid? ParentEntityGroupGuid { get; set; }
        public string? IsProject { get; set; }
        public string? ModifiedBy { get; set; }
        public string? UserGuid { get; set; }
        public string? ModifiedDateTime { get; set; }
        public string? CreatedUTCDateTime { get; set; }
        public string? ModifiedUTCDateTime { get; set; }
        public string? CreatedBy { get; set; }
        public string? CreatedDateTime { get; set; }
       
    }

    public class EntityGroupDel
    {
        public long? ID { get; set; }
        public Guid? Guid { get; set; }
    }
}
