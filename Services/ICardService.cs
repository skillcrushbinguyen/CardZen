using System;
using System.Net.Http.Json;
using System.Text.Json;
using CardZen.Models;
using CardZen.Utilities;

namespace CardZen.Services;

public interface ICardService
{
    Task<BaseResponse<MasterData>> GetInitialDataAsync();
    Task<BaseResponse<List<CardModel>>> GetCardsAsync();
    Task<BaseResponse<CardModel>> GetCardByIdAsync(string id);
    Task<BaseResponse<CardModel>> SaveCardAsync(CardModel card);
}

public class CardService : ICardService
{
    private readonly HttpClient _http;
    // Thay bằng URL Web App đã Deploy của bạn
    private const string GAS_API_URL = "https://script.google.com/macros/s/AKfycbx6DPiu_eUL70Nhka02wsOwXcGdGiXldgYA0TXSwPipFsOwPPXT5xMUXiRwyQvNqhKgig/exec"; 
    //private const string GAS_API_URL = "https://script.google.com/macros/s/XXX/exec"; 
    public CardService(HttpClient http)
    {
        _http = http;
    }

    // Thay thế app_getInitialData
    public async Task<BaseResponse<MasterData>> GetInitialDataAsync()
    {
        return await _http.GetFromJsonAsync<BaseResponse<MasterData>>($"{GAS_API_URL}?action=getInitialData") 
                ?? new BaseResponse<MasterData> { Status = false, Error = "Connection Error" };
    }

    // Thay thế logic ui_renderCards[cite: 1]
    public async Task<BaseResponse<List<CardModel>>> GetCardsAsync()
    {
        
        var response = await _http.GetFromJsonAsync<BaseResponse<List<CardModel>>>($"{GAS_API_URL}?action=getCards");
    
        if (response?.Status == true && response.Data != null)
        {
            foreach (var card in response.Data)
            {
                // Đảm bảo các trường ngày luôn có giá trị để InfoBlock hiển thị[cite: 3]
                CardLogic.CalculateNextDates(card);
            }
        }
        return response ?? new BaseResponse<List<CardModel>> { Status = false };
    }

    // Thay thế google.script.run.card_getById(cardId)[cite: 1]
    public async Task<BaseResponse<CardModel>> GetCardByIdAsync(string id)
    {
        return await _http.GetFromJsonAsync<BaseResponse<CardModel>>($"{GAS_API_URL}?action=getCardById&id={id}")
                ?? new BaseResponse<CardModel> { Status = false };
    }

    // Thay thế card_saveCard và ui_handleCardSubmit[cite: 1]
    public async Task<BaseResponse<CardModel>> SaveCardAsync(CardModel card)
    {
        var options = new JsonSerializerOptions {
            PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.SnakeCaseLower,
            PropertyNameCaseInsensitive = true
        };
        var response = await _http.PostAsJsonAsync($"{GAS_API_URL}?action=saveCard", card, options);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<BaseResponse<CardModel>>()
                ?? new BaseResponse<CardModel> { Status = false };
        }
        return new BaseResponse<CardModel> { Status = false, Error = "Server Error" };
    }
}

