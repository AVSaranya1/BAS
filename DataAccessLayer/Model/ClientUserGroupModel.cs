using System.ComponentModel;
using System.Data;
using System.Text.Json.Serialization;

namespace DataAccessLayer.Model
{
    public class ClientUserGroupModel
    {
        public long UserGroupID { get; set; }
        [DefaultValue(8)]
        public long MinPasswordLength { get; set; }
        [DefaultValue(14)]
        public long MaxPasswordLength { get; set; }
        [DefaultValue(true)]
        public bool? MustContainUppercase { get; set; }
        [DefaultValue(true)]
        public bool? MustContainLowercase {  get; set; }
        [DefaultValue(true)]
        public bool? MustContainDigit { get; set; }
        [DefaultValue(true)]
        public bool? MustContainSpecialCharacter {  get; set; }
        public string? UserGroupCode { get; set; }
        public bool? RestrictFailedLogin { get; set; }
        public long FailedLoginCount { get; set; }
        public bool? PasswordExpiry { get; set; }
        public long PasswordExpiryDays { get; set; }
        public long PasswordExpiryAlertDays { get; set; }
        public bool? RestrictPasswordReuse { get; set; }
        [JsonIgnore]
        public bool? Active { get; } = true;
        [JsonIgnore]
        public long CreatedBy { get; set; }
        public string? twoFAAuthentication { get; set; }
        [DefaultValue(false)]
        public bool? Enforce2FA { get; set; }
        [DefaultValue(5)]
        public long PreviousPasswordCannotReuse { get; set; }
        [DefaultValue(false)]
        public bool RetentionPolicy { get; set; }
        public long RetentionDuration { get; set; }
        [JsonIgnore]
        public string? ClientDBName { get; set; }
        public bool? BlockAccessOnceAllAttemptConsumed { get; set; }
    }

    public class GetClientUserGroupModel
    {
        public long UserGroupID { get; set; }
        public string? UserGroupCode { get; set; }
        public bool? RestrictFailedLogin { get; set; }
        public long FailedLoginCount { get; set; }
        public bool? PasswordExpiry { get; set; }
        public long PasswordExpiryDays { get; set; }
        public long PasswordExpiryAlertDays { get; set; }
        public bool? RestrictPasswordReuse { get; set; }
        public bool? Active { get; set; }
        public string? CreatedBy { get; set; }

        public string? ModifiedBy { get; set; }

        public string? twoFAAuthentication { get; set; }
        
        public long PasswordCount { get; set; }
        public string? UserPolicyGuid { get; set; }
    }
    public class ClientUpdateUserGroupModel
    {
        public string? UserPolicyGuid { get; set; }
        [JsonIgnore]
        public long UserGroupID { get; set; }
        [DefaultValue(8)]
        public long MinPasswordLength { get; set; }
        [DefaultValue(14)]
        public long MaxPasswordLength { get; set; }
        [DefaultValue(true)]
        public bool? MustContainUppercase { get; set; }
        [DefaultValue(true)]
        public bool? MustContainLowercase { get; set; }
        [DefaultValue(true)]
        public bool? MustContainDigit { get; set; }
        [DefaultValue(true)]
        public bool? MustContainSpecialCharacter { get; set; }
        public string? UserGroupCode { get; set; }
        public bool? RestrictFailedLogin { get; set; }
        public long FailedLoginCount { get; set; }
        public bool? PasswordExpiry { get; set; }
        public long PasswordExpiryDays { get; set; }
        public long PasswordExpiryAlertDays { get; set; }
        public bool? RestrictPasswordReuse { get; set; }
        [DefaultValue(5)]
        public long PreviousPasswordCannotReuse { get; set; }
        [JsonIgnore]
        public bool? Active { get; } = true;
        [JsonIgnore]
        public long CreatedBy { get; set; }
        [DefaultValue(false)]
        public bool? Enforce2FA { get; set; }
        public string? twoFAAuthentication { get; set; }
        [DefaultValue(5)]
        public long PreviousPasswordCannotUse { get; set; }
        [DefaultValue(false)]
        public bool RetentionPolicy { get; set; }
        public long RetentionDuration { get; set; }
        [JsonIgnore]
        public string? ClientDBName { get; set; }
        public bool? BlockAccessOnceAllAttemptConsumed { get; set; }
    }

    public class DeleteClientUserGroup
    {
        public DataTable UserGroupDeleteTable = new DataTable();

        public DataTable ConvertToDataTable(List<DeleteClientUserGroupList> models)
        {
            // Define columns dynamically based on the model's properties
            var properties = typeof(DeleteClientUserGroupList).GetProperties();

            foreach (var property in properties)
            {

                UserGroupDeleteTable.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
            }
            // Add rows dynamically based on the model data
            foreach (var model in models)
            {
                DataRow row = UserGroupDeleteTable.NewRow();
                foreach (var property in properties)
                {

                    row[property.Name] = property.GetValue(model) ?? DBNull.Value;
                }
                UserGroupDeleteTable.Rows.Add(row);
            }

            return UserGroupDeleteTable;
        }
        [JsonIgnore]
        public string? ClientDBName { get; set; }

        public List<DeleteClientUserGroupList>? DeleteDataTable { get; set; }

    }
    public class DeleteClientUserGroupList
    {
        public string? UserPolicyGUID { get; set; }

    }
    public class DeleteClientUserGroupResult
    {
        public long? SNo { get; set; }
        public string? Result { get; set; }
        public string? Remarks { get; set; }
        public string? UserGroupName { get; set; }
    }
}
