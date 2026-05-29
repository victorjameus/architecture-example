using CompanyName.ProjectName.Application.Common.Interfaces;
using CompanyName.ProjectName.Domain.Enums;
using CompanyName.ProjectName.Domain.Exceptions;
using Flurl.Http;
using Microsoft.Extensions.Configuration;
using System.Text.Json.Serialization;

namespace CompanyName.ProjectName.Infrastructure.ExternalServices;

internal sealed class ExchangeRateService(IConfiguration configuration, IInsightService insightService) : IExchangeRateService
{
    private readonly string _baseUrl = configuration["ExchangeRateApi:BaseUrl"]!;
    private readonly string _apiKey = configuration["ExchangeRateApi:ApiKey"]!;

    public async Task<decimal> GetRateAsync(string fromCurrency, string toCurrency)
    {
        try
        {
            var response = await $"{_baseUrl}/{_apiKey}/pair/{fromCurrency}/{toCurrency}"
                .GetJsonAsync<ExchangeRatePairResponse>();

            insightService.TrackTrace($"Tasa obtenida {fromCurrency}→{toCurrency}: {response.ConversionRate}", InsightLevel.Information);

            return response.ConversionRate;
        }
        catch (FlurlHttpException ex)
        {
            insightService.TrackTrace($"Error al obtener tasa {fromCurrency}→{toCurrency}: {ex.Message}", InsightLevel.Error);
            throw new ExternalServiceException($"Error al obtener la tasa de cambio {fromCurrency}→{toCurrency}: {ex.Message}");
        }
    }

    public async Task<Dictionary<string, decimal>> GetAllRatesAsync(string baseCurrency)
    {
        try
        {
            var response = await $"{_baseUrl}/{_apiKey}/latest/{baseCurrency}"
                .GetJsonAsync<ExchangeRateLatestResponse>();

            insightService.TrackTrace($"Tasas obtenidas para base {baseCurrency}: {response.ConversionRates.Count} monedas", InsightLevel.Information);

            return response.ConversionRates;
        }
        catch (FlurlHttpException ex)
        {
            insightService.TrackTrace($"Error al obtener tasas para {baseCurrency}: {ex.Message}", InsightLevel.Error);
            throw new ExternalServiceException($"Error al obtener las tasas de cambio para {baseCurrency}: {ex.Message}");
        }
    }
}

internal sealed class ExchangeRatePairResponse
{
    [JsonPropertyName("conversion_rate")]
    public decimal ConversionRate { get; set; }
}

internal sealed class ExchangeRateLatestResponse
{
    [JsonPropertyName("conversion_rates")]
    public Dictionary<string, decimal> ConversionRates { get; set; } = [];
}