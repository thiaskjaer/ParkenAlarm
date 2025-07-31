using Microsoft.AspNetCore.Mvc;
using AngleSharp;
using ParkenEvent;

[ApiController]
[Route("[controller]")]
public class ParkenEventController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ParkenEventController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet("")]
    public async Task<IActionResult> CheckEvent()
    {
        DateTime date = DateTime.UtcNow;

        var client = _httpClientFactory.CreateClient();
        var response = await client.GetAsync("https://www.parkenstadion.dk/kalender");

        if (!response.IsSuccessStatusCode)
        {
            return StatusCode((int)response.StatusCode, "Failed to fetch the event page.");
        }

        var html = await response.Content.ReadAsStringAsync();

        var config = Configuration.Default;
        var context = BrowsingContext.New(config);
        var document = await context.OpenAsync(req => req.Content(html));

        var dateElements = document.QuerySelectorAll(".dato-begivenhed");
        var found = dateElements.Select(el =>
        {
            DateTime.TryParse(el.TextContent.Trim(), out var eventDate);
            return eventDate;
        }).Where(d => d.Date == date.Date);

        return Ok(new EventResponse
        {
            isThereAnEventToday = found.Any(),
            Events = [.. found]
        });
    }
}
