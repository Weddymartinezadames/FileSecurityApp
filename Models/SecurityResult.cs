namespace FileSecurityApp.Models
{
    public class SecurityResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string OriginalContent { get; set; }
        public string ProcessedContent { get; set; }
        public byte[] OriginalData { get; set; }
        public byte[] ProcessedData { get; set; }
        public string FilePath { get; set; }
        public string OperationType { get; set; }

        public SecurityResult()
        {
            Success = false;
            Message = string.Empty;
            OriginalContent = string.Empty;
            ProcessedContent = string.Empty;
            OriginalData = Array.Empty<byte>();
            ProcessedData = Array.Empty<byte>();
            FilePath = string.Empty;
            OperationType = string.Empty;
        }
    }
}
