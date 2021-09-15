﻿using System;
using System.Linq;
using System.Threading.Tasks;

using BankApi.Domain.Exceptions;
using BankApi.Service;
using BankApi.WebApi.Controllers;
using BankApi.WebApi.Filters;
using BankApi.WebApi.Mappers;
using BankApi.WebApi.Models;

using Microsoft.AspNetCore.Mvc;

using Swashbuckle.AspNetCore.Annotations;

namespace BankApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : BaseController
    {
        private readonly ICustomerService customerService;

        public CustomerController(ICustomerService customerService)
        {
            this.customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
        }

        /// <summary>
        /// Gets the customer information.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <response code="200">OK. With the customer information</response>
        /// <response code="400">Bad request. When there is a failure in the request format, expected headers, or the payload can't be unmarshalled.</response>
        /// <response code="404">Not Found. When there is no customer with the given id.</response>
        [HttpGet]
        [Route("GetCustomer/{id}")]
        [SwaggerResponse(statusCode: 200, type: typeof(CustomerDto), description: "The customer information")]
        [SwaggerResponse(statusCode: 400, type: typeof(BaseError), description: "Error generated by validations or bussines rules.")]
        [SwaggerResponse(statusCode: 404, description: "There was no customer with that id.")]
        public async Task<IActionResult> GetCustomer(int? id)
        {
            this.ValidateNullParameter(id, nameof(id));

            var customer = await this.customerService.GetCustomerInfo(id.Value);
            if (customer is null)
            {
                return NotFound();
            }

            return Ok(customer.ToDto());
        }

        /// <summary>
        /// Creates a new customer.
        /// </summary>
        /// <param name="newCustomer">The new customer.</param>
        /// <response code="200">OK. With the new customer id</response>
        /// <response code="400">Bad request. When there is a failure in the request format, expected headers, or the payload can't be unmarshalled.</response>
        [HttpPost]
        [Route("CreateNewCustomer")]
        [ValidateModelState]
        [SwaggerResponse(statusCode: 200, type: typeof(int), description: "The id of the new customer")]
        [SwaggerResponse(statusCode: 400, type: typeof(BaseError) , description: "Error generated by validations or bussines rules.")]
        public async Task<IActionResult> CreateNewCustomer([FromBody] NewCustomerDto newCustomer)
        {
            this.ValidateNullParameter(newCustomer, nameof(newCustomer));

            var customerId = await this.customerService.CreateNewCustomer(newCustomer.CustomerName);

            return Ok(customerId);
        }

        /// <summary>
        /// Gets the customer accounts.
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        /// <response code="200">OK. With the list of accounts</response>
        /// <response code="400">Bad request. When there is a failure in the request format, expected headers, or the payload can't be unmarshalled.</response>
        /// <response code="404">Not Found. When there accounts linked to the customer</response>
        [HttpGet]
        [Route("GetCustomerAccounts/{customerId}")]
        [SwaggerResponse(statusCode: 200, type: typeof(AccountDto), description: "The list of accounts for the given customer")]
        [SwaggerResponse(statusCode: 400, type: typeof(BaseError), description: "Error generated by validations or bussines rules.")]
        [SwaggerResponse(statusCode: 404, description: "There was no accounts linked to de customer with that id.")]
        public async Task<IActionResult> GetCustomerAccounts(int? customerId)
        {
            this.ValidateNullParameter(customerId, nameof(customerId));

            var customerAccounts = await this.customerService.GetCustomerAccounts(customerId.Value);
            if (!customerAccounts.Any())
            {
                return NotFound();
            }

            return Ok(customerAccounts.Select(cusAcc => cusAcc.ToDto()));
        }
    }
}
