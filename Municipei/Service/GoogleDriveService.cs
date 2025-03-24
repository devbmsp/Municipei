using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Municipei.Service
{
    public class GoogleDriveService
    {
        private readonly DriveService _driveService;

        public GoogleDriveService(string serviceAccountJsonPath)
        {
            var credential = GoogleCredential.FromFile(serviceAccountJsonPath)
                                             .CreateScoped(DriveService.ScopeConstants.Drive);

            _driveService = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "MunicipeiDrive"
            });
        }

    
        public async Task<string> ObterFileIdPorNomeAsync(string fileName)
        {
            var listRequest = _driveService.Files.List();

            listRequest.Q = $"name = '{fileName}' and mimeType = 'application/pdf'";
            var result = await listRequest.ExecuteAsync();

            var file = result.Files.FirstOrDefault();
            return file?.Id; 
        }

        public async Task<Stream> DownloadFileAsync(string fileId)
        {
            var request = _driveService.Files.Get(fileId);
            var stream = new MemoryStream();
            await request.DownloadAsync(stream);
            stream.Position = 0; 
            return stream;
        }
    }
}
