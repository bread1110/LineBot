using System.Text;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class Record
    {
        // 取得 Blob 儲存體連線字串
        private string storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=webhookrecord;AccountKey=auHvVroiW8jLRm5ooX/dKSsW49WehfkxvJ+xB9QGqPwMr0D6o7M96VmIXTtkdhrYzKVq7iI6J3oH+ASt+N56Gw==;EndpointSuffix=core.windows.net";

        // 取得 Blob 儲存體名稱
        private string containerName = "webhook";

        public async Task StoreJsonAsync(string strContent, string strFileName)
        {


            // 建立 Blob 儲存體客戶端
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // 建立 Blob 儲存體容器
            CloudBlobContainer container = blobClient.GetContainerReference(containerName);
            await container.CreateIfNotExistsAsync();

            // 指定 Blob 儲存體名稱
            string blobName = strFileName;

            // 建立 Blob 儲存體物件
            CloudBlockBlob blob = container.GetBlockBlobReference(blobName);

            // 儲存 JSON 字串到 Blob 儲存體
            using (Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(strContent)))
            {
                await blob.UploadFromStreamAsync(stream);
            }
        }
    }
}