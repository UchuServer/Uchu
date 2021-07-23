namespace Uchu.World.Systems.Behaviors
{
    /// <summary>
    /// The execution context of a set of Start and End behaviors
    /// </summary>
    /// <remarks>
    /// When Start is used, it will create this. If a specific behavior needs to end it
    /// can get an event from the End behavior.
    /// </remarks>
    public class ExecutionEnclosedContext
    {
        public Event End { get; set; }
        public ExecutionEnclosedContext()
        {
        }
    }
}