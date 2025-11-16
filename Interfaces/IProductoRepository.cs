using tl2_tp8_2025_ElAguhs.Models;
using System.Collections.Generic;

namespace tl2_tp8_2025_ElAguhs.Interfaces
{
    public interface IProductoRepository
    {
        void Crear(Producto producto);
        void Modificar(int id, Producto producto);
        Producto? ObtenerPorId(int id);
        List<Producto> Listar();
        void Eliminar(int id);
        
        
    }
}