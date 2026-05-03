using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using CardZen;
using CardZen.Services;
using System.Diagnostics;
using Microsoft.Extensions.Http;
using System.Text.Json;
using CardZen.Utilities;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddHttpClient();

// Handler để log tất cả request/response đến GAS giúp debug dễ dàng hơn
builder.Services.AddTransient<GasLoggingHandler>();
// Handler để tự động set Content-Type: text/plain cho POST JSON
builder.Services.AddTransient<PlainTextJsonHandler>();
builder.Services.AddHttpClient("GasClient", client =>
{
    // Bạn có thể set BaseAddress ở đây nếu muốn
    // Quan trọng: Thiết lập BrowserHttpWebRequest để xử lý Redirect/CORS của 
})
    .AddHttpMessageHandler<GasLoggingHandler>()
    .AddHttpMessageHandler<PlainTextJsonHandler>();

// Đăng ký ToastService dùng chung toàn app
builder.Services.AddScoped<ToastService>();

// Đăng ký CardService sử dụng Factory để lấy Client đã có Handler
builder.Services.AddScoped<ICardService>(sp => 
{
    var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
    var client = httpClientFactory.CreateClient("GasClient");
    return new CardService(client);
});

builder.Services.AddScoped<ITransactionService>(sp => 
{
    var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
    var client = httpClientFactory.CreateClient("GasClient");
    return new TransactionService(client);
});

await builder.Build().RunAsync();

public class GasLoggingHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Log Request để kiểm tra URL và Action
        Debug.WriteLine($"---> GỬI REQUEST: {request.Method} {request.RequestUri}");
        
        if (request.Content != null)
        {
            var requestBody = await request.Content.ReadAsStringAsync();
            Console.WriteLine("======= GAS REQUEST BODY =======");
            Console.WriteLine(requestBody);
            Console.WriteLine("================================");
        }

        // Thực thi request
        var response = await base.SendAsync(request, cancellationToken);

        // Log Response Status
        Debug.WriteLine($"<--- NHẬN RESPONSE: {response.StatusCode}");

        // Đọc nội dung Response từ GAS (JSON hoặc HTML lỗi)
        if (response.Content != null)
        {
            var content = await response.Content.ReadAsStringAsync();
            
            // In ra Console của Browser (F12)
            Console.WriteLine("======= GAS RESPONSE BODY =======");
            Console.WriteLine(content);
            Console.WriteLine("================================");
        }

        return response;
    }
}

// Handler để tự động set Content-Type: text/plain cho POST tránh CORS
public class PlainTextJsonHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Console.WriteLine($"[PlainTextJsonHandler] Processing {request.Method} {request.RequestUri}");
        if (request.Content != null)
        {
            Console.WriteLine($"[PlainTextJsonHandler] Content type: {request.Content.GetType().FullName}");
            var contentType = request.Content.Headers.ContentType?.ToString() ?? "(null)";
            Console.WriteLine($"[PlainTextJsonHandler] Content-Type header: {contentType}");
            try
            {
                var body = await request.Content.ReadAsStringAsync();
                Console.WriteLine($"[PlainTextJsonHandler] Content body: {body}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PlainTextJsonHandler] Error reading content: {ex.Message}");
            }
        }
        if (request.Method == HttpMethod.Post && request.Content != null)
        {
            // Nếu là StringContent hoặc JsonContent thì ép Content-Type về text/plain
            var contentType = request.Content.Headers.ContentType?.MediaType;
            if (contentType == "application/json")
            {
                request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/plain");
            }
        }
        return await base.SendAsync(request, cancellationToken);
    }
}