using BuildingBlocks.Pagination;
using Gateway.API.Common.Interface;
using Gateway.API.Common.Model;
using Gateway.API.Common.Response;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace Gateway.API.Service
{
    public sealed class ServiceHealthChecker : IServiceHealthChecker
    {
        private readonly HttpClient _httpClient;
        private readonly Dictionary<string, string> _services;


        public ServiceHealthChecker(
            HttpClient httpClient,
            IOptions<ServiceEndpointsOptions> options)
        {
            _httpClient = httpClient;

            _services = options.Value.Endpoints;
        }



        public async Task<SystemHealthResponse> CheckAsync(
            PaginationRequest request)
        {
            var services = await CheckServicesAsync();



            var totalCount = services.Count;



            var pagedServices = services
                .Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize);



            return new SystemHealthResponse
            {
                Status = services.All(x => x.Status == "Healthy")
                    ? "Healthy"
                    : "Unhealthy",


                Services = new PaginatedResult<ServiceHealthResponse>(
                    request.PageIndex,
                    request.PageSize,
                    totalCount,
                    pagedServices
                )
            };
        }



        private async Task<List<ServiceHealthResponse>> CheckServicesAsync()
        {
            var result = new List<ServiceHealthResponse>();

            foreach (var service in _services)
            {
                var stopwatch = Stopwatch.StartNew();

                try
                {
                    using var response = await _httpClient.GetAsync(
                        service.Value,
                        HttpCompletionOption.ResponseHeadersRead
                    );

                    stopwatch.Stop();

                    result.Add(new ServiceHealthResponse
                    {
                        Name = service.Key,

                        Status = response.IsSuccessStatusCode
                            ? "Healthy"
                            : "Unhealthy",

                        Duration = stopwatch.Elapsed,

                        Error = response.IsSuccessStatusCode
                            ? null
                            : $"HTTP {(int)response.StatusCode}"
                    });
                }
                catch (TaskCanceledException)
                {
                    stopwatch.Stop();

                    result.Add(new ServiceHealthResponse
                    {
                        Name = service.Key,

                        Status = "Unhealthy",

                        Duration = stopwatch.Elapsed,

                        Error = "Timeout"
                    });
                }
                catch (HttpRequestException ex)
                {
                    stopwatch.Stop();

                    result.Add(new ServiceHealthResponse
                    {
                        Name = service.Key,

                        Status = "Unhealthy",

                        Duration = stopwatch.Elapsed,

                        Error = ex.Message
                    });
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();

                    result.Add(new ServiceHealthResponse
                    {
                        Name = service.Key,

                        Status = "Unhealthy",

                        Duration = stopwatch.Elapsed,

                        Error = ex.Message
                    });
                }
            }

            return result;
        }
    }
}