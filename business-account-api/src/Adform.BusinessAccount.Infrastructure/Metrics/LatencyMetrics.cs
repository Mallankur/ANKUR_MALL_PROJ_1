using Prometheus;

namespace Adform.BusinessAccount.Domain.Metrics
{
    public static class LatencyMetrics
    {
        private const string MultipleIdMongoQueryDurationMetric = "mongo_multipleId_query_latency";
        private const string MultipleIdAerospikeOperationDurationMetric = "aerospike_multipleId_operation_latency";
        private const string AerospikeOperationDurationMetric = "aerospike_operation_latency";
        private const string QueryDurationMetric = "query_latency";

        public static readonly Histogram MultipleIdMongoQueryDuration =
            Prometheus.Metrics.CreateHistogram(MultipleIdMongoQueryDurationMetric,
                "Latency for Mongo queries for multiple ids (in ms)", new HistogramConfiguration
                {
                    LabelNames = new[] { "query_name", "id_count" },
                    Buckets = new double[] { 10, 25, 50, 100, 250, 500, 1000, 2500, 5000, 10000 }
                });

        public static readonly Histogram MultipleIdAerospikeOperationDuration =
            Prometheus.Metrics.CreateHistogram(MultipleIdAerospikeOperationDurationMetric,
                "Latency for erospike operations for multiple ids (in ms)", new HistogramConfiguration
                {
                    LabelNames = new[] { "operation_name", "id_count" },
                    Buckets = new double[] { 10, 25, 50, 100, 250, 500, 1000 }
                });

        public static readonly Histogram AerospikeOperationDuration =
            Prometheus.Metrics.CreateHistogram(AerospikeOperationDurationMetric,
                "Latency for Aerospike operations (in ms)", new HistogramConfiguration
                {
                    LabelNames = new[] { "operation_name" },
                    Buckets = new double[] { 10, 25, 50, 100, 250, 500, 1000 }
                });

        public static readonly Histogram QueryDuration =
            Prometheus.Metrics.CreateHistogram(QueryDurationMetric,
                "Latency for query (in ms)", new HistogramConfiguration
                {
                    LabelNames = new[] { "query_name" },
                    Buckets = new double[] { 10, 25, 50, 100, 250, 500, 1000, 5000 }
                });
    }
}