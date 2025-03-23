using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientConsole
{
    public class Env
    {
        public static ConfigModel i { get { return } }
    }

    public class ConfigModel
    {
        public HostModel[] Hosts { get; set; }
    }

    public class HostModel
    {
        public string BaseUrl { get; set; }
        public string Name { get; set; }

    }
}
