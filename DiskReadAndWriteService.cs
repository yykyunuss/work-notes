using Fctr.Edison.NetCore.Core.File;
using Fctr.Edison.FileAdapter.Interfaces;
using Fctr.Edison.FileAdapter.Repositories.Models;
using Fctr.Edison.FileAdapter.Models;
using Fctr.Edison.FileAdapter.Settings;
using System.Runtime.InteropServices;
using Fctr.Edison.FileAdapter.DataAccess;

namespace Fctr.Edison.FileAdapter.Services
{
    public class DiskReadAndWriteService : IDiskReadAndWriteService
    {
        private readonly StorageAreaSetting _storageAreaSetting;
        private readonly ILogger _logger;
        private readonly IEntityRepository<DocumentPoolAdapter> _entityRepository;

        private readonly FileManager fileManager;
        public DiskReadAndWriteService(IEntityRepository<DocumentPoolAdapter> entityRepository,
         StorageAreaSetting storageAreaSetting, ILogger<DiskReadAndWriteService> logger)
        {
            _entityRepository = entityRepository;
            _storageAreaSetting = storageAreaSetting;
            _logger = logger; 

            fileManager = new FileManager();
        }

        public ResponseDocumentPoolAdapter FindDocument(long documentId)
        {
            DocumentPoolAdapter result = _entityRepository.GetAllQueryable().Where(x => x.DocumentId == documentId).ToList().First();
            
            var dto = new ResponseDocumentPoolAdapter()
            {
                Id = result.Id,
                DocumentId = (long)result.DocumentId,
                DocumentName = result.DocumentName,
                StoragePath = result.StoragePath,
                FilePath = result.FilePath
            };

            return dto;
        }

        public async Task SaveDocument(string content, long documentId, string documentName)
        {
            var guidFileName = $"{Guid.NewGuid():N}";

            var filePath = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? $"\\{_storageAreaSetting.StorageFolder}\\\\{DateTime.Today:yyyyMMdd}\\{guidFileName}"
                : $"/{_storageAreaSetting.StorageFolder}//{DateTime.Today:yyyyMMdd}/{guidFileName}";

            var storagePath = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? _storageAreaSetting.StoragePath
                : $"smb://{_storageAreaSetting.StoragePath.TrimStart('\\', '/')}".Replace('\\', '/');

            var hostPath = _storageAreaSetting.StoragePath.Substring(0, _storageAreaSetting.StoragePath.LastIndexOf("\\")).Replace("\\", "");

            var allPath = $"{storagePath}{filePath}";

            var folderPath = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? allPath.Substring(0, allPath.LastIndexOf("\\"))
                : allPath.Substring(0, allPath.LastIndexOf("/"));

            var base64content = Convert.FromBase64String(content);
            
            await fileManager.WriteFileContentAsync(hostPath, allPath, folderPath, base64content, "", _storageAreaSetting.UserName, _storageAreaSetting.Pass, _logger);

            DocumentPoolAdapter documentPoolAdapter = new DocumentPoolAdapter
            {
                DocumentId = documentId,
                DocumentName = documentName,
                StoragePath = storagePath,
                FilePath = filePath
            };

            _entityRepository.Insert(documentPoolAdapter);
        }

        public async Task<byte[]> ReadDocumentFromDiskAsync(ResponseDocumentPoolAdapter responseDocumentPoolAdapter)
        {
            var storagePath = responseDocumentPoolAdapter.StoragePath?.Replace("smb://", "");
            storagePath = storagePath.Substring(0, storagePath.LastIndexOf("/"));

            var path = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? $"{responseDocumentPoolAdapter.StoragePath}{responseDocumentPoolAdapter.FilePath}"
                : $"{responseDocumentPoolAdapter.StoragePath?.Replace('\\', '/')}{responseDocumentPoolAdapter.FilePath}".Replace('\\', '/');

            return await fileManager.ReadFileContentAsync(storagePath, path, string.Empty, _storageAreaSetting.UserName, _storageAreaSetting.Pass, _logger).ConfigureAwait(false);
        }

    }
}
