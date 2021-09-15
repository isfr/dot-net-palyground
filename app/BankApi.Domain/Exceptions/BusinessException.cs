using System;
using System.Collections.Generic;

namespace BankApi.Domain.Exceptions
{
    /// <summary>
    /// Business Exception to be thrown on demand
    /// </summary>
    /// <seealso cref="System.Exception" />
    [Serializable]
    public class BusinessException : Exception
    {
        public DateTime Date { get; }

        public IEnumerable<string> Messages { get; set; }

        public BusinessException()
        {
            this.Date = DateTime.Now;
            this.Messages = new List<string>();
        }

        public BusinessException(string message)
            : base(message)
        {
            this.Date = DateTime.Now;
            this.Messages = new List<string>() { message };
        }

        public BusinessException(string message, Exception innerException)
            : base(message, innerException)
        {
            this.Date = DateTime.Now;
            this.Messages = new List<string>() { message };
        }

        public BusinessException(IEnumerable<string> messages)
        {
            this.Date = DateTime.Now;
            this.Messages = messages;
        }
    }
}
