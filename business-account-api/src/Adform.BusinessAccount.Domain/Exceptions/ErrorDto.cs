using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adform.BusinessAccount.Domain.Exceptions
{
    public class ErrorDto
    {
        public string Reason { get; set; }
        public string Message { get;set; }

        public ErrorDto() : this(ErrorReasons.Invalid, Messages.InvalidRequest)
        {
        }

        public ErrorDto(string Message) : this(ErrorReasons.Invalid, Message)
        {
        }

        public ErrorDto(string Reason, string Message)
        {
            this.Reason = string.IsNullOrEmpty(Reason) ? ErrorReasons.Invalid : Reason;
            this.Message = Message;
        }
    }
}
