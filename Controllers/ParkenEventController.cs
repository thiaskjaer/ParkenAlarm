using Microsoft.AspNetCore.Mvc;
using AngleSharp;
using ParkenEvent;
using System.Globalization;

[ApiController]
[Route("[controller]")]
public class ParkenEventController : ControllerBase
{
    private const string CALENDAR_URL = "https://www.parkenstadion.dk/kalender";
    private const string CLASSNAME_EVENTDATE = ".dato-begivenhed";
    private const string CLASSNAME_EVENTDETAILS = ".heading-begivenhed-left";
    private const string FCKDESCRIPTION = "F.C. København";
    
    private readonly IHttpClientFactory _httpClientFactory;

    public ParkenEventController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet("")]
    public async Task<IActionResult> CheckEvent()
    {
        DateTime todaysDate = DateTime.UtcNow;

        var client = _httpClientFactory.CreateClient();
        var response = await client.GetAsync(CALENDAR_URL);

        if (!response.IsSuccessStatusCode)
        {
            return StatusCode((int)response.StatusCode, "Failed to fetch the event page.");
        }

        var html = await response.Content.ReadAsStringAsync();

        var config = Configuration.Default;
        var context = BrowsingContext.New(config);
        var document = await context.OpenAsync(req => req.Content(html));

        var dateElements = document.QuerySelectorAll(CLASSNAME_EVENTDATE); // all event dates, only the first element is really needed since events are always listed in order
        var eventDetailsElement = document.QuerySelector(CLASSNAME_EVENTDETAILS); // first event description
        var allEvents = dateElements.Select(el =>
        {
            DateTime.TryParse(el.TextContent.Trim(), 
                new CultureInfo("da-DK"), 
                DateTimeStyles.None, 
                out var eventDate);
            return eventDate;
        });
        var found = dateElements.Select(el =>
        {
            DateTime.TryParse(el.TextContent.Trim(), 
                new CultureInfo("da-DK"), 
                DateTimeStyles.None, 
                out var eventDate);
            return eventDate;
        }).Where(d => d.Date == todaysDate.Date);
        bool isFCK = found.Any() && (eventDetailsElement?.TextContent.Trim().Contains(FCKDESCRIPTION) ?? false);
        
        return Ok(new EventResponse
        {
            TodaysDate = todaysDate.Date,
            EventDate = allEvents.FirstOrDefault().Date,
            isThereAnEventToday = found.Any(),
            isFCK = isFCK,
            Events = [.. found]
        });
    }
}
