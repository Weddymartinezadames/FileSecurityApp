namespace FileSecurityApp.Models
{
    public class FileData
    {
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public byte[] Content { get; set; }
        public string ContentAsString { get; set; }
        public long FileSize { get; set; }
        public string FileExtension { get; set; }

        public FileData()
        {
            FilePath = string.Empty;
            FileName = string.Empty;
            Content = Array.Empty<byte>();
            ContentAsString = string.Empty;
            FileExtension = string.Empty;
        }
    }
}
