using System.Data;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace SaberDev.WebAPI.HealthCheck.HealthCheck
{
    public class DatabaseHealthCheck : IHealthCheck
    {
        public readonly IDbConnection _dbConnection;

        public DatabaseHealthCheck(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                if (_dbConnection.State != ConnectionState.Open)
                    _dbConnection.Open();

                return Task.FromResult(HealthCheckResult.Healthy());
            }
            catch (Exception ex)
            {
                return Task.FromResult(HealthCheckResult.Unhealthy(ex.Message));
            }
        }
    }
}