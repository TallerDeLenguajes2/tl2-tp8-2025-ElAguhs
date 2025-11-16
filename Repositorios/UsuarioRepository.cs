using tl2_tp8_2025_ElAguhs.Interfaces;
using tl2_tp8_2025_ElAguhs.Models;
using Microsoft.Data.Sqlite;
using System.Linq;

namespace tl2_tp8_2025_ElAguhs.Repositorios
{
    public class UsuarioRepository : IUserRepository
    {
        
        private readonly string _connectionString = "Data Source=tienda.db"; 

        private SqliteConnection GetConnection()
        {
            return new SqliteConnection(_connectionString);
        }

        public Usuario? GetUser(string username, string password)
        {
            Usuario? user = null; 
            const string sql = @"
                SELECT Id, Nombre, User, Pass, Rol
                FROM Usuarios
                WHERE User = @Usuario AND Pass = @Contrasena"; 

            using (var connection = GetConnection()) 
            {
                connection.Open(); 
                using var command = new SqliteCommand(sql, connection); 
                
                
                command.Parameters.AddWithValue("@Usuario", username); 
                command.Parameters.AddWithValue("@Contrasena", password); 

                using var reader = command.ExecuteReader(); 
                if (reader.Read()) 
                {
                    
                    user = new Usuario
                    {
                        Id = reader.GetInt32(0),
                        Nombre = reader.GetString(1),
                        User = reader.GetString(2),
                        Pass = reader.GetString(3),
                        Rol = reader.GetString(4)
                    };
                }
            }
            return user; 
        }
    }
}