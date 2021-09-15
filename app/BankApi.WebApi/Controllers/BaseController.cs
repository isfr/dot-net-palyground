using BankApi.Domain.Exceptions;

using Microsoft.AspNetCore.Mvc;

namespace BankApi.WebApi.Controllers
{

    public class BaseController : ControllerBase
    {
        protected virtual void ValidateNullParameter<T>(T param, string paramName)
        {
            if (param is null)
            {
                throw new BusinessException($"The parameter {paramName} is mandatory");
            }
        }
    }
}
