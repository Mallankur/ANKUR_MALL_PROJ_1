using Adform.BusinessAccount.Domain.Metrics;
using MediatR;
using System.Diagnostics;

namespace Adform.BusinessAccount.Application.Decorators
{
	public class MeasuredHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
	where TRequest : IRequest<TResponse>
	{
		private readonly IRequestHandler<TRequest, TResponse> _innerHandler;

		public MeasuredHandler(IRequestHandler<TRequest, TResponse> innerHandler)
		{
			_innerHandler = innerHandler ?? throw new ArgumentNullException(nameof(innerHandler));
		}

		public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
		{
			var sw = Stopwatch.StartNew();
			try
			{
				return await _innerHandler.Handle(request, cancellationToken);
			}
			finally
			{
				sw.Stop();
				LatencyMetrics.QueryDuration.WithLabels(request.GetType().Name).Observe(sw.ElapsedMilliseconds);
			}
		}
	}
}
