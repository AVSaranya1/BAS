using DataAccessLayer.Model;
using DataAccessLayer.Services;

namespace DataAccessLayer.Interface
{
    public interface IForgotPasswordDAL
    {
        Task<(List<ForgotPasswordModel> forgotPasswordModels, int RetVal, string Msg)> InsertUpdateForgotPassword(ForgotPasswordModel LM, string Token);
        Task<(ForgotPassword forgotPasswordModels, int RetVal, string Msg)> ResetPassword(ForgotPassword LM);
        MailServer mailServerPort();
        Task<GetForgotPasswordModel?> ValidateTokenForgotPassword(string? token);
    }
}
