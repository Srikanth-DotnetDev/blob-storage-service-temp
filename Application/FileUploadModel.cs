
public class FileUploadModel
{
    public FileUploadModel(string fileName, string filePath)
    {
        FileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
        FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
    }

    public string FileName { get; set; }
    public string FilePath { get; set; }
}