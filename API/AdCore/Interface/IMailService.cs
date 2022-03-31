using AdCore.Settings;

namespace AdCore.Interface
{
    public interface IMailService
    {
        Task SendEmailAsync(Message message);
    }
}
