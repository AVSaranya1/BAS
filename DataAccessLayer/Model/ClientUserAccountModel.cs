using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text.Json.Serialization;
namespace DataAccessLayer.Model
{
    public class ClientUserAccountModel
    {
        
        public DataTable UserAccountRoleTable = new DataTable();
        public DataTable ConvertToDataTable(List<ClientRoleNameInUserAccount> models, long CreatedBy, int id)
        {
            // Define columns dynamically based on the model's properties
            var properties = typeof(ClientRoleNameInUserAccount).GetProperties();
            if (UserAccountRoleTable.Columns.Contains("UserID"))
            {
                UserAccountRoleTable.Rows.Clear();
            }
            else
            {
                UserAccountRoleTable.Columns.Add("UserID", typeof(Int64));
            }

            foreach (var property in properties)
            {
                if (!UserAccountRoleTable.Columns.Contains(property.Name))
                {
                    UserAccountRoleTable.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
                }

            }
            if (UserAccountRoleTable.Columns.Contains("CreatedBy"))
            {
                UserAccountRoleTable.Rows.Clear();
            }
            else
            {
                UserAccountRoleTable.Columns.Add("CreatedBy", typeof(Int64));
            }

            // Add rows dynamically based on the model data
            foreach (var model in models)
            {
                DataRow row = UserAccountRoleTable.NewRow();
                foreach (var property in properties)
                {
                    if (id == 0)
                    {
                        row["UserID"] = 0;
                    }
                    else if (id > 0)
                    {
                        row["UserID"] = id;
                    }

                    row[property.Name] = property.GetValue(model) ?? DBNull.Value;
                    row["CreatedBy"] = CreatedBy;

                }
                UserAccountRoleTable.Rows.Add(row);
            }

            return UserAccountRoleTable;
        }
        
        [JsonIgnore]
        public Int64? UserId { get; set; }
        public string? UserName { get; set; }
        public string? UserPassword { get; set; }
        [JsonIgnore]
        public string? Guid { get; set; }
        [JsonIgnore]
        public string? DBName { get; set; }
        public string? PlatformUser { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public string? Vendor { get; set; }
        public string? DisplayName { get; set; }
        public Int64 LanguageID { get; set; }
        public Int64 TimeZoneID { get; set; }
        [EmailAddress]
        public string? emailID { get; set; }
        public string? ContactNo { get; set; }
        [JsonIgnore]
        public Int64 RoleID { get; set; }
        public long UserPolicy { get; set; }
        public string? PasswordChange { get; set; }
        public DateTime? PasswordExpiryDate { get; set; }
        public string? AccountLocked { get; set; }
        [JsonIgnore]
        public int Tenant { get; set; }
        public string? Active { get; set; }
        public string? TempDeactive { get; set; }

        [JsonIgnore]
        public long CreatedBy { get; set; }
        public long ProfileID { get; set; }
        public DateOnly? UserExpiryDate { get; set; }
        public string? ProfileImg { get; set; }

    }
    public class GetClientUserAccountModel
    {
        public long? UserId { get; set; }
        public string? UserName { get; set; }
        public string? UserPassword { get; set; }

        public string? MasterGuid { get; set; }
        public string? PlatformUser { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public string? Vendor { get; set; }
        public string? DisplayName { get; set; }
        public long LanguageID { get; set; }
        [JsonIgnore]
        public long TimeZoneID { get; set; }
        [EmailAddress]
        public string? emailID { get; set; }
        public string? ContactNo { get; set; }
        public int RoleID { get; set; }
        public long UserPolicy { get; set; }
        public string? PasswordChange { get; set; }
        public DateTime? PasswordExpiryDate { get; set; }
        public string? AccountLocked { get; set; }
        public Int64 Tenant { get; set; }
        public string? Active { get; set; }
        public string? TempDeactive { get; set; }

        public string? CreatedBy { get; set; }
        public long ProfileID { get; set; }
        public DateOnly? UserExpiryDate { get; set; }
        [JsonIgnore]
        public DateTime? UserExpiryDateTime { get; set; }

        public string? ProfileImg { get; set; }
        public string? LanguageName { get; set; }
        public string? UserGroupName { get; set; }
        public string? RoleName { get; set; }
        public string? ProfileImgUrl {  get; set; }
    }
    public class UpdateClientUserAccountModel
    {
        public DataTable UserAccountOrgTable = new DataTable();
        public DataTable UserAccountRoleTable = new DataTable();
        public DataTable ConvertToDataTable(List<ClientRoleNameInUserAccount> models, long CreatedBy, string GUid)
        {
            // Define columns dynamically based on the model's properties
            var properties = typeof(ClientRoleNameInUserAccount).GetProperties();
            if (UserAccountRoleTable.Columns.Contains("UserGUID"))
            {
                UserAccountRoleTable.Rows.Clear();
            }
            else
            {
                UserAccountRoleTable.Columns.Add("UserGUID", typeof(string));

            }
            foreach (var property in properties)
            {
                if (!UserAccountRoleTable.Columns.Contains(property.Name))
                {
                    UserAccountRoleTable.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
                }
            }
            if (UserAccountRoleTable.Columns.Contains("CreatedBy"))
            {
                UserAccountRoleTable.Rows.Clear();
            }
            else
            {
                UserAccountRoleTable.Columns.Add("CreatedBy", typeof(Int64));
            }

            // Add rows dynamically based on the model data
            foreach (var model in models)
            {
                DataRow row = UserAccountRoleTable.NewRow();
                foreach (var property in properties)
                {
                    if (GUid == "")
                    {
                        row["UserGUID"] = "";
                    }
                    else if (GUid != "")
                    {
                        row["UserGUID"] = GUid;
                    }

                    row[property.Name] = property.GetValue(model) ?? DBNull.Value;
                    row["CreatedBy"] = CreatedBy;

                }
                UserAccountRoleTable.Rows.Add(row);
            }

            return UserAccountRoleTable;
        }
        
        [JsonIgnore]
        public long? UserId { get; set; }
        public string? UserName { get; set; }
        public string? UserPassword { get; set; }

        public string? MasterGuid { get; set; }
        public string? PlatformUser { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public string? Vendor { get; set; }
        public string? DisplayName { get; set; }
        public long LanguageID { get; set; }
        [JsonIgnore]
        public long TimeZoneID { get; set; }
        [EmailAddress]
        public string? emailID { get; set; }
        public string? ContactNo { get; set; }
        [JsonIgnore]
        public Int64 RoleID { get; set; }
        public long UserPolicy { get; set; }
        public string? PasswordChange { get; set; }
        public DateTime? PasswordExpiryDate { get; set; }
        public string? AccountLocked { get; set; }
        [JsonIgnore]
        public int Tenant { get; set; }
        public string? Active { get; set; }
        public string? TempDeactive { get; set; }

        [JsonIgnore]
        public long? CreatedBy { get; set; }
        public long ProfileID { get; set; }
        public DateOnly? UserExpiryDate { get; set; }
        public string? ProfileImg { get; set; }
    }
    public class ClientUserAccountUpdateRequest
    {
        public UpdateClientUserAccountModel? UserAccount { get; set; }
        public List<ClientRoleNameInUserAccount?> RoleNameList { get; set; } = new List<ClientRoleNameInUserAccount?>();
        
    }
    public class ClientUserAccountInsertRequest
    {
        public ClientUserAccountModel? UserAccount { get; set; }
        public List<ClientRoleNameInUserAccount?> RoleNameList { get; set; } = new List<ClientRoleNameInUserAccount?>();
        
    }

    public class GetClientRoleName
    {
        public string? id { get; set; }

        public string? Text { get; set; }
    }
    public class DeleteClientRoleName
    {
        public List<DeleteClientRoleNameInList> DeleteRoleNames { get; set; } = new List<DeleteClientRoleNameInList>();
        public DataTable UserAccountDeleteRoleTable = new DataTable();
        public DataTable ConvertToDataTable(List<DeleteClientRoleNameInList> models)
        {
            // Define columns dynamically based on the model's properties
            var properties = typeof(DeleteClientRoleNameInList).GetProperties();
            foreach (var property in properties)
            {
                if (!UserAccountDeleteRoleTable.Columns.Contains(property.Name))
                {
                    UserAccountDeleteRoleTable.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
                }

            }
            // Add rows dynamically based on the model data
            foreach (var model in models)
            {
                DataRow row = UserAccountDeleteRoleTable.NewRow();
                foreach (var property in properties)
                {
                    row[property.Name] = property.GetValue(model) ?? DBNull.Value;
                }
                UserAccountDeleteRoleTable.Rows.Add(row);
            }

            return UserAccountDeleteRoleTable;
        }
        [JsonIgnore]
        public long? CreatedBy { get; set; }
    }
    public class DeleteClientRoleNameInList
    {
        public string? UserAccountRoleGuid { get; set; }
    }
    public class ClientRoleName
    {
        [JsonIgnore]
        public long? RoleID { get; set; }
        public string? RoleGUID { get; set; }
        public DateOnly? RoleNameEffectiveDate { get; set; }
        public string? UserGUID { get; set; }
        [JsonIgnore]
        public long? CreatedBy { get; set; }
    }
    public class ClientRoleNameInUserAccount
    {
        public long? RoleID { get; set; }
        public DateOnly? RoleNameEffectiveDate { get; set; }

    }

    public class ClientResetPassword
    {
        [JsonIgnore]
        public long? UserId { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        [JsonIgnore]
        public long? CreatedBy { get; set; }
        public long? LevelID { get; set; }
        public long? LevelDetailID { get; set; }

        [JsonIgnore]
        public string? UserGuid { get; set; }
    }
    public class ClientUserPolicyName
    {
        public string? id { get; set; }
        public string? Text { get; set; }
        
    }
    public class ClientUserLanguageName
    {
        public string? id { get; set; }
        public string? Text { get; set; }
    }
    public class ClientUserTimeZoneName
    {
        public string? id { get; set; }
        public string? Text { get; set; }
    }
    public class ClientGetUserAccountRole
    {
        public string? UserAccountRoleGuid { get; set; }
        public long? RoleID { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public string? AccessToAllClient { get; set; }

    }
    public class ClientGetUserAccountModules
    {
        public string? ItemID { get; set; }
        public string? ItemDesc { get; set; }
        public string? ParentID { get; set; }
        public string? Type { get; set; }
        public string? ParentType { get; set; }
        public string? selected { get; set; }
        public string? color { get; set; }
    }
    public class GetClientUserAccount
    {
        public long? UserID { get; set; }
        public string? UserName { get; set; }
        public string? PromptPasswordChange { get; set; }
        public string? EmailID { get; set; }
        public string? DisplayName { get; set; }
        public string? ContactNo { get; set; }
        public string? UserPolicy { get; set; }
        public string? AccountLocked { get; set; }
        public string? Active { get; set; }
        public DateTime? LastDate { get; set; }
        
        public long? ProfileID { get; set; }
        public string? UserGroupCode { get; set; }
        public string? TimeZone { get; set; }
        public string? LanguageName { get; set; }
        public long? LanguageID { get; set; }
        public string? ProfileImg { get; set; }
        public string? ProfileImgUrl { get; set; }
    }
    public class ClientUserAccountResponse
    {
        public GetClientUserAccount? User { get; set; }
        public List<ClientGetUserAccountRole>? Roles { get; set; }
        //public List<ClientGetUserAccountModules>? Modules { get; set; }
    }
    public class ClientUnlockUser
    {
        public List<ClientUnlockUserList?> Users
        {
            get; set;
        }
        [JsonIgnore]
        public long? UpdatedBy { get; set; }
        public DataTable UnlockTable = new DataTable();
        public DataTable ConvertToDataTable(List<ClientUnlockUserList?> models)
        {
            // Define columns dynamically based on the model's properties
            var properties = typeof(ClientUnlockUserList).GetProperties();
            foreach (var property in properties)
            {
                if (!UnlockTable.Columns.Contains(property.Name))
                {
                    UnlockTable.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
                }
            }
            // Add rows dynamically based on the model data
            foreach (var model in models)
            {
                DataRow row = UnlockTable.NewRow();
                foreach (var property in properties)
                {
                    row[property.Name] = property.GetValue(model) ?? DBNull.Value;
                }
                UnlockTable.Rows.Add(row);
            }

            return UnlockTable;
        }
    }
    public class ClientUnlockUserList
    {
        public string? UserGuId { get; set; }

    }

    public class ClientDeleteUserAccount
    {
        public DataTable UserAccountDeleteTable = new DataTable();

        public DataTable ConvertToDataTable(List<ClientDeleteUserAccountList> models)
        {
            // Define columns dynamically based on the model's properties
            var properties = typeof(ClientDeleteUserAccountList).GetProperties();

            foreach (var property in properties)
            {

                UserAccountDeleteTable.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
            }
            // Add rows dynamically based on the model data
            foreach (var model in models)
            {
                DataRow row = UserAccountDeleteTable.NewRow();
                foreach (var property in properties)
                {

                    row[property.Name] = property.GetValue(model) ?? DBNull.Value;
                }
                UserAccountDeleteTable.Rows.Add(row);
            }

            return UserAccountDeleteTable;
        }


        public List<ClientDeleteUserAccountList>? DeleteDataTable { get; set; }

    }
    public class ClientDeleteUserAccountList
    {
        public string? UserGUID { get; set; }
    }
    public class ClientDeleteResult
    {
        public long? SNo { get; set; }
        public string? Result { get; set; }
        public string? Remarks { get; set; }
        public string? UserName { get; set; }
    }

}