namespace AnalyticService.Application.Models.Events;

public class EventException : Exception
{
    public EventException(string message) : base(message) { }
}