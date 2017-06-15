using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure; // Namespace for CloudConfigurationManager
using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Blob; // Namespace for Blob storage types
using Microsoft.WindowsAzure.Storage.Blob.Protocol;
using Microsoft.WindowsAzure.Storage.Auth;

namespace singlestorageaccountcopy
{
    class Program
    {
        static CloudBlobClient cloudBlobClient;

        static void CopyOperation(String sourceBlobName, String targetBlobName, String containerName, String targetContainerName)
        {
            CloudBlobContainer sourceContainer = cloudBlobClient.GetContainerReference(containerName);
            CloudBlobContainer targetContainer = cloudBlobClient.GetContainerReference(targetContainerName);
            CloudBlockBlob sourceBlob = sourceContainer.GetBlockBlobReference(sourceBlobName);
            CloudBlockBlob targetBlob = targetContainer.GetBlockBlobReference(targetBlobName);
            //if(targetBlob.Exists())
            targetBlob.StartCopy(sourceBlob);
        }

        static String dateFormat(String day, String month, String year)
        {
            return year + "/" + month + "/" + day;
        }

        static void Main(string[] args)
        {
            String accountName = "storage_account_name";
            String accountKey = "some_key";

            String containerName = "myfirstcontainer";
            String targetContainerName = "newcontainer";

            String sourceBlobName = "ScanItemBlobId#2123.png";
            String targetBlobName = "TransactionId#3456.png";

            String year = "2009";
            String month = "3";
            String day = "20";

            String directoryHierarchyName = dateFormat(day, month, year) + "/";


            CloudStorageAccount storageAccount = new CloudStorageAccount(new StorageCredentials(accountName, accountKey), true);
            cloudBlobClient = storageAccount.CreateCloudBlobClient();
            CopyOperation(sourceBlobName, directoryHierarchyName + targetBlobName, containerName, targetContainerName);
        }


    }
}
