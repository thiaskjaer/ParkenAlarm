namespace ParkenEvent.Models;

public class EventResponse
{
    public DateTime TodaysDate { get; set; }
    public bool isThereAnEventToday { get; set; }
    public string? EventDetails { get; set; }
    public bool isFCK { get; set; }
}
