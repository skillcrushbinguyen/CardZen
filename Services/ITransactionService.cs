using System.Net.Http.Json;
using CardZen.Models;

namespace CardZen.Services;


public interface ITransactionService
{
    Task<BaseResponse<List<TransactionModel>>> GetTransactionsAsync();
    Task<BaseResponse<TransactionModel>> SaveTransactionAsync(TransactionModel txn);
    Task<BaseResponse<string>> DeleteTransactionAsync(string id);
}


public class TransactionService : ITransactionService
{
    private readonly HttpClient _http;
    private const string GAS_API_URL = "https://script.google.com/macros/s/AKfycbx6DPiu_eUL70Nhka02wsOwXcGdGiXldgYA0TXSwPipFsOwPPXT5xMUXiRwyQvNqhKgig/exec";
    public TransactionService(HttpClient http) { _http = http; }

    public async Task<BaseResponse<List<TransactionModel>>> GetTransactionsAsync()
    {
        var resp = await _http.GetFromJsonAsync<BaseResponse<List<TransactionModel>>>($"{GAS_API_URL}?action=getTransactions")
            ?? new BaseResponse<List<TransactionModel>> { Status = false };
        if (resp.Status && resp.Data != null)
        {
            resp.Data = resp.Data.OrderByDescending(t => t.Date).ToList();
        }
        return resp;
    }

    public async Task<BaseResponse<TransactionModel>> SaveTransactionAsync(TransactionModel txn)
    {
        var response = await _http.PostAsJsonAsync($"{GAS_API_URL}?action=saveTransaction", txn);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<BaseResponse<TransactionModel>>()
                ?? new BaseResponse<TransactionModel> { Status = false };
        }
        return new BaseResponse<TransactionModel> { Status = false };
    }

    public async Task<BaseResponse<string>> DeleteTransactionAsync(string id)
    {
        try
        {
            var response = await _http.PostAsJsonAsync($"{GAS_API_URL}?action=deleteTransaction", new { id });
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<BaseResponse<string>>()
                    ?? new BaseResponse<string> { Status = false };
            }
            return new BaseResponse<string> { Status = false };
            
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting transaction: {ex.Message}");
            return new BaseResponse<string> { Status = false, Error = ex.Message };
        }
    }
}
