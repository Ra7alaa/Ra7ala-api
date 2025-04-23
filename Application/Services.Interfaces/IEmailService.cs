namespace Application.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendAdminCredentialsEmailAsync(string email, string userName, string password);
        Task SendEmailAsync(string to, string subject, string body);
        Task SendPasswordResetEmailAsync(string toEmail, string username, string resetToken);
    }
}