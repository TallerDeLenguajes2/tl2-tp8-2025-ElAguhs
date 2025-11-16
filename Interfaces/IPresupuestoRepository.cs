using tl2_tp8_2025_ElAguhs.Models;
using System.Collections.Generic;

namespace tl2_tp8_2025_ElAguhs.Interfaces
{
    public interface IPresupuestoRepository
    {
        int Crear(Presupuesto presupuesto);
        void Modificar(int id, Presupuesto presupuesto);
        Presupuesto? ObtenerPorId(int id);
        List<Presupuesto> Listar();
        void Eliminar(int id);
        void AgregarProductoDetalle(int idPresupuesto, int idProducto, int cantidad);

        void QuitarProductoDetalle(int idPresupuesto, int idProducto);
    }
}