using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.ViewModels
{
    public class creditContractsDTO
    {
  
    }
    public struct Tasa
    {
        public int id { get; set; }
        public string description { get; set; }
    }

    public struct ViewSeller
    {
        public int id { get; set; }
        public string name { get; set; }
        public string dni { get; set; }
        public string company { get; set; }
        public string phone { get; set; }
    }
    public struct viewCustumers
    {
        public int id { get; set; }
        public string name { get; set; }
        public string dni { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public string direction { get; set; }
    }
    public struct viewFrecuency
    {
        public int id { get; set; }
        public string name { get; set; }
    }
    public struct viewProductUnit
    {
        public int id { get; set; }
        public string name { get; set; }
        public string IMEI { get; set; }
        public string IMEI2 { get; set; }
        public decimal priceSales { get; set; }
        public string description { get; set; }
    }
}
