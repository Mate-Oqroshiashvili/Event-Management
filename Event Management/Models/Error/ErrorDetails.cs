using System.Text.Json;

namespace Event_Management.Models.Error
{
    public class ErrorDetails
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string ErrorId { get; set; } = Guid.NewGuid().ToString();
        public string RequestId { get; set; }
        public string Details { get; set; }
        public DateTime TimeSpan { get; set; } = DateTime.Now;
        public string StackTrace { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this, new JsonSerializerOptions
            {
                WriteIndented = true,
            });
        }
    }
}
