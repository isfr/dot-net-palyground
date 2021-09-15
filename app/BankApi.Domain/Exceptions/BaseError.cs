using System;
using System.Collections.Generic;

namespace BankApi.Domain.Exceptions
{
    public class BaseError
    {
        public string Date { get; set; }

        public IEnumerable<string> Messages { get; set; }

        public BaseError() {}

        public BaseError(BusinessException exception)
        {
            this.Messages = exception.Messages;
            this.Date = exception.Date.ToString("o");
        }

        public BaseError(Exception _)
        {
            this.Messages = new List<string>() { "Something went wrong, try it again later." };
            this.Date = DateTime.Now.ToString("o");
        }
    }
}
