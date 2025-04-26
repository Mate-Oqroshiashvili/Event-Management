using System.Text.Json;

namespace Event_Management.Models.Error
{
    public class ErrorDetails
    {
        public int StatusCode { get; set; } // Status code for server errors
        public string Message { get; set; } // Error message
        public string ErrorId { get; set; } = Guid.NewGuid().ToString(); // Unique identifier for the error
        public string RequestId { get; set; } // Unique identifier for the request
        public string Details { get; set; } // Additional details about the error
        public DateTime TimeSpan { get; set; } = DateTime.Now; // Timestamp of when the error occurred
        public string StackTrace { get; set; } // Stack trace of the error

        public override string ToString()
        { 
            return JsonSerializer.Serialize(this, new JsonSerializerOptions
            {
                WriteIndented = true,
            });
        }
    }
}
