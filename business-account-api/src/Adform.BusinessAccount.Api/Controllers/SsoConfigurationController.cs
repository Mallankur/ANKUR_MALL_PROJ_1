using Adform.BusinessAccount.Api.Swagger.Examples;
using Adform.BusinessAccount.Contracts;
using Adform.Ciam.ExceptionHandling.Abstractions.Contracts;
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
using Adform.BusinessAccount.Api.Capabilities;
using Adform.BusinessAccount.Contracts.Entities;
using Adform.Ciam.Authorization.Filters;
using Adform.Ciam.Swagger.Attributes;
using Adform.Ciam.Swagger.Enums;
using Mapster;
using Adform.AspNetCore.Paging;
using Order = Adform.AspNetCore.Paging.Order;
using Page = Adform.AspNetCore.Paging.Page;
using Adform.BusinessAccount.Domain.Exceptions;
using System.Text.Json.Serialization;

namespace Adform.BusinessAccount.Api.Controllers;

[ApiVersion("1.0")]
[Route("v{version:apiVersion}/[controller]")]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
[ApiController]
[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
public class SsoConfigurationController : ControllerBase
{
	private readonly ILogger<SsoConfigurationController> _logger;
	private readonly ISsoConfigurationService _ssoConfigurationService;


	public SsoConfigurationController(ILogger<SsoConfigurationController> logger, ISsoConfigurationService ssoConfigurationService)
	{
		_logger = logger;
		_ssoConfigurationService = ssoConfigurationService;
	}

	[HttpPost]
	[ProducesResponseType(typeof(SsoConfiguration), StatusCodes.Status201Created)]
	[SwaggerResponseExample(StatusCodes.Status201Created, typeof(SsoConfigurationResponseExample))]
	[RequiredScope(StartupOAuth.Scopes.Full)]
	public async Task<IActionResult> \   CreateAsync(Contracts.Entities.CreateSsoConfigurationInput ssoConfigurationRequest)
	{
		_logger.LogInformation($"POST: {Environment.NewLine}{JsonConvert.SerializeObject(ssoConfigurationRequest)}");

		var result = await _ssoConfigurationService.CreateAsync(ssoConfigurationRequest);
	
		return Created($"v1/ssoconfiguration/{result.Id}", result);
	}


	[HttpGet("domain:{domain}")]
	[ProducesResponseType(typeof(SsoConfiguration), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
	[SwaggerResponseExample(StatusCodes.Status200OK, typeof(SsoConfigurationResponseExample))]
	[RequiredScope(StartupOAuth.Scopes.Readonly)]
	public async Task<IActionResult> GetByDomainAsync([FromRoute] string domain, CancellationToken cancellationToken)
	{
		_logger.LogInformation($"GET By Domain: {domain}");
		var item = await _ssoConfigurationService.GetByDomainNameAsync(domain, cancellationToken);

		if (item == null)
			return NotFound(new ErrorResponse(ErrorReasons.NotFound, Messages.SsoConfigurationNotFound));

		return Ok(item);
	}


	[HttpGet("name:{name}")]
	[ProducesResponseType(typeof(SsoConfiguration), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
	[SwaggerResponseExample(StatusCodes.Status200OK, typeof(SsoConfigurationResponseExample))]
	[RequiredScope(StartupOAuth.Scopes.Readonly)]
	public async Task<IActionResult> GetByNameAsync([FromRoute]string name, CancellationToken cancellationToken)
	{
		_logger.LogInformation($"GET By Name: {name}");
		var item = await _ssoConfigurationService.GetByNameAsync(name, cancellationToken);

		if (item == null)
			return NotFound(new ErrorResponse(ErrorReasons.NotFound, Messages.SsoConfigurationNotFound));

		return Ok(item);
	}

	[HttpGet]
	[RequiredScope(StartupOAuth.Scopes.Readonly)]
	[ProducesResponseType(typeof(IEnumerable<SsoConfiguration>), StatusCodes.Status200OK)]
	[ProducesResponseHeaders(Headers.Pagination, StatusCodes.Status200OK)]
	[SwaggerResponseExample(StatusCodes.Status200OK, typeof(SsoConfigurationListExample))]
	public async Task<IActionResult> GetAsync([FromQuery] SsoConfigurationType type, [FromQuery] Page page = null, [FromQuery] Order order = null, CancellationToken cancellationToken = default)
	{
		_logger.LogInformation($"GET. Offset: {page.Offset}, Limit: {page.Limit}");

		var servicePage = page.Adapt<Contracts.Entities.Page>();
		var serviceOrder = order.Adapt<Contracts.Entities.Order>();
		var result = await _ssoConfigurationService.GetAsync(type, servicePage, serviceOrder, cancellationToken);
		return new OkWithPaginationResult<IEnumerable<SsoConfiguration>>(result.Content, page, order, page.ReturnTotalCount ? result.TotalCount : (long?)null);
	}


    [HttpPut]
    [ProducesResponseType(typeof(SsoConfiguration), StatusCodes.Status200OK)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(SsoConfigurationResponseExample))]
    [RequiredScope(StartupOAuth.Scopes.Full)]
    public async Task<IActionResult> UpdateAsync(Guid Id,Contracts.Entities.UpdateSsoConfigurationInput ssoConfigurationRequest)
    {
        _logger.LogInformation($"PUT: {Environment.NewLine}{JsonConvert.SerializeObject(ssoConfigurationRequest)}");

        var result = await _ssoConfigurationService.UpdateAsync(Id,ssoConfigurationRequest);

        return Ok(result);
    }

	[HttpPut]
	[ProducesResponseType(typeof(SsoConfiguration),StatusCode.Status200OK)]
	[SwaggwerResponceExamole(StatusCodes.Status200OK),typeof(SsoConfigurationResponseExample))]
	[RequiredScope(StaertupOAuth.Scopes.Full)]

	public async Task<IActionResult>UpdateAsync(String Name ,UpdateSsoConfigurationInputbyName updateinput)
	{
		_logger.LogInformation($"PUT:{Enviroment.NewLine}{JsonConverter.SerializObject(updateinput)});

	   Var result = await _ssoConfigurationService.UpdateAsync(Name, updateinput);
		return Ok(result)
	}
}
