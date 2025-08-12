namespace ParkenEvent;

public class EventResponse
{
    public DateTime TodaysDate { get; set; }
    public DateTime EventDate { get; set; }
    public bool isThereAnEventToday { get; set; }
    public bool isFCK { get; set; }
    public DateTime[]? Events { get; set; }

}
