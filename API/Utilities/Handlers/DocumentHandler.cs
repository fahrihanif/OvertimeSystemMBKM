namespace API.Utilities.Handlers;

public class DocumentHandler
{
    public static async Task<string> Upload(IFormFile? document, Guid overtimeId)
    {
        var fileExtension = Path.GetExtension(document.FileName);
        var fileName = $"{overtimeId}{fileExtension}";
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/documents", fileName);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await document.CopyToAsync(stream);

        return filePath;
    }

    public static async Task<byte[]> Download(string document)
    {
        return await File.ReadAllBytesAsync(document);
    }
}
