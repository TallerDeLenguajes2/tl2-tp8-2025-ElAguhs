using tl2_tp8_2025_ElAguhs.Interfaces;
using Microsoft.AspNetCore.Http; 
using System;

namespace tl2_tp8_2025_ElAguhs.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository _userRepository; 
        private readonly IHttpContextAccessor _httpContextAccessor; 

        public AuthenticationService(IUserRepository userRepository, IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public bool Login(string username, string password)
        {
            var context = _httpContextAccessor.HttpContext;
            var user = _userRepository.GetUser(username, password);

            if (user == null) return false;

            if (context == null) throw new InvalidOperationException("HttpContext no está disponible.");

            
            context.Session.SetString("IsAuthenticated", "true");
            context.Session.SetString("User", user.User!);
            context.Session.SetString("UserNombre", user.Nombre!);
            context.Session.SetString("Rol", user.Rol!); 

            return true;
        }

        public void Logout()
        {
            var context = _httpContextAccessor.HttpContext;
            if (context == null) throw new InvalidOperationException("HttpContext no está disponible.");
            context.Session.Clear(); 
        }

        public bool IsAuthenticated()
        {
            var context = _httpContextAccessor.HttpContext;
            if (context == null) return false;
            
            return context.Session.GetString("IsAuthenticated") == "true";
        }

        public bool HasAccessLevel(string requiredAccessLevel)
        {
            var context = _httpContextAccessor.HttpContext;
            if (context == null) return false;
            
           
            string? userRole = context.Session.GetString("Rol");
            return userRole == requiredAccessLevel;
        }
    }
}