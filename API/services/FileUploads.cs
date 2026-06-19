namespace API.Services;

public class FileUploads
{
    public static async Task<string> Upload(IFormFile file)
    {
        string uploadFolder;
        if(Path.GetExtension(file.FileName).Equals(".mp3", StringComparison.OrdinalIgnoreCase))
        {
            uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads", "audio");
        }
        else if(Path.GetExtension(file.FileName).Equals(".png", StringComparison.OrdinalIgnoreCase)
            || Path.GetExtension(file.FileName).Equals(".jpg", StringComparison.OrdinalIgnoreCase)
            || Path.GetExtension(file.FileName).Equals(".jpeg", StringComparison.OrdinalIgnoreCase))
        {
            uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads", "picture");
        }
        else
        {
            throw new InvalidOperationException("Unsupported file type.");
        }
        if(!Directory.Exists(uploadFolder))
        {
            Directory.CreateDirectory(uploadFolder);
        }
        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
        var filePath = Path.Combine(uploadFolder, fileName);
        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);
        return fileName;
    }

    public static async Task<string> UploadAudioStreamAsync(Stream stream, string fileName)
    {
        if (!Path.GetExtension(fileName).Equals(".mp3", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Unsupported audio file type.");
        }

        var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads", "audio");
        if(!Directory.Exists(uploadFolder))
        {
            Directory.CreateDirectory(uploadFolder);
        }

        var generatedName = Guid.NewGuid().ToString() + Path.GetExtension(fileName);
        var filePath = Path.Combine(uploadFolder, generatedName);
        await using var fileStream = new FileStream(filePath, FileMode.Create);
        await stream.CopyToAsync(fileStream);
        return generatedName;
    }
}
