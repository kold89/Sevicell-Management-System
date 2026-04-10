using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Services
{
    public class ServicesResult<T>
    {
        // 'ServiceResult' es el nombre. '<T>' es el molde genérico.

        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; } // 'Data' será del tipo que tú decidas al instanciarla

        // Método tradicional para éxito
        public static ServicesResult<T> Ok(T data, string message)
        {
            ServicesResult<T> result = new ServicesResult<T>();
            result.Success = true;
            result.Data = data;
            result.Message = message;
            return result;
        }

        // Método tradicional para fallo
        public static ServicesResult<T> Fail(string message)
        {
            ServicesResult<T> result = new ServicesResult<T>();
            result.Success = false;
            result.Message = message;
            // No asignamos Data porque falló
            return result;
        }
    }      
    
}
