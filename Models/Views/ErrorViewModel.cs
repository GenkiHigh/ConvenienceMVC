using Microsoft.AspNetCore.Mvc;

namespace ConvenienceMVC.Models.Views
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [IgnoreAntiforgeryToken]
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }
        public int? Id { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        public string? Path { get; set; }
        public string? Error_Message { get; set; }
    }
}
