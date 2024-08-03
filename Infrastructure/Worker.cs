using System;
using FluentFTP;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Azure.Storage.Blobs;
using System.Security.Cryptography;
using Microsoft.Extensions.Hosting;
using System.ComponentModel;

namespace Personal.BlobStorage.Infrastructure
{
    public class Worker : BackgroundService
    {
        private readonly string _ftpHost;
        private readonly string _ftpUsername;
        private readonly string _ftpPassword;
        private readonly string _blobStorageConnectionString;
        private readonly string _blobContainerName;
        private readonly Dictionary<string, bool> ProcessesFileHashes = new Dictionary<string, bool>();
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested)
            {
                var ftpClient = new FtpClient(_ftpHost, _ftpUsername, _ftpPassword);

                await DownloadAndUploadFilesAsync(stoppingToken);
                // Wait for 24 hours
                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }
        private async Task DownloadAndUploadFilesAsync(CancellationToken cancellationToken)
        {
            try
            {
                using (var ftpClient = new FtpClient(_ftpHost, _ftpUsername, _ftpPassword))
                {
                    //ftpClient.DataConnectionType = FtpDataConnectionType.AutoPassive;
                    //ftpClient.ConnectTimeout = 10000;
                    //ftpClient.DataConnectionConnectTimeout = 10000;
                    //ftpClient.OnLogEvent += (sender, e) => Console.WriteLine(e);
                    ftpClient.Connect();

                    string _ftpFilePath = "/pub/";
                    // Traverse directories and download files
                    await TraverseAndProcessDirectoriesAsync(ftpClient, _ftpFilePath, cancellationToken);
                }
            }
            catch (Exception ex)
            {

            }
        }
        private async Task TraverseAndProcessDirectoriesAsync(FtpClient ftpClient, string remotePath, CancellationToken cancellationToken)
        {
            //if (!Directory.Exists(remotePath))
            //{
            //    Directory.CreateDirectory(remotePath);
            //}
            var items = ftpClient.GetListing(remotePath);

            foreach (var item in items)
            {
                if (item.Type == FtpFileSystemObjectType.Directory)
                {
                    // Recursively traverse subdirectories
                    await TraverseAndProcessDirectoriesAsync(ftpClient, item.FullName, cancellationToken);
                }
                else if (item.Type == FtpFileSystemObjectType.File)
                {
                    // Download and processfile into blob storage
                    await ProcessFileAsync(ftpClient, item.FullName, cancellationToken);
                }
            }
        }
        private async Task ProcessFileAsync(FtpClient ftpClient, string filePath, CancellationToken cancellationToken)
        {
            //Processing file

            try
            {
                using (var memoryStream = ftpClient.OpenRead(filePath))
                {
                    var blobClient = new BlobClient(_blobStorageConnectionString, _blobContainerName, Path.GetFileName(filePath));
                    await blobClient.UploadAsync(memoryStream, overwrite: true, cancellationToken);

                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}



