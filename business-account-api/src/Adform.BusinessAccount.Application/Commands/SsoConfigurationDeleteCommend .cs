using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Adform.BusinessAccount.Application.Commands
{
    public  class SsoConfigurationDeleteCommend : IRequest<String?>
    {
        public string Name { get; set; }

        public SsoConfigurationDeleteCommend(string name)
        {
            Name = name;    
        }
    }
}
