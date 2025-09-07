using Polly;
using Polly.CircuitBreaker;

const string apiKey = "2NMSSYUP77QNCNV5";
const string baseUrl = "https://www.alphavantage.co/query";

/*
 * O CircuitBreakerAsync é configurado para interromper o circuito se 25% das solicitações falharem
 * (configurado para 7 solicitações permitidas antes de quebrar) dentro de uma janela de tempo, neste caso, 30 segundos.
 * */

var circuitBreakerPolicy = Policy
            .Handle<HttpRequestException>()
            .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 7,
                durationOfBreak: TimeSpan.FromSeconds(30),
                onBreak: (result, breakDelay) =>
                {
                    Console.WriteLine($"Circuit broken! Break duration: {breakDelay.TotalSeconds} seconds");
                },
                onReset: () => Console.WriteLine("Circuit reset!"),
                onHalfOpen: () => Console.WriteLine("Circuit half-open, next call is a trial.")
            );

var httpClient = new HttpClient();

for (int i = 0; i < 10; i++)
{
    try
    {
        var response = await circuitBreakerPolicy.ExecuteAsync(() => MakeApiRequest(httpClient));
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine("API Response: " + "OK");
        }
        else
        {
            Console.WriteLine("Request failed with status code: " + response.StatusCode);
        }
    }
    catch (BrokenCircuitException)
    {
        Console.WriteLine("Circuit is open! Skipping request...");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Request failed: {ex.Message}");
    }

    await Task.Delay(5000);
}

static Task<HttpResponseMessage> MakeApiRequest(HttpClient httpClient)
{
    var symbol = "MSFT";
    var function = "TIME_SERIES_INTRADAY";
    var interval = "5min";

    var url = $"{baseUrl}?function={function}&symbol={symbol}&interval={interval}&apikey={apiKey}";

    return httpClient.GetAsync(url);
}