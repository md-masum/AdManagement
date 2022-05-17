using AdCore.Dto.Ads;
using AdCore.Entity;
using AdCore.Exceptions;
using AdCore.Extensions;
using AdRepository.Interface;
using AdService.Base;
using AdService.Interface;
using AutoMapper;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AdService
{
    public class FileUploadService : BaseService<AdFile, AdFileDto>, IFileUploadService
    {
        private readonly IMapper _mapper;
        private readonly ILogger<FileUploadService> _logger;
        private readonly string _storageConnectionString;

        public FileUploadService(ICosmosDbRepository<AdFile> baseRepository, 
            IConfiguration configuration, 
            IMapper mapper,
            ILogger<FileUploadService> logger) : base(baseRepository, mapper)
        {
            _mapper = mapper;
            _logger = logger;
            _storageConnectionString = configuration.GetConnectionString("AzureStorage");
        }
        public async Task<bool> DeleteFileDisk(string key)
        {
            var fileObj = await BaseRepository.GetAsync(key);
            if(fileObj == null) return false;
            var pathBuilt = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\" + fileObj.Url);
            FileInfo file = new FileInfo(pathBuilt);
            if (file.Exists)
            {
                file.Delete();
                await BaseRepository.DeleteAsync(key);
                return true;
            }

            return false;
        }

        public async Task<bool> DeleteFileCloud(string key)
        {
            try
            {
                var fileObj = await BaseRepository.GetAsync(key);
                if(fileObj is null) return false;
                var container = await BlobContainerClient(fileObj.FileType.GetContainerName());
                var blob = container.GetBlobClient(fileObj.Name);
                await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
                await BaseRepository.DeleteAsync(key);
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Failed to delete file, Message: {e.Message}");
                throw new CustomException($"Failed to delete file, Message: {e.Message}");
            }
        }

        public async Task<AdFileDto> UploadFileDisk(IFormFile file, string keyPrefix = null)
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

                var adFile = new AdFile
                {
                    Name = fileName,
                    Url = Path.Combine("Upload\\fils", fileName),
                    FileType = extension.GetFileType()
                };
                return _mapper.Map<AdFileDto>(await BaseRepository.AddAsync(adFile));
            }
            catch (Exception e)
            {
                throw new CustomException($"Failed to save file, Message: {e.Message}");
            }
        }

        public async Task<AdFileDto> UploadFileCloud(IFormFile file, string keyPrefix = null)
        {
            try
            {
                keyPrefix ??= Guid.NewGuid().ToString();
                var extension = file.FileName.GetFileExtension();
                var fileName = keyPrefix + extension;

                var container = await BlobContainerClient(extension.GetContainerName());
                var blob = container.GetBlobClient(fileName);
                await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
                await blob.UploadAsync(file.OpenReadStream(), new BlobHttpHeaders { ContentType = file.ContentType });

                var adFile = new AdFile
                {
                    Name = fileName,
                    Url = blob.Uri.ToString(),
                    FileType = extension.GetFileType()
                };
                return _mapper.Map<AdFileDto>(await BaseRepository.AddAsync(adFile));
            }
            catch (Exception e)
            {
                throw new CustomException($"Failed to save file, Message: {e.Message}");
            }
        }

        public async Task<List<AdFileDto>> UploadFilesDisk(
            List<IFormFile> uploadFiles)
        {
            List<AdFileDto> adFile = new List<AdFileDto>();

            foreach (var uploadFile in uploadFiles)
            {
                adFile.Add(await UploadFileDisk(uploadFile));
            }

            return adFile;
        }

        public async Task<List<AdFileDto>> UploadFilesCloud(
            List<IFormFile> uploadFiles)
        {
            List<AdFileDto> adFile = new List<AdFileDto>();

            foreach (var uploadFile in uploadFiles)
            {
                adFile.Add(await UploadFileCloud(uploadFile));
            }

            return adFile;
        }


        private async Task<BlobContainerClient> BlobContainerClient(string containerName)
        {
            var container = new BlobContainerClient(_storageConnectionString, containerName);
            var createResponse = await container.CreateIfNotExistsAsync();

            if (createResponse is not null && createResponse.GetRawResponse().Status == 201)
                await container.SetAccessPolicyAsync(PublicAccessType.Blob);
            return container;
        }
    }
}
