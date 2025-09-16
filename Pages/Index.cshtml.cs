using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

public class IndexModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly HttpClient _httpClient;

    public IndexModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _httpClient = httpClientFactory.CreateClient("TinyUrlApi");
    }

    public string ApiResponse { get; set; }

    [BindProperty]
    public UrlDto NewUrl { get; set; } = new();
    [BindProperty]
    public string SearchTerm { get; set; }
    [BindProperty]
    public string OriginalUrl { get; set; } = string.Empty;
    [BindProperty]
    public bool IsPrivate { get; set; }
    public string GeneratedShortUrl { get; set; } = string.Empty;
    public List<UrlDto> Urls { get; set; } = new();

    public List<UrlDto> FilteredUrls { get; set; } = new();



    public async Task OnGetAsync()
    {
        try
        {
            var urls = await _httpClient.GetFromJsonAsync<List<UrlDto>>("urls") ?? new();
            
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                FilteredUrls = urls
                    .Where(u => u.OriginalUrl.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase)
                             || u.ShortCode.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
            else
            {
                FilteredUrls = urls;
            }
        }
        catch (HttpRequestException ex)
        {
        }
    }

    public async Task<IActionResult> OnPostGenerateUrlAsync()
    {
        var requestData = new
        {
            OriginalUrl,
            IsPrivate
        };

        var response = await _httpClient.PostAsJsonAsync("urls", requestData);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<UrlDto>();
            if (result != null)
            {
                GeneratedShortUrl = result.ShortUrl;
            }
        }
        else
        {
            ModelState.AddModelError(string.Empty, "Error generating short URL");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await _httpClient.PostAsJsonAsync("api/urls", NewUrl);
        return RedirectToPage();
    }

    public async Task<IActionResult>     OnPostDeleteAsync(int id)
    {
        await _httpClient.DeleteAsync($"api/urls/{id}");
        return RedirectToPage();
    }


    public class UrlDto
    {
        public string Id { get; set; } = string.Empty;
        public string OriginalUrl { get; set; } = string.Empty;
        public string ShortUrl { get; set; } = string.Empty;
        public string ShortCode { get; set; } = string.Empty;
        public long Clicks { get; set; }
        public bool IsPrivate { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
