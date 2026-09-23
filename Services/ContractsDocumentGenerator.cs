using DocumentFormat.OpenXml.Packaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DocumentFormat.OpenXml.Wordprocessing;

using WpfApp1.ViewModels;

namespace WpfApp1.Services
{
    public class ContractsDocumentGenerator
    {
        public void GenerarContratoDocumento(TemplateContractDto datos)
        {
            if (datos == null) return;

            string rutaPlantilla = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plantillas", "Contrato.docx");
            string carpetaDestino = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ContratosGenerados");

            if (!File.Exists(rutaPlantilla))
            {
                MessageBox.Show($"No se encontró la plantilla del contrato en la ruta:\n{rutaPlantilla}",
                                "Error de Configuración", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                if (!Directory.Exists(carpetaDestino))
                    Directory.CreateDirectory(carpetaDestino);

                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string clienteSeguro = string.Concat((datos.NombreComprador ?? "Cliente").Split(Path.GetInvalidFileNameChars()));
                string rutaArchivoFinal = Path.Combine(carpetaDestino, $"Contrato_{clienteSeguro}_{timestamp}.docx");

                File.Copy(rutaPlantilla, rutaArchivoFinal, overwrite: true);

                var valores = new Dictionary<string, string>
                {
                    ["{NombreVendedor}"] = datos.NombreVendedor ?? "",
                    ["{Identidad}"] = datos.DniVendedor ?? "",
                    ["{Empresa}"] = datos.empresa ?? "",
                    ["{NombreComprador}"] = datos.NombreComprador ?? "",
                    ["{DniComprador}"] = datos.DniComprador ?? "",
                    ["{Residencia}"] = datos.DomicilioComprador ?? "",
                    ["{Producto}"] = datos.Articulo ?? "",
                    ["{Marca}"] = datos.Marca ?? "",
                    ["{Modelo}"] = datos.Modelo ?? "",
                    ["{color}"] = datos.Color ?? "",
                    ["{IMEI}"] = datos.Imei ?? "",
                    ["{IMEI2}"] = datos.Imei2 ?? "",
                    ["{PrecioTotal}"] = datos.PrecioTotal.ToString("N2"),
                    ["{Enganche}"] = datos.Prima.ToString("N2"),
                    ["{SaldoFinanciado}"] = datos.SaldoFinanciado.ToString("N2"),
                    ["{CantidadCuotas}"] = datos.CantidadCuotas.ToString(),
                    ["{montoCuotas}"] = datos.ValorCuota.ToString("N2"),
                    ["{FrecuenciaPago}"] = datos.FrecuenciaPago ?? "",
                    ["{diaPago}"] = datos.FechaInicio.Day.ToString("00"),
                    ["{primerPago}"] = datos.FechaInicio.ToString("dd 'de' MMMM 'del' yyyy"),
                    ["{ultimoPago}"] = datos.FechaFin.ToString("dd 'de' MMMM 'del' yyyy"),
                    ["{Municipio}"] = datos.Municipio ?? "",
                    ["{Departamento}"] = datos.Departamento ?? "",
                    ["{Anio}"] = datos.FechaFirma.ToString("dd 'días del mes de' MMMM 'del año' yyyy"),
                };

                using (WordprocessingDocument documento = WordprocessingDocument.Open(rutaArchivoFinal, true))
                {
                    ReemplazarPlaceholders(documento, valores);
                    documento.MainDocumentPart.Document.Save();
                }

                MessageBoxResult abrir = MessageBox.Show($"Contrato generado para {datos.NombreComprador}.\n\n¿Desea abrir el archivo ahora?",
                                                         "Operación Exitosa", MessageBoxButton.YesNo, MessageBoxImage.Information);

                if (abrir == MessageBoxResult.Yes)
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(rutaArchivoFinal) { UseShellExecute = true });
                }
            }
            catch (IOException)
            {
                MessageBox.Show("El archivo destino está abierto en Microsoft Word. Por favor ciérralo e intenta de nuevo.",
                                "Archivo Bloqueado", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error general al procesar el documento:\n{ex.Message}",
                                "Error Crítico", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static void ReemplazarPlaceholders(WordprocessingDocument documento, Dictionary<string, string> valores)
        {
            var body = documento.MainDocumentPart.Document.Body;

            foreach (var paragraph in body.Descendants<Paragraph>())
            {
                ReemplazarEnParrafo(paragraph, valores);
            }
        }

        private static void ReemplazarEnParrafo(Paragraph paragraph, Dictionary<string, string> valores)
        {
            bool huboReemplazo = true;

            while (huboReemplazo)
            {
                huboReemplazo = false;

                var runs = paragraph.Descendants<Run>().ToList();
                if (runs.Count == 0) break;

                var textoPorRun = runs.Select(r => string.Concat(r.Elements<Text>().Select(t => t.Text))).ToList();
                string textoCompleto = string.Concat(textoPorRun);

                var coincidencia = valores.Keys
                    .Select(k => new { Key = k, Index = textoCompleto.IndexOf(k, StringComparison.Ordinal) })
                    .Where(x => x.Index >= 0)
                    .OrderBy(x => x.Index)
                    .FirstOrDefault();

                if (coincidencia == null) break;

                string valorNuevo = valores[coincidencia.Key];
                int inicio = coincidencia.Index;
                int fin = inicio + coincidencia.Key.Length;

                int cursor = 0, runInicio = -1, runFin = -1, offsetInicio = 0, offsetFin = 0;
                for (int i = 0; i < textoPorRun.Count; i++)
                {
                    int largo = textoPorRun[i].Length;
                    int finRun = cursor + largo;

                    if (runInicio == -1 && inicio < finRun)
                    {
                        runInicio = i;
                        offsetInicio = inicio - cursor;
                    }
                    if (fin <= finRun)
                    {
                        runFin = i;
                        offsetFin = fin - cursor;
                        break;
                    }
                    cursor = finRun;
                }

                if (runInicio == -1 || runFin == -1) break;

                if (runInicio == runFin)
                {
                    string texto = textoPorRun[runInicio];
                    string nuevo = texto.Substring(0, offsetInicio) + valorNuevo + texto.Substring(offsetFin);
                    SetRunText(runs[runInicio], nuevo);
                }
                else
                {
                    string textoInicio = textoPorRun[runInicio];
                    string antes = textoInicio.Substring(0, offsetInicio);
                    SetRunText(runs[runInicio], antes + valorNuevo);

                    for (int i = runInicio + 1; i < runFin; i++)
                        SetRunText(runs[i], "");

                    string textoFin = textoPorRun[runFin];
                    string despues = textoFin.Substring(offsetFin);
                    SetRunText(runs[runFin], despues);
                }

                huboReemplazo = true;
            }
        }

        private static void SetRunText(Run run, string nuevoTexto)
        {
            run.RemoveAllChildren<Text>();
            run.AppendChild(new Text(nuevoTexto) { Space = DocumentFormat.OpenXml.SpaceProcessingModeValues.Preserve });
        }

    }
}
