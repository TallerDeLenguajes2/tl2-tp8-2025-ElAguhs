using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using tl2_tp8_2025_ElAguhs.Models;

namespace tl2_tp8_2025_ElAguhs.Repositorios
{
    public class PresupuestosRepository
    {
       private readonly string _connectionString = @"Data Source=C:\Users\rodri\OneDrive\Escritorio\tps-taller de lenguajes 2\tp8\tl2-tp8-2025-ElAguhs\tienda.db";

        private SqliteConnection GetConnection()
        {
            return new SqliteConnection(_connectionString);
        }


        public int Crear(Presupuesto presupuesto)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var command = connection.CreateCommand();

                command.CommandText = "INSERT INTO Presupuestos (NombreDestinatario, FechaCreacion) VALUES (@nombre, @fecha)";
                command.Parameters.AddWithValue("@nombre", presupuesto.NombreDestinatario);
                command.Parameters.AddWithValue("@fecha", DateTime.Now);

                command.ExecuteNonQuery();


                command.CommandText = "SELECT last_insert_rowid()";
                long newId = (long)(command.ExecuteScalar() ?? 0L);
                return (int)newId;
            }
        }


        public void AgregarProductoDetalle(int idPresupuesto, int idProducto, int cantidad)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var command = connection.CreateCommand();


                command.CommandText = "INSERT INTO PresupuestosDetalle (idPresupuesto, idProducto, Cantidad) VALUES (@idPres, @idProd, @cant)";
                command.Parameters.AddWithValue("@idPres", idPresupuesto);
                command.Parameters.AddWithValue("@idProd", idProducto);
                command.Parameters.AddWithValue("@cant", cantidad);

                command.ExecuteNonQuery();
            }
        }


        public List<Presupuesto> Listar()
        {
            var lista = new List<Presupuesto>();
            using (var connection = GetConnection())
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT IdPresupuesto, NombreDestinatario, FechaCreacion FROM Presupuestos";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var presupuesto = new Presupuesto
                        {
                            IdPresupuesto = reader.GetInt32(0),
                            NombreDestinatario = reader.GetString(1),
                            FechaCreacion = reader.GetDateTime(2)
                        };
                        lista.Add(presupuesto);
                    }
                }
            }
            return lista;
        }


        public Presupuesto? ObtenerPorId(int id)
        {
            Presupuesto? presupuesto = null;

            using (var connection = GetConnection())
            {
                connection.Open();


                var command = connection.CreateCommand();
                command.CommandText = "SELECT IdPresupuesto, NombreDestinatario, FechaCreacion FROM Presupuestos WHERE IdPresupuesto = @id";
                command.Parameters.AddWithValue("@id", id);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        presupuesto = new Presupuesto
                        {
                            IdPresupuesto = reader.GetInt32(0),
                            NombreDestinatario = reader.GetString(1),
                            FechaCreacion = reader.GetDateTime(2)
                        };
                    }
                }


                if (presupuesto != null)
                {
                    var detailCommand = connection.CreateCommand();
                    detailCommand.CommandText = @"
                        SELECT d.idProducto, d.Cantidad, p.Descripcion, p.Precio 
                        FROM PresupuestosDetalle d
                        JOIN Productos p ON d.idProducto = p.idProducto
                        WHERE d.idPresupuesto = @idPres";

                    detailCommand.Parameters.AddWithValue("@idPres", id);

                    using (var reader = detailCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var producto = new Producto
                            {
                                IdProducto = reader.GetInt32(0),
                                Descripcion = reader.GetString(2),
                                Precio = (int)reader.GetDouble(3)
                            };

                            var detalle = new PresupuestoDetalle
                            {
                                Producto = producto,
                                Cantidad = reader.GetInt32(1)
                            };

                            presupuesto.Detalle.Add(detalle);
                        }
                    }
                }
            }
            return presupuesto;
        }


        public void Eliminar(int id)
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {

                        var command = connection.CreateCommand();
                        command.CommandText = "DELETE FROM PresupuestosDetalle WHERE idPresupuesto = @id";
                        command.Parameters.AddWithValue("@id", id);
                        command.ExecuteNonQuery();


                        command.CommandText = "DELETE FROM Presupuestos WHERE IdPresupuesto = @id";
                        command.ExecuteNonQuery();

                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
        // Método Modificar a añadir en Repositorios/PresupuestosRepository.cs

        public void Modificar(int id, Presupuesto presupuesto)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var command = connection.CreateCommand();

                command.CommandText = "UPDATE Presupuestos SET NombreDestinatario = @nombre WHERE IdPresupuesto = @id";

                command.Parameters.AddWithValue("@nombre", presupuesto.NombreDestinatario);
                command.Parameters.AddWithValue("@id", id);

                command.ExecuteNonQuery();
            }
        }
    }
}