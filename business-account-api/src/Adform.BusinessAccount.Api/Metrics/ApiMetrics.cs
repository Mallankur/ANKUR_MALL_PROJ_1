using Adform.Ciam.Monitoring.Abstractions.CustomStructures;
using Adform.Ciam.Monitoring.CustomStructures;

namespace Adform.BusinessAccount.Api.Metrics
{
    public static class ApiMetrics
    {
        public static readonly ICustomGauge HelloWorldGauge =
            new CustomGauge(
                Prometheus.Metrics.CreateGauge("helloworld_gauge", "Hello world gauge"));
    }
}
