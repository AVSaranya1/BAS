using DataAccessLayer.Interface;
using DataAccessLayer.Model;

namespace DataAccessLayer.Uow.Interface;

public interface IUnitOfWork : IDisposable
{
    IAuditLogDAL auditLogDALRepo { get; }
    IBusinessEntityDAL BusinessEntityDALRepo { get; }
    //IClientOrganisationDAL ClientOrganisationDALRepo { get; }
    //IClientRoleDAL ClientRoleDALRepo { get; }
    //IClientUserAccountDAL ClientUserAccountDALRepo { get; }
    //IClientUserGroupDAL ClientUserGroupDALRepo { get; }
    //ICostCenterDAL CostCenterDALRepo { get; }
    //IDepartmentDAL DepartmentDALRepo { get; }
    //IDivisionDAL DivisionDALRepo { get; }
    IDropdownDAL DropdownDALRepo { get; }
    //IEmailTemplateDAL EmailTemplateDALRepo { get; }
    //IEntityDAL EntityDALRepo { get; }
    IEntityGroupDAL EntityGroupDALRepo { get; }
    //IEntityMenuDAL EntityMenuDALRepo { get; }
    //IForgotPasswordDAL ForgotPasswordDALRepo { get; }
    //IGUIDDAL GuidDALRepo { get; }
    //ILanguageDAL LanguageDALRepo { get; }
    ILoginDAL LoginDALRepo { get; }
    //IMailServerDAL MailServerDALRepo { get; }
    //IMenuDAL MenuDALRepo { get; }
    //INationalityDAL NationalityDALRepo { get; }
    //IOrganisationDAL OrganisationDALRepo { get; }
    //IRoleDAL RoleDALRepo { get; }
    //ITranslationDAL TranslationDALRepo { get; }
    //IUserAccountDAL UserAccountDALRepo { get; }
    //IUserGroupDAL UserGroupDALRepo { get; }
    ILocationDAL LocationDALRepo { get; }

    void SwitchDatabase(DatabaseType databaseType);
    void BeginTransaction();
    Task<int> CompleteAsync();
}
