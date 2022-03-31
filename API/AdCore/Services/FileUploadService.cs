using AdCore.Exceptions;
using AdCore.Interface;
using Microsoft.AspNetCore.Http;

namespace AdCore.Services
{
    public class FileUploadService : IFileUploadService
    {
        public bool DeleteFile(string key)
        {
            var pathBuilt = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\" + key);
            FileInfo file = new FileInfo(pathBuilt);
            if (file.Exists)
            {
                file.Delete();
                return true;
            }

            return false;
        }

        public async Task<string> UploadFile(string keyPrefix, IFormFile file)
        {
            try
            {
                var extension = "." + file.FileName.Split('.')[file.FileName.Split('.').Length - 1];
                var fileName = keyPrefix + extension;

                var pathBuilt = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Upload\\fils");

                if (!Directory.Exists(pathBuilt))
                {
                    Directory.CreateDirectory(pathBuilt);
                }

                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Upload\\fils",
                    fileName);

                await using var stream = new FileStream(path, FileMode.Create);
                await file.CopyToAsync(stream);

                return Path.Combine("Upload\\fils", fileName);
            }
            catch (Exception e)
            {
                throw new CustomException($"Failed to save file, Message: {e.Message}");
            }
        }

        public async Task<List<string>> UploadFiles(
            List<(string keyPrefix, IFormFile file)> uploadFiles)
        {
            List<string> fileName = new List<string>();

            foreach (var uploadFile in uploadFiles)
            {
                fileName.Add(await UploadFile(uploadFile.keyPrefix, uploadFile.file));
            }

            return fileName;
        }
    }
}
