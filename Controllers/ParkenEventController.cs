using Microsoft.AspNetCore.Mvc;
using ParkenEvent.Services;

[ApiController]
[Route("[controller]")]
public class ParkenEventController : ControllerBase
{
    private readonly IParkenEventService _parkenEventService;

    public ParkenEventController(IParkenEventService parkenEventService)
    {
        _parkenEventService = parkenEventService;
    }
    
    /// <summary>
    /// Gets today's event information from Parken Stadium
    /// </summary>
    /// <returns>Event information for today</returns>
    [HttpGet("")]
    public async Task<IActionResult> CheckEvent()
    {
        var result = await _parkenEventService.GetTodaysEventAsync();
        return Ok(result);
    }
}
