using System.Threading.Tasks;
using The_World.ViewModels;

namespace The_World.Services
{
    public interface IMailService
    {
        Task SendMail(ContactViewModel model);
    }
}
