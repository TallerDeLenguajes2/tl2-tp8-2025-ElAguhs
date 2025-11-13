using System;
using System.Collections.Generic;
using System.Linq;

namespace tl2_tp8_2025_ElAguhs.Models
{
    public class Presupuesto
    {
        public int IdPresupuesto { get; set; }
        public string ? NombreDestinatario { get; set; }
        public DateTime FechaCreacion { get; set; }
        
        
        public List<PresupuestoDetalle> Detalle { get; set; }

        

        public double MontoPresupuesto()
        {
            
            return Detalle.Where(item => item.Producto != null)
               .Sum(item => item.Producto!.Precio * item.Cantidad);
        }

        public double MontoPresupuestoConIva()
        {
            
            return MontoPresupuesto() * 1.21;
        }

        public int CantidadProductos()
        {
            
            return Detalle.Sum(item => item.Cantidad);
        }

        
        public Presupuesto()
        {
            Detalle = new List<PresupuestoDetalle>();
        }
    }
}