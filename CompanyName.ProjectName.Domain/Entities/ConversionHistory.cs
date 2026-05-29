namespace CompanyName.ProjectName.Domain.Entities;

public class ConversionHistory
{
    public int Id { get; set; }
    public int FromCurrencyId { get; set; }
    public int ToCurrencyId { get; set; }
    public decimal Amount { get; set; }
    public decimal ConvertedAmount { get; set; }
    public decimal ExchangeRate { get; set; }
    public DateTime ConvertedAt { get; set; }
    public Currency? FromCurrency { get; set; }
    public Currency? ToCurrency { get; set; }
}