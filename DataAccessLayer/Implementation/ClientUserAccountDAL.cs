using Dapper;
using DataAccessLayer.Interface;
using DataAccessLayer.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Data;
using DataAccessLayer.Services;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Xml.Linq;
using System.Reflection;

namespace DataAccessLayer.Implementation
{
    public class ClientUserAccountDAL : RepositoryBase, IClientUserAccountDAL
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly EncryptedDecrypt? _encryptedDecrypt;

        public ClientUserAccountDAL(IHttpContextAccessor httpContextAccessor, IConfiguration configuration) : base(new SqlConnection(configuration.GetConnectionString("connection")))
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _encryptedDecrypt = new EncryptedDecrypt(configuration);
            SetDynamicConnection();
        }
        

        private void SetDynamicConnection()
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext != null && httpContext.Items.ContainsKey("connection"))
            {
                string dynamicConnectionString = httpContext.Items["connection"] as string ?? string.Empty;

                if (!string.IsNullOrEmpty(dynamicConnectionString))
                {
                    Connection = new SqlConnection(dynamicConnectionString);
                }
            }
        }
        private void UpdateConnectionString(string? dbName, string? server, string? user, string? password)
        {

            string? masterConnection = _configuration.GetConnectionString("connection");

            var builder = new SqlConnectionStringBuilder(masterConnection)
            {
                DataSource = _encryptedDecrypt?.Decrypt(server),
                UserID = _encryptedDecrypt?.Decrypt(user),
                Password = _encryptedDecrypt?.Decrypt(password),
                InitialCatalog = dbName
            };

            // 🌟 Update DAL Connection
            Connection = new SqlConnection(builder.ToString());

            // 🌟 Store Updated Connection in HttpContext.Items (For Middleware)
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                httpContext.Items["connection"] = builder.ToString();
                httpContext.Session.SetString("DBName", dbName ?? string.Empty);
                httpContext.Session.SetString("InstanceName", server ?? string.Empty);
                httpContext.Session.SetString("DataBaseUserName", user ?? string.Empty);
                httpContext.Session.SetString("DataBasePassword", password ?? string.Empty);
            }
        }

        // 🔹 Fetch Org Details and Update Connection String
        public async Task<List<OrgDetails?>> GetOrgDetailsByUserId(long? userId)
        {
            if (Connection.Database != "MasterData") // To Change the DB Name into Master DB
            {
                string? masterConnection = _configuration.GetConnectionString("connection");
                Connection = new SqlConnection(masterConnection);
            }
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@UserId", userId);
            parameters.Add("@Mode", Common.PageMode.GET_ORG);

            var multi = await Connection.QueryMultipleAsync("sp_UserAccountCreation",
                parameters,
                transaction: Transaction,
                commandType: CommandType.StoredProcedure);

            var orgDetails = multi.Read<OrgDetails?>().ToList();
            return orgDetails;
        }

        public async Task<List<OrgDetails?>> GetOrgDetailsByUserName(string? username)
        {
            if (Connection.Database != "MasterData") // To Change the DB Name into Master DB
            {
                string? masterConnection = _configuration.GetConnectionString("connection");
                Connection = new SqlConnection(masterConnection);
            }
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@UserName", username);
            parameters.Add("@Mode", Common.PageMode.GET_ORG);

            var multi = await Connection.QueryMultipleAsync("sp_ClientUserAccountCreation",
                parameters,
                transaction: Transaction,
                commandType: CommandType.StoredProcedure);

            var orgDetails = multi.Read<OrgDetails?>().ToList();
            return orgDetails;
        }

        public async Task<List<OrgDetails?>> GetOrgDetailsByGUID(string? GUID)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@GUID", GUID);
            parameters.Add("@Mode", Common.PageMode.GET_ORG);

            var multi = await Connection.QueryMultipleAsync("sp_ClientUserAccountCreation",
                parameters,
                transaction: Transaction,
                commandType: CommandType.StoredProcedure);

            var orgDetails = multi.Read<OrgDetails?>().ToList();
            return orgDetails;
        }
        public async Task<(List<ClientUserAccountModel?> InsertedUsers, long? RetVal, string? Msg)> InsertCheckUserAccount(string UserName)
        {
            var strdbname = _httpContextAccessor?.HttpContext?.Session.GetString("DBName");
            var strinstancename = _httpContextAccessor?.HttpContext?.Session.GetString("InstanceName");
            var strusername = _httpContextAccessor?.HttpContext?.Session.GetString("DataBaseUserName");
            var strpassword = _httpContextAccessor?.HttpContext?.Session.GetString("DataBasePassword");
            if (Connection.Database != "MasterData") // To Change the DB Name into Master DB
            {
                string? masterConnection = _configuration.GetConnectionString("connection");
                Connection = new SqlConnection(masterConnection);
            }
            // To Check the DBName is in Default DB Name is Master DB
            
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@UserName",UserName);
            parameters.Add("@RetVal", dbType: DbType.Int64, direction: ParameterDirection.Output);
            parameters.Add("@Msg", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);

            parameters.Add("@Mode", "CHECKUSEREXISTS");

            try
            {
                if (Connection.State == ConnectionState.Closed)
                {
                    Connection.Open();
                }

                using var result = await Connection.QueryMultipleAsync(
                    "sp_UserAccountCreation",
                    parameters,
                    transaction: Transaction,
                    commandType: CommandType.StoredProcedure);

                var InsertedUsers = result.Read<ClientUserAccountModel?>().ToList();
                while (!result.IsConsumed)
                {
                    result.Read();
                }
                long RetVal = parameters.Get<long>("@RetVal");

                string Msg = parameters.Get<string?>("@Msg") ?? "No Records Found";
                _httpContextAccessor?.HttpContext?.Session.SetString("DBName", strdbname ?? string.Empty);
                _httpContextAccessor?.HttpContext?.Session.SetString("InstanceName", strinstancename ?? string.Empty);
                _httpContextAccessor?.HttpContext?.Session.SetString("DataBaseUserName", strusername ?? string.Empty);
                _httpContextAccessor?.HttpContext?.Session.SetString("DataBasePassword", strpassword ?? string.Empty);
                
                return (InsertedUsers ?? new List<ClientUserAccountModel?>(), RetVal, Msg);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                throw;
            }
        }
        // Insert or Update User, Then Change DB Connection
        public async Task<(List<ClientUserAccountModel?> InsertedUsers, List<OrgDetails?> OrgDetails, long? RetVal, string? Msg)> InsertUpdateUserAccount(ClientUserAccountModel? model)
        {
            // To Check the DBName is in Default DB Name is Master DB
            
                if (Connection.Database != "MasterData") // To Change the DB Name into Master DB
                {
                    string? masterConnection = _configuration.GetConnectionString("connection");
                    Connection = new SqlConnection(masterConnection);
                }

                
            
            DateOnly? date = (DateOnly?)model?.UserExpiryDate;
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@UserId", model?.UserId);
            parameters.Add("@UserName", model?.UserName);
            parameters.Add("@Password", EncryptShaAlg.Encrypt(model?.UserPassword));
            parameters.Add("@UserPolicy", model?.UserPolicy);
            parameters.Add("@Language", model?.LanguageID);
            parameters.Add("@Vendor", model?.Vendor);
            parameters.Add("@ContactNo", model?.ContactNo);
            parameters.Add("@TimeZone", model?.TimeZoneID);
            parameters.Add("@DisplayName", model?.DisplayName);
            parameters.Add("@AccountLocked", model?.AccountLocked);
            parameters.Add("@RoleID", model?.RoleID);
            parameters.Add("@PasswordChange", model?.PasswordChange);
            parameters.Add("@Active", model?.Active);
            parameters.Add("@Tenant", model?.Tenant);
            parameters.Add("@TempDeactive", model?.TempDeactive);
            parameters.Add("@EmailID", model?.emailID);
            parameters.Add("@PlatformUser", model?.PlatformUser);
            parameters.Add("@PasswordExpiryDate", model?.PasswordExpiryDate);
            parameters.Add("@UpdatedBy", model?.CreatedBy);
            parameters.Add("@ProfileID", model?.ProfileID);
            parameters.Add("@EffectiveDate", model?.EffectiveDate);
            parameters.Add("@LastActiveDate", date?.ToString("yyyy-MM-dd"));
            parameters.Add("@ClientDBName", model?.DBName);
            parameters.Add("@dtOrgRole", JsonConvert.SerializeObject(model?.UserAccountRoleTable), DbType.String);
            parameters.Add("@RetVal", dbType: DbType.Int64, direction: ParameterDirection.Output);
            parameters.Add("@Msg", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);
            parameters.Add("@UserGuid", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);
            parameters.Add("@Mode", Common.PageMode.ADD_CLIENT_TO_MASTER);
            parameters.Add("@ProfileImg", model?.ProfileImg);

            try
            {
                if (Connection.State == ConnectionState.Closed)
                {
                    Connection.Open();
                }

                using var result = await Connection.QueryMultipleAsync(
                    "sp_UserAccountCreation",
                    parameters,
                    transaction: Transaction,
                    commandType: CommandType.StoredProcedure);

                var InsertedUsers = result.Read<ClientUserAccountModel?>().ToList();
                while (!result.IsConsumed)
                {
                    result.Read();
                }
                var UserGuid = parameters.Get<string?>("@UserGuid");
                if (UserGuid != null) {
                    var vPassword = model?.UserPassword + UserGuid;
                    DynamicParameters parameterPassword = new DynamicParameters();

                    parameterPassword.Add("@UserId", model?.UserId);
                    parameterPassword.Add("@UserName", model?.UserName);
                    parameterPassword.Add("@Password", EncryptShaAlg.Encrypt(vPassword));
                    parameterPassword.Add("@Mode", Common.PageMode.UPDATE_USER_PASSWORD);
                    parameterPassword.Add("@PasswordRetVal", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    using var vPasswordUpdate = await Connection.QueryMultipleAsync(
                        "sp_UserAccountCreation",
                        parameterPassword,
                        transaction: Transaction,
                        commandType: CommandType.StoredProcedure);

                    model!.UserPassword = EncryptShaAlg.Encrypt(vPassword);
                    model.Guid = UserGuid;
                }
                long RetVal = parameters.Get<long>("@RetVal");
                model.UserId = parameters.Get<long>("@RetVal");
                string Msg = parameters.Get<string?>("@Msg") ?? "No Records Found";
                List<OrgDetails?> OrgDetails = new List<OrgDetails?>();
                if (RetVal >= 1)
                {
                    OrgDetails = await GetOrgDetailsByUserId(Convert.ToInt32(RetVal));
                    if (OrgDetails != null && OrgDetails.Any())
                    {
                        var strdbname = _httpContextAccessor?.HttpContext?.Session.GetString("DBName");
                        var strinstancename = _httpContextAccessor?.HttpContext?.Session.GetString("InstanceName");
                        var strusername = _httpContextAccessor?.HttpContext?.Session.GetString("DataBaseUserName");
                        var strpassword = _httpContextAccessor?.HttpContext?.Session.GetString("DataBasePassword");
                        
                            if (OrgDetails.FirstOrDefault() != null)
                            {
                                if ((OrgDetails?.FirstOrDefault().DBName != null && OrgDetails?.FirstOrDefault().DBName != string.Empty) && (OrgDetails?.FirstOrDefault().InstanceName != null && OrgDetails?.FirstOrDefault().InstanceName != string.Empty)
                                     && (OrgDetails?.FirstOrDefault().ConUserName != null && OrgDetails?.FirstOrDefault().ConUserName != string.Empty)
                                     && (OrgDetails?.FirstOrDefault().ConPassword != null && OrgDetails?.FirstOrDefault().ConPassword != string.Empty))
                                {
                                    UpdateConnectionString(OrgDetails?.FirstOrDefault().DBName, OrgDetails?.FirstOrDefault().InstanceName, OrgDetails?.FirstOrDefault().ConUserName, OrgDetails?.FirstOrDefault().ConPassword);
                                    var ClientResult = await InsertUpdateUserAccountClient(model);
                                }
                            }
                        _httpContextAccessor?.HttpContext?.Session.SetString("DBName", strdbname ?? string.Empty);
                        _httpContextAccessor?.HttpContext?.Session.SetString("InstanceName", strinstancename ?? string.Empty);
                        _httpContextAccessor?.HttpContext?.Session.SetString("DataBaseUserName", strusername ?? string.Empty);
                        _httpContextAccessor?.HttpContext?.Session.SetString("DataBasePassword", strpassword ?? string.Empty);
                    }
                            
                }
                return (InsertedUsers ?? new List<ClientUserAccountModel?>(), OrgDetails ?? new List<OrgDetails?>(), RetVal, Msg);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                throw;
            }
        }

        // To Insert into Client Database
        public async Task<(List<ClientUserAccountModel?> InsertedClientUsers, List<OrgDetails?> OrgClientDetails, long? RetClientVal, string? ClientMsg)> InsertUpdateUserAccountClient(ClientUserAccountModel? model)
        {
            DynamicParameters clientParameters = new DynamicParameters();
            clientParameters.Add("@UserId", model?.UserId);
            clientParameters.Add("@UserName", model?.UserName);
            clientParameters.Add("@Password", model?.UserPassword);
            clientParameters.Add("@UserPolicy", model?.UserPolicy);
            clientParameters.Add("@Language", model?.LanguageID);
            clientParameters.Add("@Vendor", model?.Vendor);
            clientParameters.Add("@ContactNo", model?.ContactNo);
            clientParameters.Add("@TimeZone", model?.TimeZoneID);
            clientParameters.Add("@DisplayName", model?.DisplayName);
            clientParameters.Add("@AccountLocked", model?.AccountLocked);
            clientParameters.Add("@RoleID", model?.RoleID);
            clientParameters.Add("@PasswordChange", model?.PasswordChange);
            clientParameters.Add("@Active", model?.Active);
            clientParameters.Add("@Tenant", model?.Tenant);
            clientParameters.Add("@TempDeactive", model?.TempDeactive);
            clientParameters.Add("@EmailID", model?.emailID);
            clientParameters.Add("@UserGuid", model?.Guid);
            clientParameters.Add("@PlatformUser", model?.PlatformUser);
            clientParameters.Add("@PasswordExpiryDate", model?.PasswordExpiryDate);
            clientParameters.Add("@UpdatedBy", model?.CreatedBy);
            clientParameters.Add("@ProfileID", model?.ProfileID);
            clientParameters.Add("@EffectiveDate", model?.EffectiveDate);
           
            clientParameters.Add("@dtOrgRole", JsonConvert.SerializeObject(model?.UserAccountRoleTable), DbType.String);
            clientParameters.Add("@RetVal", dbType: DbType.Int64, direction: ParameterDirection.Output);
            clientParameters.Add("@Msg", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);
            clientParameters.Add("@Mode", Common.PageMode.ADD);
            using var resultClient = await Connection.QueryMultipleAsync(
                                                                        "sp_ClientUserAccountCreation",
                                                                        clientParameters,
                                                                        transaction: Transaction,
                                                                        commandType: CommandType.StoredProcedure);
            List<ClientUserAccountModel?> InsertedUsersClient = new List<ClientUserAccountModel?>();
            List<OrgDetails?> OrgDetails = new List<OrgDetails?>();
            // Ensure the stored procedure returns data before reading
            if (!resultClient.IsConsumed)
            {
                InsertedUsersClient = (await resultClient.ReadAsync<ClientUserAccountModel?>()).ToList();
            }
            // Consume remaining results safely
            while (!resultClient.IsConsumed)
            {
                await resultClient.ReadAsync();
            }
            OrgDetails = await GetOrgDetailsByUserName(Convert.ToString(model.UserName));
            long RetValClient = clientParameters.Get<long>("@RetVal");
            string MsgClient = clientParameters.Get<string?>("@Msg") ?? "No Records Found";
            var UserGuid = clientParameters.Get<string?>("@UserGuid");
            var vPassword = model?.UserPassword + UserGuid;
            return (InsertedUsersClient, OrgDetails, RetValClient, MsgClient);
        }

        // To Update into Client DB

        public async Task<(List<UpdateClientUserAccountModel?> updateClientuseraccount, long? RetValClient, string? MsgClient)> UpdateClientUserAccountAsync(string? username, UpdateClientUserAccountModel? model)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@UserId", model?.UserId);
            parameters.Add("@UserName", username);
            parameters.Add("@Password", model?.UserPassword);
            parameters.Add("@UserPolicy", model?.UserPolicy);
            parameters.Add("@Language", model?.LanguageID);
            parameters.Add("@Vendor", model?.Vendor);
            parameters.Add("@ContactNo", model?.ContactNo);
            parameters.Add("@TimeZone", model?.TimeZoneID);
            parameters.Add("@DisplayName", model?.DisplayName);
            parameters.Add("@AccountLocked", model?.AccountLocked);
            parameters.Add("@RoleID", model?.RoleID);
            parameters.Add("@PasswordChange", model?.PasswordChange);
            parameters.Add("@Active", model?.Active);
            parameters.Add("@Tenant", model?.Tenant);
            parameters.Add("@TempDeactive", model?.TempDeactive);
            parameters.Add("@EmailID", model?.emailID);
            parameters.Add("@ProfileImg", model?.ProfileImg);
            parameters.Add("@PlatformUser", model?.PlatformUser);
            parameters.Add("@PasswordExpiryDate", model?.PasswordExpiryDate);
            parameters.Add("@UpdatedBy", model?.CreatedBy);
            parameters.Add("@ProfileID", model?.ProfileID);
            parameters.Add("@EffectiveDate", model?.EffectiveDate);
            parameters.Add("@GuId", model?.MasterGuid);
            parameters.Add("@dtOrgRights", JsonConvert.SerializeObject(model?.UserAccountOrgTable), DbType.String);
            parameters.Add("@dtOrgRole", JsonConvert.SerializeObject(model?.UserAccountRoleTable), DbType.String);
            parameters.Add("@RetVal", dbType: DbType.Int64, direction: ParameterDirection.Output);
            parameters.Add("@Msg", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);
            parameters.Add("@Mode", Common.PageMode.EDIT);
            using var result = await Connection.QueryMultipleAsync("sp_ClientUserAccountCreation",
                                                                     parameters,
                                                                     transaction: Transaction,
                                                                     commandType: CommandType.StoredProcedure);

            // Process the first result set (roles)
            var UserAccount = result.Read<UpdateClientUserAccountModel?>().ToList();
            // Ensure all result sets are consumed to retrieve the output parameters
            while (!result.IsConsumed)
            {
                result.Read(); // Process remaining datasets
            }

            // Access the output parameters after consuming all datasets
            long retVal = parameters.Get<long>("@RetVal");
            string msg = parameters.Get<string?>("@Msg") ?? "No Records Found";
            // Return the roles list along with output parameters
            return (UserAccount, retVal, msg);
        }
        public async Task<(bool deleteuseraccount, List<DeleteResult> deleteResults)> DeleteUserAccount(long UpdatedBy, ClientDeleteUserAccount deleteUserAccount)
        {
            List<OrgDetails?> OrgDetails = new List<OrgDetails?>();
            if (deleteUserAccount.DeleteDataTable != null)
            {
                foreach (var DeleteResult in deleteUserAccount.DeleteDataTable)
                {
                    // 🌟 Fetch Org Details and Change Connection
                    OrgDetails = await GetOrgDetailsByGUID(DeleteResult.UserGUID);
                    if (OrgDetails != null && OrgDetails.Any())
                    {
                        var strdbname = _httpContextAccessor?.HttpContext?.Session.GetString("DBName");
                        var strinstancename = _httpContextAccessor?.HttpContext?.Session.GetString("InstanceName");
                        var strusername = _httpContextAccessor?.HttpContext?.Session.GetString("DataBaseUserName");
                        var strpassword = _httpContextAccessor?.HttpContext?.Session.GetString("DataBasePassword");

                        foreach (var org in OrgDetails)
                        {

                            if (org != null)
                            {
                                if ((org.DBName != null && org.DBName != string.Empty) && (org.InstanceName != null && org.InstanceName != string.Empty)
                                                && (org.ConUserName != null && org.ConUserName != string.Empty)
                                                && (org.ConPassword != null && org.ConPassword != string.Empty))
                                {
                                    UpdateConnectionString(org.DBName, org.InstanceName, org.ConUserName, org.ConPassword);
                                    var ClientResult = await DeleteClientUserAccount(UpdatedBy, deleteUserAccount);
                                }
                            }

                        }
                        _httpContextAccessor?.HttpContext?.Session.SetString("DBName", strdbname ?? string.Empty);
                        _httpContextAccessor?.HttpContext?.Session.SetString("InstanceName", strinstancename ?? string.Empty);
                        _httpContextAccessor?.HttpContext?.Session.SetString("DataBaseUserName", strusername ?? string.Empty);
                        _httpContextAccessor?.HttpContext?.Session.SetString("DataBasePassword", strpassword ?? string.Empty);
                    }

                }
            }
            if (_httpContextAccessor?.HttpContext != null)
            {
                if (Connection.Database != "MasterData")// To Change the DB Name into Master DB
                {
                    string? masterConnection = _configuration.GetConnectionString("connection");
                    Connection = new SqlConnection(masterConnection);
                }
            }

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@tblDelete", JsonConvert.SerializeObject(deleteUserAccount.UserAccountDeleteTable));
            parameters.Add("@UpdatedBy", UpdatedBy);
            parameters.Add("@Mode", Common.PageMode.DELETE);

            var Result = await Connection.QueryMultipleAsync("sp_UserAccountCreation",
                parameters,
                transaction: Transaction,
                commandType: CommandType.StoredProcedure);
            List<DeleteResult> DeleteUserAccount = (await Result.ReadAsync<DeleteResult>()).ToList();
            while (!Result.IsConsumed)
            {
                await Result.ReadAsync();
            }

            bool res = DeleteUserAccount.Any();



            return (res, DeleteUserAccount.ToList());
        }
        //To Delete Client DB
        public async Task<(bool deleteuseraccount, List<DeleteResult>)> DeleteClientUserAccount(long UpdatedBy, ClientDeleteUserAccount deleteUserAccount)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@tblDelete", JsonConvert.SerializeObject(deleteUserAccount.UserAccountDeleteTable));
            parameters.Add("@UpdatedBy", UpdatedBy);
            parameters.Add("@Mode", Common.PageMode.DELETE);

            var Result = await Connection.QueryMultipleAsync("sp_UserAccountCreation",
                parameters,
                transaction: Transaction,
                commandType: CommandType.StoredProcedure);
            List<DeleteResult?> deleteuseraccount = new List<DeleteResult?>();
            if (!Result.IsConsumed)
            {
                deleteuseraccount = (await Result.ReadAsync<DeleteResult>()).ToList();
            }

            while (!Result.IsConsumed)
            {
                await Result.ReadAsync();
            }

            bool res = deleteuseraccount.Any();


            return (res, deleteuseraccount.ToList());
        }

        
        public async Task<List<GetClientUserAccountModel?>> GetAllUserAccount(long UpdatedBy, string? LevelDetailGuid)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Mode", Common.PageMode.GET);
            parameters.Add("@UserGuid", string.Empty);
            parameters.Add("@UpdatedBy", UpdatedBy);
            parameters.Add("@LevelDetailGuid", LevelDetailGuid);
            
            using var multi = await Connection.QueryMultipleAsync("sp_ClientUserAccountCreation",
                parameters,
                transaction: Transaction,
                commandType: CommandType.StoredProcedure);
            var result = multi.Read<GetClientUserAccountModel?>().ToList();
            foreach (var user in result)
            {
                if (user?.UserExpiryDateTime != null && user?.UserExpiryDateTime.HasValue == true)
                {
                    if (DateOnly.TryParse(user?.UserExpiryDateTime?.ToString("yyyy-MM-dd"), out var parsedDate))
                    {
                        user.UserExpiryDate = parsedDate;
                    }
                }
            }

            return result;
        }

        public async Task<List<ClientUserPolicyName?>> getAllUserPolicyInDropdown(string ClientDBName)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Mode", Common.PageMode.USER_GROUP);
            parameters.Add("@UserId", 0);
            parameters.Add("@ClientDBName", ClientDBName);
            var multi = await Connection.QueryMultipleAsync("sp_ClientUserAccountCreation",
                                                            parameters,
                                                            transaction: Transaction,
                                                            commandType: CommandType.StoredProcedure);
            return multi.Read<ClientUserPolicyName?>().ToList();
        }

        public async Task<List<ClientUserLanguageName?>> getAllUserLanguageInDropdown()
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Mode", Common.PageMode.GET_MASTER_LANGUAGE);
            parameters.Add("@UserId", 0);
            var multi = await Connection.QueryMultipleAsync("sp_ClientUserAccountCreation",
                                                            parameters,
                                                            transaction: Transaction,
                                                            commandType: CommandType.StoredProcedure);
            return multi.Read<ClientUserLanguageName?>().ToList();
        }
        public async Task<List<ClientUserTimeZoneName?>> getAllUserTimeZoneInDropdown()
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Mode", Common.PageMode.GET_TIMEZONE);
            parameters.Add("@UserId", 0);
            var multi = await Connection.QueryMultipleAsync("sp_ClientUserAccountCreation",
                                                            parameters,
                                                            transaction: Transaction,
                                                            commandType: CommandType.StoredProcedure);
            return multi.Read<ClientUserTimeZoneName?>().ToList();
        }

        public async Task<List<GetClientRoleName?>> getAllUserRoleInDropdown(string ClientDBName)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Mode", Common.PageMode.USER_ROLE);
            parameters.Add("@ClientDBName", ClientDBName);
            var multi = await Connection.QueryMultipleAsync("sp_ClientUserAccountCreation",
                                                             parameters,
                                                             transaction: Transaction,
                                                             commandType: CommandType.StoredProcedure);
            return multi.Read<GetClientRoleName?>().ToList();
        }

        //public async Task<(GetClientUserAccount? userAccounts, List<ClientGetUserAccountRole>? UserRoles, List<ClientGetUserAccountModules>? Modules)> GetUserAccountByGUId(string? GUId, long UpdatedBy)
        public async Task<(GetClientUserAccount? userAccounts, List<ClientGetUserAccountRole>? UserRoles)> GetUserAccountByGUId(string? GUId, long UpdatedBy)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@UserGuid", GUId);
            parameters.Add("@UpdatedBy", UpdatedBy);
            parameters.Add("@Mode", Common.PageMode.GET_CLIENT_DETAIL);
            var multi = await Connection.QueryMultipleAsync("sp_UserAccountCreation",
                                                             parameters,
                                                             transaction: Transaction,
                                                             commandType: CommandType.StoredProcedure);
            var UserAccount = (await multi.ReadAsync<GetClientUserAccount>())?.FirstOrDefault();
            var RoleDetails = (await multi.ReadAsync<ClientGetUserAccountRole>())?.ToList();
            //var UserAccountModules = (await multi.ReadAsync<ClientGetUserAccountModules>())?.ToList();
            //return (UserAccount, RoleDetails, UserAccountModules);
            return (UserAccount, RoleDetails);
        }
        public async Task<(List<UpdateClientUserAccountModel?> updateuseraccount, List<OrgDetails?> OrgDetails, long? RetVal, string? Msg)> UpdateUserAccountAsync(UpdateClientUserAccountModel? model)
        {
            // To Check the DBName is in Default DB Name is Master DB
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                if (Connection.Database != "MasterData")// To Change the DB Name into Master DB
                {
                    string? masterConnection = _configuration.GetConnectionString("connection");
                    Connection = new SqlConnection(masterConnection);
                }

            }
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@UserId", 0);
            parameters.Add("@UserName", model?.UserName);
            parameters.Add("@Password", EncryptShaAlg.Encrypt(model?.UserPassword));
            parameters.Add("@UserPolicy", model?.UserPolicy);
            parameters.Add("@Language", model?.LanguageID);
            parameters.Add("@Vendor", model?.Vendor);
            parameters.Add("@ContactNo", model?.ContactNo);
            parameters.Add("@TimeZone", model?.TimeZoneID);
            parameters.Add("@DisplayName", model?.DisplayName);
            parameters.Add("@AccountLocked", model?.AccountLocked);
            parameters.Add("@RoleID", model?.RoleID);
            parameters.Add("@PasswordChange", model?.PasswordChange);
            parameters.Add("@Active", model?.Active);
            parameters.Add("@Tenant", model?.Tenant);
            parameters.Add("@TempDeactive", model?.TempDeactive);
            parameters.Add("@EmailID", model?.emailID);

            parameters.Add("@PlatformUser", model?.PlatformUser);
            parameters.Add("@PasswordExpiryDate", model?.PasswordExpiryDate);
            parameters.Add("@UpdatedBy", model?.CreatedBy);
            parameters.Add("@ProfileID", model?.ProfileID);
            parameters.Add("@EffectiveDate", model?.EffectiveDate);
            parameters.Add("@dtOrgRole", JsonConvert.SerializeObject(model?.UserAccountRoleTable), DbType.String);
            parameters.Add("@RetVal", dbType: DbType.Int64, direction: ParameterDirection.Output);
            parameters.Add("@UserGuid", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);
            parameters.Add("@GuId", model?.MasterGuid);
            parameters.Add("@Msg", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);

            parameters.Add("@Mode", Common.PageMode.EDIT);
            using var result = await Connection.QueryMultipleAsync("sp_ClientUserAccountCreation",
                                                                        parameters,
                                                                        transaction: Transaction,
                                                                        commandType: CommandType.StoredProcedure);

            // Process the first result set (roles)
            var UserAccount = result.Read<UpdateClientUserAccountModel?>().ToList();
            // Ensure all result sets are consumed to retrieve the output parameters
            while (!result.IsConsumed)
            {
                result.Read(); // Process remaining datasets
            }

            // Access the output parameters after consuming all datasets
            long retVal = parameters.Get<long>("@RetVal");
            string msg = parameters.Get<string?>("@Msg") ?? "No Records Found";
            var UserGuid = parameters.Get<string?>("@UserGuid");
            if (UserGuid != null)
            {
                var vPassword = model?.UserPassword + UserGuid;


                DynamicParameters parameterPassword = new DynamicParameters();

                parameterPassword.Add("@UserId", model?.UserId);
                parameterPassword.Add("@UserName", model?.UserName);
                parameterPassword.Add("@Password", EncryptShaAlg.Encrypt(vPassword));
                parameterPassword.Add("@Mode", Common.PageMode.UPDATE_USER_PASSWORD);
                parameterPassword.Add("@PasswordRetVal", dbType: DbType.Int64, direction: ParameterDirection.Output);

                using var vPasswordUpdate = await Connection.QueryMultipleAsync(
                                                                                "sp_UserAccountCreation",
                                                                                 parameterPassword,
                                                                                 transaction: Transaction,
                                                                                commandType: CommandType.StoredProcedure);

                model.UserPassword = EncryptShaAlg.Encrypt(vPassword);
                model.MasterGuid = UserGuid;
            }
            List<OrgDetails?> OrgDetails = new List<OrgDetails?>();
            if (retVal >= 1)
            {
                OrgDetails = await GetOrgDetailsByUserName(model?.UserName);
                if (OrgDetails != null && OrgDetails.Any())
                {
                    var strdbname = _httpContextAccessor?.HttpContext?.Session.GetString("DBName");
                    var strinstancename = _httpContextAccessor?.HttpContext?.Session.GetString("InstanceName");
                    var strusername = _httpContextAccessor?.HttpContext?.Session.GetString("DataBaseUserName");
                    var strpassword = _httpContextAccessor?.HttpContext?.Session.GetString("DataBasePassword");
                    if (OrgDetails.FirstOrDefault() != null)
                    {
                        if ((OrgDetails?.FirstOrDefault().DBName != null && OrgDetails?.FirstOrDefault().DBName != string.Empty) && (OrgDetails?.FirstOrDefault().InstanceName != null && OrgDetails?.FirstOrDefault().InstanceName != string.Empty)
                             && (OrgDetails?.FirstOrDefault().ConUserName != null && OrgDetails?.FirstOrDefault().ConUserName != string.Empty)
                             && (OrgDetails?.FirstOrDefault().ConPassword != null && OrgDetails?.FirstOrDefault().ConPassword != string.Empty))
                        {
                            UpdateConnectionString(OrgDetails?.FirstOrDefault().DBName, OrgDetails?.FirstOrDefault().InstanceName, OrgDetails?.FirstOrDefault().ConUserName, OrgDetails?.FirstOrDefault().ConPassword);
                            var ClientResult = await UpdateClientUserAccountAsync(model?.UserName, model);
                        }
                    }
                    _httpContextAccessor?.HttpContext?.Session.SetString("DBName", strdbname ?? string.Empty);
                    _httpContextAccessor?.HttpContext?.Session.SetString("InstanceName", strinstancename ?? string.Empty);
                    _httpContextAccessor?.HttpContext?.Session.SetString("DataBaseUserName", strusername ?? string.Empty);
                    _httpContextAccessor?.HttpContext?.Session.SetString("DataBasePassword", strpassword ?? string.Empty);
                }
            }
            // Return the roles list along with output parameters
            return (UserAccount ?? new List<UpdateClientUserAccountModel?>(), OrgDetails ?? new List<OrgDetails?>(), retVal, msg);
        }
        public async Task<(List<ClientUnlockUser?> unlockuser, int? RetVal, string? Msg)> UnlockUserAsync(ClientUnlockUser? model)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@UpdatedBy", model?.UpdatedBy, dbType: DbType.Int64, direction: ParameterDirection.Input);
            string jsonUserAccount = JsonConvert.SerializeObject(model?.UnlockTable);

            parameters.Add("@dtUserAccount", jsonUserAccount, DbType.String);
            parameters.Add("@RetVal", dbType: DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@Msg", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);
            parameters.Add("@Mode", Common.PageMode.UNLOCK);
            using var result = await Connection.QueryMultipleAsync("sp_UserAccountCreation",
                                                                        parameters,
                                                                        transaction: Transaction,
                                                                        commandType: CommandType.StoredProcedure);

            // Process the first result set (roles)
            var useraccount = result.Read<ClientUnlockUser?>().ToList();
            // Ensure all result sets are consumed to retrieve the output parameters
            while (!result.IsConsumed)
            {
                result.Read(); // Process remaining datasets
            }

            // Access the output parameters after consuming all datasets
            int retVal = parameters.Get<int?>("@RetVal") ?? -4;
            string msg = parameters.Get<string?>("@Msg") ?? "No Records Found";

            // Return the roles list along with output parameters
            return (useraccount, retVal, msg);

        }
        public async Task<(List<ResetPassword?> PasswordReset, int? RetVal, string? Msg)> ResetPasswordInUserAccount(ResetPassword model)
        {
            if (_httpContextAccessor.HttpContext != null)
            {
                if (Connection.Database != "MasterData") // To Change the DB Name into Master DB
                {
                    string? masterConnection = _configuration.GetConnectionString("connection");
                    Connection = new SqlConnection(masterConnection);
                }
            }
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@UserID", model.UserId);
            parameters.Add("@UserName", model.UserName);
            parameters.Add("@Password", EncryptShaAlg.Encrypt(model.Password));
            parameters.Add("@UpdatedBy", model.CreatedBy);
            parameters.Add("@Mode", Common.PageMode.RESET_PWD_MASTER);
            parameters.Add("@LevelID", model.LevelID);
            parameters.Add("@LevelDetailID", model.LevelDetailID);
            parameters.Add("@RetVal", dbType: DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@Msg", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);
            parameters.Add("@UserGuid", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);

            try
            {
                if (Connection.State == ConnectionState.Closed)
                {
                    Connection.Open();
                }
                using var result = await Connection.QueryMultipleAsync("sp_UserAccountCreation",
                                                                        parameters,
                                                                        transaction: Transaction,
                                                                        commandType: CommandType.StoredProcedure);

                // Process the first result set (Passwordreset)
                var PasswordReset = result.Read<ResetPassword?>().ToList();

                // Ensure all result sets are consumed to retrieve the output parameters
                while (!result.IsConsumed)
                {
                    result.Read(); // Process remaining datasets
                }

                // Access the output parameters after consuming all datasets
                int retVal = parameters.Get<int?>("@RetVal") ?? -4;
                model.UserId = (long?)parameters.Get<int?>("@RetVal") ?? -4;
                string msg = parameters.Get<string?>("@Msg") ?? "No Records Found";
                var UserGuid = parameters.Get<string?>("@UserGuid");
                var vPassword = model?.Password + UserGuid;


                DynamicParameters parameterPassword = new DynamicParameters();

                parameterPassword.Add("@UserId", model?.UserId);
                parameterPassword.Add("@UserName", model?.UserName);
                parameterPassword.Add("@Password", EncryptShaAlg.Encrypt(vPassword));
                parameterPassword.Add("@Mode", Common.PageMode.UPDATE_USER_PASSWORD);
                parameterPassword.Add("@PasswordRetVal", dbType: DbType.Int64, direction: ParameterDirection.Output);

                using var vPasswordUpdate = await Connection.QueryMultipleAsync(
                    "sp_UserAccountCreation",
                    parameterPassword,
                    transaction: Transaction,
                    commandType: CommandType.StoredProcedure);

                model.Password = EncryptShaAlg.Encrypt(vPassword);
                model.UserGuid = UserGuid;
                List<OrgDetails?> OrgDetails = new List<OrgDetails?>();
                if (retVal >= 1)
                {
                    // 🌟 Fetch Org Details and Change Connection
                    OrgDetails = await GetOrgDetailsByUserName(model.UserName);
                    if (OrgDetails != null && OrgDetails.Any())
                    {
                        var strdbname = _httpContextAccessor?.HttpContext?.Session.GetString("DBName");
                        var strinstancename = _httpContextAccessor?.HttpContext?.Session.GetString("InstanceName");
                        var strusername = _httpContextAccessor?.HttpContext?.Session.GetString("DataBaseUserName");
                        var strpassword = _httpContextAccessor?.HttpContext?.Session.GetString("DataBasePassword");

                        foreach (var org in OrgDetails)
                        {

                            if (org != null)
                            {
                                if ((org.DBName != null && org.DBName != string.Empty) && (org.InstanceName != null && org.InstanceName != string.Empty)
                                            && (org.ConUserName != null && org.ConUserName != string.Empty)
                                            && (org.ConPassword != null && org.ConPassword != string.Empty))
                                {
                                    UpdateConnectionString(org.DBName, org.InstanceName, org.ConUserName, org.ConPassword);
                                    var ClientResult = await ResetPasswordInClientUserAccount(model);
                                }
                            }
                        }
                        _httpContextAccessor?.HttpContext?.Session.SetString("DBName", strdbname ?? string.Empty);
                        _httpContextAccessor?.HttpContext?.Session.SetString("InstanceName", strinstancename ?? string.Empty);
                        _httpContextAccessor?.HttpContext?.Session.SetString("DataBaseUserName", strusername ?? string.Empty);
                        _httpContextAccessor?.HttpContext?.Session.SetString("DataBasePassword", strpassword ?? string.Empty);
                    }
                }
                // Return the Password Reset list along with output parameters
                return (PasswordReset, retVal, msg);


            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("There is already an open DataReader"))
            {
                // Log the exception or handle it
                Console.WriteLine("An open DataReader was detected. Ensure proper disposal.");
                throw;
            }

        }

        //To Update Password in Reset Password User Account
        public async Task<(List<ResetPassword?> PasswordReset, long? RetVal, string? Msg)> ResetPasswordInClientUserAccount(ResetPassword model)
        {
            DynamicParameters clientparameters = new DynamicParameters();
            clientparameters.Add("@UserID", model.UserId);
            clientparameters.Add("@UserName", model.UserName);
            clientparameters.Add("@Password", model.Password);
            clientparameters.Add("@UpdatedBy", model.CreatedBy);
            clientparameters.Add("@Mode", Common.PageMode.RESET_PWD_MASTER);
            clientparameters.Add("@LevelID", model.LevelID);
            clientparameters.Add("@LevelDetailID", model.LevelDetailID);
            clientparameters.Add("@RetVal", dbType: DbType.Int64, direction: ParameterDirection.Output);
            clientparameters.Add("@Msg", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);
            clientparameters.Add("@UserGuid", model.UserGuid);

            try
            {
                if (Connection.State == ConnectionState.Closed)
                {
                    Connection.Open();
                }
                using var result = await Connection.QueryMultipleAsync("sp_UserAccountCreation",
                                                                         clientparameters,
                                                                        transaction: Transaction,
                                                                        commandType: CommandType.StoredProcedure);

                // Process the first result set (Passwordreset)
                var PasswordReset = result.Read<ResetPassword?>().ToList();

                // Ensure all result sets are consumed to retrieve the output parameters
                while (!result.IsConsumed)
                {
                    result.Read(); // Process remaining datasets
                }

                // Access the output parameters after consuming all datasets
                long retVal = clientparameters.Get<long>("@RetVal");
                model.UserId = (long)clientparameters.Get<long>("@RetVal");
                string msg = clientparameters.Get<string?>("@Msg") ?? "No Records Found";

                return (PasswordReset, retVal, msg);


            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("There is already an open DataReader"))
            {
                // Log the exception or handle it
                Console.WriteLine("An open DataReader was detected. Ensure proper disposal.");
                throw;
            }

        }
    }
}
