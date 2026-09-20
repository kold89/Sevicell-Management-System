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
        public int ContractNumber { get; set; }
        public string SellerName { get; set; }
        public string DniSeller {  get; set; }
        public string empresa { get; set; }
        public string ProductName { get; set; }
        public string CustomerName { get; set; }
        public string DniCustomer {  get; set; }
        public string CustomerAddress { get; set; }
        public string Status { get; set; }
        public decimal PriceSales { get; set; }
        public decimal DownPayment { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal TotalDebt { get; set; }
        public decimal Balance { get; set; }
        //phone
        public string Imei { get; set; }
        public string Imei2 { get; set; }
        public string Brand { get; set; }
        public string model { get; set; }
        public string colour { get; set; }
        public List<InstallmentPreview> instalments  { get; set; } = new ();
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
