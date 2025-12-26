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
    }
}
