using Adform.BusinessAccount.Api.Capabilities;
using Adform.BusinessAccount.Api.Swagger.Examples;
using Adform.BusinessAccount.Contracts;
using Adform.BusinessAccount.Contracts.Entities;
using Adform.Ciam.ExceptionHandling.Abstractions.Contracts;
using Adform.Ciam.Swagger.Attributes;
using Adform.Ciam.Swagger.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using Adform.AspNetCore.Paging;
using Adform.Ciam.Authorization.Filters;
using Mapster;
using Order = Adform.AspNetCore.Paging.Order;
using Page = Adform.AspNetCore.Paging.Page;
using Adform.BusinessAccount.Domain.Exceptions;

namespace Adform.BusinessAccount.Api.Controllers;

[ApiVersion("1.0")]
[Route("v{version:apiVersion}/[controller]")]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
[ApiController]
public class BusinessAccountController : ControllerBase
{
	private readonly ILogger<BusinessAccountController> _logger;
	private readonly IBusinessAccountService _businessAccountService;

	public BusinessAccountController(ILogger<BusinessAccountController> logger,
		IBusinessAccountService businessAccountService)
	{
		_logger = logger;
		_businessAccountService = businessAccountService;
	}

	[HttpPost]
	[ProducesResponseType(typeof(Contracts.Entities.BusinessAccount), StatusCodes.Status201Created)]
	[SwaggerResponseExample(StatusCodes.Status201Created, typeof(BusinessAccountResponseExample))]
	[RequiredScope(StartupOAuth.Scopes.Full)]
	public async Task<IActionResult> CreateAsync(Contracts.Entities.CreateBusinessAccountInput accountRequest)
	{
		_logger.LogInformation($"POST: {Environment.NewLine}{JsonConvert.SerializeObject(accountRequest)}");
		var result = await _businessAccountService.CreateAsync(accountRequest);

		return Created($"v1/businessaccount/{result.Id}", result);
	}

	[HttpGet("{id:guid}")]
	[ProducesResponseType(typeof(Contracts.Entities.BusinessAccount), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
	[SwaggerResponseExample(StatusCodes.Status200OK, typeof(BusinessAccountResponseExample))]
	[RequiredScope(StartupOAuth.Scopes.Readonly)]
	public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id, CancellationToken cancellationToken)
	{
		_logger.LogInformation($"GET By Id: {id}");
		var item = await _businessAccountService.GetByIdAsync(id, cancellationToken);

		if (item == null)
			return NotFound( new ErrorResponse(ErrorReasons.NotFound, Messages.BusinessAccountNotFound));

		return Ok(item);
	}

	[HttpGet]
	[RequiredScope(StartupOAuth.Scopes.Readonly)]
	[ProducesResponseType(typeof(IEnumerable<Contracts.Entities.BusinessAccount>), StatusCodes.Status200OK)]
	[ProducesResponseHeaders(Headers.Pagination, StatusCodes.Status200OK)]
	[SwaggerResponseExample(StatusCodes.Status200OK, typeof(BusinessAccountsListResponseExample))]
	public async Task<IActionResult> GetAsync([FromQuery]Page page, [FromQuery] Order order, CancellationToken cancellationToken)
	{
		_logger.LogInformation($"GET. Offset: {page.Offset}, Limit: {page.Limit}");

		var servicePage = page.Adapt<Contracts.Entities.Page>();
        var serviceOrder = order.Adapt<Contracts.Entities.Order>();
        var result = await _businessAccountService.GetAsync(servicePage, serviceOrder, cancellationToken);
		return new OkWithPaginationResult<IEnumerable<Contracts.Entities.BusinessAccount>>(result.Content, page, order, page.ReturnTotalCount ? result.TotalCount : (long?)null);
	}

	[HttpDelete("{id}/{accountVersion}")]
	[RequiredScope(StartupOAuth.Scopes.Full)]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
	public async Task<IActionResult> Delete(Guid id, long accountVersion)
	{
		_logger.LogInformation($"DELETE: {id}, {accountVersion}");

		var response = await _businessAccountService.DeleteAsync(
			new DeleteBusinessAccountInput
			{
				Id = id,
				Version = accountVersion
			});

		if (response == null)
		{
			return NotFound(new ErrorResponse(ErrorReasons.NotFound, Messages.BusinessAccountNotFound));
		}
		else
		{
			return NoContent();
		}
	}
	
}