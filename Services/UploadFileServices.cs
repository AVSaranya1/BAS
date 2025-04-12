using DataAccessLayer.Model;
using WebApi.Controllers;

namespace WebApi.Services
{
    public class UploadFileServices
    {
        public async Task<string?> InsertandUpdateFileName(string FileName, IFormFile ProfileImage, string? _physicalPath)
        {
            string? ImageUpdated = string.Empty;
            string? ErrMessage = "Invalid file type. Allowed Files types" + Common.FileExtensions.FileNameExtension;

            if (!string.IsNullOrEmpty(FileName))
            {
                var allowedExtensions = Common.FileExtensions.FileNameExtension;

                // Get file extension
                var extension = Path.GetExtension(ProfileImage.FileName).Trim().ToLowerInvariant();
                if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension))
                {
                    return ErrMessage;
                }
                else
                {
                    if (!Directory.Exists(_physicalPath))
                        Directory.CreateDirectory(_physicalPath);
                    string UniqueName = Path.ChangeExtension(Path.GetRandomFileName(), extension);
                    var filePath = Path.Combine(_physicalPath, UniqueName);
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ProfileImage.CopyToAsync(stream);
                    }
                    ImageUpdated = ProfileImage?.FileName.Replace(FileName, UniqueName);
                }
            }
            return ImageUpdated;
        }
        public string? GetFile(string ProfileImg, string? _virtualPath)
        {
            string? filePath = string.Empty;

            if (!string.IsNullOrEmpty(ProfileImg))
            {
                filePath = Path.Combine(_virtualPath, ProfileImg);
            }
            else
            {
                //Refers Previous Path
                filePath = Path.Combine(Path.GetDirectoryName(_virtualPath), Common.FileName.noPhoto);

            }
            return filePath;
        }

    }
}


