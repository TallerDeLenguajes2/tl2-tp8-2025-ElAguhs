using tl2_tp8_2025_ElAguhs.Models;

namespace tl2_tp8_2025_ElAguhs.Interfaces
{
    public interface IUserRepository
    {
       
        Usuario? GetUser(string username, string password); 
    }
}