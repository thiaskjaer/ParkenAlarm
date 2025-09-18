using ParkenEvent.Models;
using AngleSharp;
using System.Globalization;

namespace ParkenEvent.Services
{
    public class ParkenEventService : IParkenEventService
    {
        private const string CALENDAR_URL = "https://www.parkenstadion.dk/kalender";
        private const string CLASSNAME_EVENTDATE = ".dato-begivenhed";
        private const string CLASSNAME_EVENTDETAILS = ".heading-begivenhed";
        private const string FCKDESCRIPTION = "F.C. København";
        
        private readonly IHttpClientFactory _httpClientFactory;

        public ParkenEventService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<EventResponse> GetTodaysEventAsync()
        {
            DateTime todaysDate = DateTime.UtcNow;

            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync(CALENDAR_URL);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Failed to fetch event page: {response.StatusCode}");
            }

            var html = await response.Content.ReadAsStringAsync();

            var config = Configuration.Default;
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(req => req.Content(html));

            var dateElements = document.QuerySelectorAll(CLASSNAME_EVENTDATE);
            var eventDetailsElement = document.QuerySelector(CLASSNAME_EVENTDETAILS);

            var found = dateElements.Select(el =>
            {
                DateTime.TryParse(el.TextContent.Trim(),
                    new CultureInfo("da-DK"),
                    DateTimeStyles.None,
                    out var eventDate);
                return eventDate;
            }).Where(d => d.Date == todaysDate.Date);

            bool isFCK = found.Any() && (eventDetailsElement?.TextContent.Trim().Contains(FCKDESCRIPTION) ?? false);

            return new EventResponse
            {
                TodaysDate = todaysDate.Date,
                isThereAnEventToday = found.Any(),
                EventDetails = found.Any() ? eventDetailsElement?.TextContent : null,
                isFCK = isFCK
            };
        }
    }
}