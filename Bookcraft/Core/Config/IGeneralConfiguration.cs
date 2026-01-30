using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Config
{
    public interface IGeneralConfiguration
    {
         string AdminEmail { get; set; }
         string DeveloperEmail { get; set; }
         string ChatGptAuthorization { get; set; }
         string ChatGptBaseAddress { get; set; }
         string ChatGptImageAddress { get; set; }
         string BaseUrl { get; set; }
    }
}
