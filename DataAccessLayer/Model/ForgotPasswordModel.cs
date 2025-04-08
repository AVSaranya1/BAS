using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text.Json.Serialization;
using DataAccessLayer.Services;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DataAccessLayer.Model
{
    public class ForgotPasswordModel
    {
        public string? EmailID
        {
            get;
            set;
        }
        DataTable Emailtable = new DataTable();
        DataTable MailServerTable = new DataTable();

        public DataTable ConvertToDataTableAsync(List<GetEmailTemplate?> models)
        {
            // Define columns dynamically based on the model's properties
            var properties = typeof(GetEmailTemplate).GetProperties();
            foreach (var property in properties)
            {
                Emailtable.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
            }
            // Add rows dynamically based on the model data
            foreach (var model in models)
            {
                DataRow row = Emailtable.NewRow();
                foreach (var property in properties)
                {

                    row[property.Name] = property.GetValue(model) ?? DBNull.Value;
                }
                Emailtable.Rows.Add(row);
            }
            return Emailtable;
        }

        public DataTable ConvertToDataTableAsync(List<MailServer?> models)
        {
            // Define columns dynamically based on the model's properties
            var properties = typeof(MailServer).GetProperties();
            foreach (var property in properties)
            {
                MailServerTable.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
            }
            // Add rows dynamically based on the model data
            foreach (var model in models)
            {
                DataRow row = MailServerTable.NewRow();
                foreach (var property in properties)
                {

                    row[property.Name] = property.GetValue(model) ?? DBNull.Value;
                }
                MailServerTable.Rows.Add(row);
            }
            return MailServerTable;
        }
        
    }
    public class ForgotPasswordRequest
    {
        public ForgotPasswordModel ForgotPasswordModel { get; set; }
        public EmailTemplate _emailrepository { get; set; }
        public ForgotPasswordRequest()
        {
            _emailrepository = new EmailTemplate(); // Ensure it's never null
            ForgotPasswordModel = new ForgotPasswordModel();
        }
    }
    public class GetForgotPasswordModel
    {
        public Int64? UserID { get; set; }
        public string? UserName { get; set; }
        public Int64? ID { get; set; }
        public string? UserGuid { get; set; }
    }
    public class ForgotPassword
    {
        public string? UserName { get; set; }
        [Required]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{6,}$",
        ErrorMessage = "Password must be at least 6 characters long and contain at least one uppercase letter, one number, and one special character.")]
        public string? Password { get; set; }
        [Required]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{6,}$",
        ErrorMessage = "Password must be at least 6 characters long and contain at least one uppercase letter, one number, and one special character.")]
        public string? ConfirmPassword { get; set; }
    }

}


