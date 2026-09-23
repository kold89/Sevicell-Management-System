using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.ViewModels
{
    public class TemplateContractDto
    {
        // Datos del Vendedor
        public string NombreVendedor { get; set; } = "HUMBERTO";
        public string DniVendedor { get; set; } = "xxxx-xxxx-xxxxx";
        public string empresa { get; set; }

        // Datos del Comprador
        public string NombreComprador { get; set; } = string.Empty;
        public string DniComprador { get; set; } = string.Empty;
        public string DomicilioComprador { get; set; } = string.Empty;

        // Detalles del Dispositivo Mueble
        public string Articulo { get; set; } = "Teléfono Celular";
        public string Marca { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string Imei { get; set; } = string.Empty;
        public string Imei2 { get; set; } = string.Empty;

        // Valores y Parámetros Financieros (Tipos Numéricos Correctos)
        public decimal PrecioTotal { get; set; }
        public decimal Prima { get; set; }
        public decimal SaldoFinanciado => PrecioTotal - Prima; // Se calcula sola
        public int CantidadCuotas { get; set; }
        public decimal ValorCuota { get; set; }
        public string FrecuenciaPago { get; set; } = "Mensual";

        // Fechas e Información Geográfica
        public DateTime FechaInicio { get; set; } = DateTime.Today;
        public DateTime FechaFin { get; set; } = DateTime.Today;
        public string Municipio { get; set; } = "Teupasenti";
        public string Departamento { get; set; } = "El Paraíso";
        public DateTime FechaFirma { get; set; } = DateTime.Now;
    }

    public interface IBuilderTemplateContractDto
    {
        public IBuilderTemplateContractDto AddSeller(SellerInfo dtoComprador);
        public IBuilderTemplateContractDto AddCustomer(CustomerInfo customer);
        public IBuilderTemplateContractDto AddProduct(ProductInfo product);
        public IBuilderTemplateContractDto AddPaymentStructure(PaymentInfo payment);
        public IBuilderTemplateContractDto AddValidityAndPlace(InstallmentPreview cuotaInicial, InstallmentPreview cuotaFinal);
        public TemplateContractDto buildContratoDto();
    }
    public record SellerInfo(string Nombre, string Dni, string Empresa);
    public record CustomerInfo(string Nombre, string Dni, string Domicilio);
    public record ProductInfo(string Articulo, string Marca, string Modelo, string Color, string Imei, string Imei2);
    public record PaymentInfo(decimal PrecioTotal, decimal Prima, int CantidadCuotas, decimal ValorCuota, string Frecuencia);


    public class BuilderContractDto : IBuilderTemplateContractDto
    {
        private TemplateContractDto _template = new();
        public TemplateContractDto buildContratoDto()
        {
            var template = _template;
            _template = new();

            return template;
        }
        public IBuilderTemplateContractDto AddSeller(SellerInfo seller)
        {
            _template.NombreVendedor = seller.Nombre;
            _template.DniVendedor = seller.Dni;
            _template.empresa = seller.Empresa;

            return this;
        }
        public IBuilderTemplateContractDto AddCustomer(CustomerInfo customer)
        {
            _template.NombreComprador = customer.Nombre;
            _template.DniComprador = customer.Dni;
            _template.DomicilioComprador = customer.Domicilio;

            return this;
        }
        public IBuilderTemplateContractDto AddProduct(ProductInfo product)
        {
            _template.Articulo = product.Articulo;
            _template.Marca = product.Marca;
            _template.Modelo = product.Modelo;
            _template.Color = product.Color;
            _template.Imei = product.Imei;
            _template.Imei2 = product.Imei2;

            return this;
        }
        public IBuilderTemplateContractDto AddPaymentStructure(PaymentInfo paymentInfo)
        {

            _template.PrecioTotal = paymentInfo.PrecioTotal;
            _template.Prima = paymentInfo.Prima;
            _template.CantidadCuotas = paymentInfo.CantidadCuotas;
            _template.ValorCuota = paymentInfo.ValorCuota;
            _template.FrecuenciaPago = "Mensual";

            return this;
        }
        public IBuilderTemplateContractDto AddValidityAndPlace(InstallmentPreview primeraCuota, InstallmentPreview ultimaCuota)
        {
            _template.FechaInicio = primeraCuota.DueDate;
            _template.FechaFin = ultimaCuota.DueDate;
            _template.FechaFirma = DateTime.Now;
            _template.Municipio = "Teupasenti";
            _template.Departamento = "El Paraíso";

            return this;
        }
    }
}
