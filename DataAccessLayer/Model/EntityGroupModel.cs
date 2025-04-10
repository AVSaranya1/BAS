using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Model
{


    public class EntityGroupModel
    {
        public string? Mode { get; set; }
        public long? ID { get; set; }
        public long? ParentID { get; set; }
        public Guid? Guid { get; set; }
        public string? EntityGroupCode { get; set; }
        public string? EntityGroupDesc { get; set; }
        public string? Logo { get; set; }
        public bool? IsChild { get; set; }
        public string? ParentEntityGroupGuid { get; set; }
        public string? IsProject { get; set; }
        public long? ModifiedBy { get; set; }
        public string? UserGuid { get; set; }
        public string? ModifiedDateTime { get; set; }
        public string? CreatedUTCDateTime { get; set; }
        public string? ModifiedUTCDateTime { get; set; }
        public string? CreatedBy { get; set; }
        public string? CreatedDateTime { get; set; }
        public List<EntityGroupDel>? LevelInfo { get; set; } // For TVP
    }

    public class EntityGroupDel
    {
        public long? ID { get; set; }
        public Guid? Guid { get; set; }
    }
}
