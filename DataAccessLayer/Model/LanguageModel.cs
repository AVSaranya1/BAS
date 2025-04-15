using System.Data;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Xml.Linq;


namespace DataAccessLayer.Model
{
    public class LanguageModel
    {
        public long LanguageID { get; set; }
        public string? LanguageCode { get; set; }
        public string? LanguageName { get; set; }
        public bool? Active { get; set; }
        [JsonIgnore]
        public long CreatedBy { get; set; }
        [JsonIgnore]
        public Guid Guid { get; } =Guid.NewGuid();
        
    }
    public class UpdateLanguageModel
    {
        public long LanguageID { get; set; }
        public string? LanguageCode { get; set; }
        public string? LanguageName { get; set; }
        public bool? Active { get; set; }
        [JsonIgnore]
        public long CreatedBy { get; set; }
        public Guid? Guid { get; set; }
    }
    public class GetLanguageModel
    {
        public long LanguageID { get; set; }
        public string? LanguageCode { get; set; }
        public string? Description { get; set; }
        public bool? Active { get; set; }
        
        public string? CreatedBy { get; set; }
        public Guid? Guid { get; set; }
    }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum LanguageNameEnum
    {
        [EnumMember(Value = "0")]
        Select,
        [EnumMember(Value = "1")]
        English,
        [EnumMember(Value = "2")]
        Tamil,
        [EnumMember(Value = "3")]
        Chinese
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public class LanguageName
    {
        public long Value { get; set; }
        public string? Text { get; set; }

        // Static helper method to convert the enum to a list of LanguageName
        public static List<LanguageName> GetAllLanguages()
        {
            return Enum.GetValues(typeof(LanguageNameEnum))
                       .Cast<LanguageNameEnum>()
                       .Select(e => new LanguageName
                       {
                           Value = Convert.ToInt64(e), // Map enum's integer value
                           Text = e.ToString()        // Map enum's string representation
                       })
                       .ToList();
        }
    }
}
