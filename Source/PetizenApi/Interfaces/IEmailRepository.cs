using System.Threading.Tasks;

namespace PetizenApi.Interfaces
{
    public interface IEmailRepository
    {
        Task<bool> ConfirmatonEmailAsync(string Name, string Email, string Link);

        Task<bool> ConfirmEmailFromBackOfficeAsync(string Name, string Email, string Password, string Link);

        Task<bool> ResetPasswordEmailAsync(string Name, string Email, string Link);
    }
}
