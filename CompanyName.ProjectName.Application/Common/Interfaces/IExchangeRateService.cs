namespace CompanyName.ProjectName.Application.Common.Interfaces;

public interface IExchangeRateService
{
    Task<decimal> GetRateAsync(string fromCurrency, string toCurrency);
    Task<Dictionary<string, decimal>> GetAllRatesAsync(string baseCurrency);
}