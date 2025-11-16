using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using tl2_tp8_2025_ElAguhs.Models;
using tl2_tp8_2025_ElAguhs.Interfaces;
namespace tl2_tp8_2025_ElAguhs.Repositorios
{
    public class ProductoRepository : IProductoRepository
    {
        private readonly string _connectionString = "Data Source=tienda.db";

        public List<Producto> GetAll()
        {
            
            return Listar();
        }

        private SqliteConnection GetConnection()
        {
            return new SqliteConnection(_connectionString);
        }


        public void Crear(Producto producto)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var command = connection.CreateCommand();


                command.CommandText = "INSERT INTO Productos (Descripcion, Precio) VALUES (@desc, @precio)";
                command.Parameters.AddWithValue("@desc", producto.Descripcion);
                command.Parameters.AddWithValue("@precio", producto.Precio);

                command.ExecuteNonQuery();
            }
        }


        public void Modificar(int id, Producto producto)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var command = connection.CreateCommand();


                command.CommandText = "UPDATE Productos SET Descripcion = @desc, Precio = @precio WHERE idProducto = @id";
                command.Parameters.AddWithValue("@desc", producto.Descripcion);
                command.Parameters.AddWithValue("@precio", producto.Precio);
                command.Parameters.AddWithValue("@id", id);

                command.ExecuteNonQuery();
            }
        }


        public List<Producto> Listar()
        {
            var listaProductos = new List<Producto>();
            using (var connection = GetConnection())
            {
                connection.Open();
                var command = connection.CreateCommand();


                command.CommandText = "SELECT idProducto, Descripcion, Precio FROM [Productos]";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var producto = new Producto
                        {
                            IdProducto = reader.GetInt32(0),
                            Descripcion = reader.GetString(1),

                            Precio = (int)reader.GetDouble(2)
                        };
                        listaProductos.Add(producto);
                    }
                }
            }
            return listaProductos;
        }


        public Producto? ObtenerPorId(int id)
        {
            Producto? producto = null;
            using (var connection = GetConnection())
            {
                connection.Open();
                var command = connection.CreateCommand();


                command.CommandText = "SELECT idProducto, Descripcion, Precio FROM Productos WHERE idProducto = @id";
                command.Parameters.AddWithValue("@id", id);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        producto = new Producto
                        {
                            IdProducto = reader.GetInt32(0),
                            Descripcion = reader.GetString(1),
                            Precio = (int)reader.GetDouble(2)
                        };
                    }
                }
            }
            return producto;
        }


        public void Eliminar(int id)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var command = connection.CreateCommand();


                command.CommandText = "DELETE FROM Productos WHERE idProducto = @id";
                command.Parameters.AddWithValue("@id", id);
                command.ExecuteNonQuery();
            }
        }
    }
}