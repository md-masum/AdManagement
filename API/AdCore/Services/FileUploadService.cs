using AdCore.Exceptions;
using AdCore.Extensions;
using AdCore.Interface;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AdCore.Services
{
    public class FileUploadService : IFileUploadService
    {
        private readonly ILogger<FileUploadService> _logger;
        private readonly string _storageConnectionString;

        public FileUploadService(IConfiguration configuration, ILogger<FileUploadService> logger)
        {
            _logger = logger;
            _storageConnectionString = configuration.GetConnectionString("AzureStorage");
        }
        public bool DeleteFileDisk(string key)
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

        public async Task<bool> DeleteFileCloud(string key)
        {
            try
            {
                var extension = key.GetFileExtension();

                var container = await BlobContainerClient(extension);
                var blob = container.GetBlobClient(key);
                await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Failed to delete file, Message: {e.Message}");
                throw new CustomException($"Failed to delete file, Message: {e.Message}");
            }
        }

        public async Task<string> UploadFileDisk(IFormFile file, string keyPrefix = null)
        {
            try
            {
                keyPrefix ??= Guid.NewGuid().ToString();
                var extension = file.FileName.GetFileExtension();
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

        public async Task<string> UploadFileCloud(IFormFile file, string keyPrefix = null)
        {
            try
            {
                keyPrefix ??= Guid.NewGuid().ToString();
                var extension = file.FileName.GetFileExtension();
                var fileName = keyPrefix + extension;

                var container = await BlobContainerClient(extension);
                var blob = container.GetBlobClient(fileName);
                await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
                await blob.UploadAsync(file.OpenReadStream(), new BlobHttpHeaders { ContentType = file.ContentType });
                return blob.Uri.ToString();
            }
            catch (Exception e)
            {
                throw new CustomException($"Failed to save file, Message: {e.Message}");
            }
        }

        public async Task<List<string>> UploadFilesDisk(
            List<(IFormFile file, string keyPrefix)> uploadFiles)
        {
            List<string> fileName = new List<string>();

            foreach (var uploadFile in uploadFiles)
            {
                fileName.Add(await UploadFileDisk(uploadFile.file, uploadFile.keyPrefix));
            }

            return fileName;
        }

        public async Task<List<string>> UploadFilesCloud(
            List<(IFormFile file, string keyPrefix)> uploadFiles)
        {
            List<string> fileName = new List<string>();

            foreach (var uploadFile in uploadFiles)
            {
                fileName.Add(await UploadFileCloud(uploadFile.file, uploadFile.keyPrefix));
            }

            return fileName;
        }


        private async Task<BlobContainerClient> BlobContainerClient(string extension)
        {
            var container = new BlobContainerClient(_storageConnectionString, extension.GetContainerName());
            var createResponse = await container.CreateIfNotExistsAsync();

            if (createResponse is not null && createResponse.GetRawResponse().Status == 201)
                await container.SetAccessPolicyAsync(PublicAccessType.Blob);
            return container;
        }
    }
}
