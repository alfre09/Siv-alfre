using Siv.Web.Modelos;

namespace Siv.Web.Interfaces;

public interface IAuthApiServicio
{
    Task<LoginResponseViewModel> LoginAsync(LoginViewModel modelo);
    Task RegistrarAsync(RegistroViewModel modelo);
}
