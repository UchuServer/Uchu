using System;
using System.Threading;

namespace Uchu.Core
{
    public class LogEntry
    {
        public string message { get; set; }
        public ConsoleColor color { get; set; }
        public LogEntry nextEntry { get; set; }
    }
    
    public class LogQueue : IDisposable
    {
        private readonly Mutex logMutex = new Mutex();
        private LogEntry nextEntry = null;
        private LogEntry lastEntry = null;
        
        /// <summary>
        /// Adds a line to the log queue.
        /// </summary>
        public void AddLine(string line, ConsoleColor color)
        {
            // Lock the log.
            logMutex.WaitOne();
            
            // Create the new entry.
            var entry = new LogEntry()
            {
                message = line,
                color = color,
            };
            
            // Update the pointers.
            if (this.nextEntry == null)
            {
                this.nextEntry = entry;
                this.lastEntry = entry;
            }
            else
            {
                this.lastEntry.nextEntry = entry;
                this.lastEntry = entry;
            }

            // Unlock the log.
            this.logMutex.ReleaseMutex();
        }

        /// <summary>
        /// Pops the next entry to output.
        /// Returns null if there is no pending entry.
        /// </summary>
        public LogEntry PopEntry()
        {
            // Lock the log.
            logMutex.WaitOne();
            
            // Pop the next entry.
            LogEntry entry = null;
            if (this.nextEntry != null)
            {
                entry = this.nextEntry;
                this.nextEntry = entry.nextEntry;
            }
            
            // Unlock the log.
            this.logMutex.ReleaseMutex();
            
            // Return the entry.
            return entry;
        }

        /// <summary>
        /// Starts a thread for outputting the log. This prevents
        /// the console IO from blocking the application.
        /// </summary>
        public void StartConsoleOutput()
        {
            new Thread(() =>
            {
                // Run the output loop.
                while (true)
                {
                    // Read the entries until they are exhausted.
                    var entry = this.PopEntry();
                    while (entry != null)
                    {
                        // Set the color to output.
                        if (entry.color == ConsoleColor.White)
                        {
                            Console.ResetColor();
                        }
                        else
                        {
                            Console.ForegroundColor = entry.color;
                        }
                        
                        // Write the entry.
                        Console.WriteLine(entry.message);
                        entry = this.PopEntry();
                    }
                    
                    // Reset the color.
                    Console.ResetColor();
                    
                    // Sleep for a bit before resuming.
                    Thread.Sleep(50);
                }
            }).Start();
        }
        
        /// <summary>
        /// Disposed the object.
        /// Implemented to silence CA1001 warning.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.logMutex.Dispose();
            }
        }

        /// <summary>
        /// Disposed the object.
        /// Implemented to silence CA1001 warning.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}