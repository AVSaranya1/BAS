using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Model
{
    public class EntityMenuModel
    {
        public string? ParentLevelDetailGuid { get; set; }
        public string? LevelGuid { get; set; }
        public string? Level_Detail_Code { get; set; }
        public string? Level_Details_ID { get; set; }
        public string? Level_Detail_Name { get; set; }
        public string? IsLastLevelEntity { get; set; }
        public string? IsAccess { get; set; }
    }
}
