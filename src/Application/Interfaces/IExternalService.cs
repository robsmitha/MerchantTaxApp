using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IExternalService
    {
        Task<T> SendAsync<T>(string uri, string token);
        Task<T> PostAsync<T>(string uri, string json, string token);
    }
}
