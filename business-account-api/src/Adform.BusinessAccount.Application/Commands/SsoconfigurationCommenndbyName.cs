using Adform.BusinessAccount.Contracts.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adform.BusinessAccount.Application.Commands
{
    public class SsoconfigurationCommenndbyName : IRequest<SsoConfiguration>
    {
        public string name { get; set; }

        public SsoconfigurationCommenndbyName ssoconfigurationcommendbyname { get; set };
    }
}
