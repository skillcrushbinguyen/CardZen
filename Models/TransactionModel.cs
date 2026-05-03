using System;
using System.Text.Json.Serialization;
using CardZen.Utilities;

namespace CardZen.Models
{
    public class TransactionModel
    {
        public string Id { get; set; } = string.Empty;
        [JsonPropertyName("card_id")]
        public string CardId { get; set; } = string.Empty;
        public string Type { get; set; } = "EXPENSE"; // EXPENSE/INCOME/PAYMENT
        [JsonConverter(typeof(FlexibleDateTimeConverter))]
        public DateTime Date { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Mcc { get; set; } = string.Empty;
        public int Amount { get; set; }
        [JsonPropertyName("cashback_amount")]
        public int CashbackAmount { get; set; }
        public string Note { get; set; } = string.Empty;
    }
}
