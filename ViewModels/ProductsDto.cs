using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.ViewModels
{
    public class ProductsDto
    {
        public int id {  get; set; }
        public string name { get; set; }
        public double salesPrice { get; set; }
        public int stock {  get; set; }
        public int minimunStock { get; set; }
        public string category { get; set; }
        public string Brand { get; set; }
        public string status { get; set; }
        public string code  { get; set; }
        public string description { get; set; }
        // REVISIÓN: Asegúrate de que no haya un ";" antes del "=>"
        public bool EsBajoStock => stock <= minimunStock;

    }
}
