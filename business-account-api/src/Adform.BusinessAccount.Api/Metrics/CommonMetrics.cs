using Adform.Ciam.Monitoring.Abstractions.CustomStructures;
using Adform.Ciam.Monitoring.CustomStructures;
using Prometheus;

namespace Adform.BusinessAccount.Api.Metrics
{
    public static class CommonMetrics
    {
        private const string ClientDurationMetric = "http_client_latency";
        private const string MongoDurationMetric = "mongo_latency";
        private const string TokenProviderDurationMetric = "tokenprovider_latency";
        private const string CacheDurationMetric = "cache_latency";
        private const string KafkaConsumerDurationMetric = "kafka_consumer_latency";
        private const string KafkaProducerDurationMetric = "kafka_producer_latency";

        public static readonly ICustomHistogram KafkaProducerExecutionDuration =
            new CustomHistogram(
                Prometheus.Metrics.CreateHistogram(KafkaProducerDurationMetric,
                    "Latency for producing messages (in ms)", new HistogramConfiguration
                    {
                        LabelNames = new[] { "topic_name" },
                        Buckets = new double[] { 10, 25, 50, 100, 250, 500, 1000, 2500, 5000 }
                    }));

        public static readonly ICustomHistogram KafkaConsumerExecutionDuration =
            new CustomHistogram(
                Prometheus.Metrics.CreateHistogram(KafkaConsumerDurationMetric,
                    "Latency for consuming messages (in ms)", new HistogramConfiguration
                    {
                        LabelNames = new[] { "topic_name" },
                        Buckets = new double[] { 10, 25, 50, 100, 250, 500, 1000, 2500, 5000 }
                    }));

        public static readonly ICustomHistogram CacheExecutionDuration =
            new CustomHistogram(
                Prometheus.Metrics.CreateHistogram(CacheDurationMetric,
                    "Latency for cache operations (in ms)", new HistogramConfiguration
                    {
                        LabelNames = new[] { "operation_name" },
                        Buckets = new double[] { 10, 25, 50, 100, 250, 500, 1000, 2500, 5000 }
                    }));

        public static readonly ICustomHistogram TokenProviderExecutionDuration =
            new CustomHistogram(
                Prometheus.Metrics.CreateHistogram(TokenProviderDurationMetric,
                    "Latency for TokenProvider requests (in ms)", new HistogramConfiguration
                    {
                        Buckets = new double[] { 10, 25, 50, 100, 250, 500, 1000, 2500, 5000, 10000, 25000 }
                    }));

        public static readonly ICustomHistogram MongoExecutionDuration =
            new CustomHistogram(
                Prometheus.Metrics.CreateHistogram(MongoDurationMetric,
                    "Latency for Mongo operations (in ms)", new HistogramConfiguration
                    {
                        LabelNames = new[] { "operation_name", "collection_name" },
                        Buckets = new double[] { 10, 25, 50, 100, 250, 500, 1000, 2500, 5000, 10000, 25000 }
                    }));

        public static readonly ICustomHistogram ClientExecutionDuration =
            new CustomHistogram(
                Prometheus.Metrics.CreateHistogram(ClientDurationMetric,
                    "Latency for HTTP client requests (in ms)", new HistogramConfiguration
                    {
                        LabelNames = new[] { "status_code", "method", "endpoint" },
                        Buckets = new double[] { 10, 25, 50, 100, 250, 500, 1000, 2500, 5000, 10000, 25000 }
                    }));
    }
}
