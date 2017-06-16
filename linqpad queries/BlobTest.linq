<Query Kind="Program">
  <Connection>
    <ID>03878e15-ea7f-4e68-bd12-c3057b18eb94</ID>
    <Persist>true</Persist>
    <Server>CORP-SQLSC01.CLIENT.EXT, 58101</Server>
    <IsProduction>true</IsProduction>
    <Database>ExpressLaneDataStore</Database>
    <ShowServer>true</ShowServer>
  </Connection>
  <Reference>&lt;RuntimeDirectory&gt;\System.Messaging.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.DirectoryServices.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Configuration.Install.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Windows.Forms.dll</Reference>
  <NuGetReference>Microsoft.Azure.Storage.DataMovement</NuGetReference>
  <NuGetReference>WindowsAzure.Storage</NuGetReference>
  <Namespace>Microsoft.WindowsAzure.Storage</Namespace>
  <Namespace>Microsoft.WindowsAzure.Storage.Auth</Namespace>
  <Namespace>Microsoft.WindowsAzure.Storage.Blob</Namespace>
  <Namespace>Microsoft.WindowsAzure.Storage.DataMovement</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.Collections.Concurrent</Namespace>
  <Namespace>System.Messaging</Namespace>
</Query>

static CloudBlobClient cloudBlobClient;
static object amountlock = new object();
static async Task CopyOperation(String sourceBlobName, String targetBlobName, String containerName, String targetContainerName)
{
	CloudBlockBlob sourceBlob = cloudBlobClient.GetContainerReference(containerName).GetBlockBlobReference(sourceBlobName);
	CloudBlockBlob targetBlob = cloudBlobClient.GetContainerReference(targetContainerName).GetBlockBlobReference(targetBlobName);

	if (sourceBlob.Exists())
	{
		//Successful Copy
		//targetBlob.StartCopy(sourceBlob);
		await TransferManager.CopyAsync(sourceBlob, targetBlob, isServiceCopy: false);
	}
	else
	{
		//Unsuccessful Copy
		//Console.WriteLine("Copy unsuccessful");
	}

}

static String dateFormat(String year, String month, String day)
{
	return year + "/" + month + "/" + day;
}

static List<String> parseDate(String date)
{
	List<String> t = new List<String>();
	t.Insert(0, date.Substring(0, 4));
	t.Insert(1, date.Substring(4, 2));
	t.Insert(2, date.Substring(6, 2));
	return t;
}



void Main()
{
	String accountName = "samsteststorageaccount1";
	String accountKey = "nRQmMZ/auB/KTPHz4AiqLfkFyxmXNpApjiS3o33W3zDEqtX/NngVsZ0mNvxOYYrZnitBunCTR7/q3NlVAwJf0A==";
	String containerName = "myfirstcontainer";
	String targetContainerName = "newcontainer";

	String fileExtension = ".txt";

	CloudStorageAccount storageAccount = new CloudStorageAccount(new StorageCredentials(accountName, accountKey), true);
	cloudBlobClient = storageAccount.CreateCloudBlobClient();

	

	var localFilePath = @"C:\";

	//Log = Console.Out;
	var sw = new Stopwatch();
	sw.Start();
	for (int i = 0; i < 100000; i += 10000)
	{
		var list = (from s in Scans
						//where s.ScanItemBlobId != null && s.ScanItemBlobId != s.TransactionId && s.TransactionId > 0
					select new
					{
						//s.ScanId,
						//s.Description,
						s.ScanItemBlobId,
						s.TransactionId,
						//s.ScanDay,
						s.CreatedAt
					}).Where(s => s.TransactionId > i && s.TransactionId < i + 10000).ToList();


		Console.WriteLine(sw.ElapsedMilliseconds);
		Parallel.ForEach(list, item =>
		{
			String testSourceBlobName = item.ScanItemBlobId.ToString();
			String testTargetBlobName = item.TransactionId.ToString();

				//0 - year, 1 - month, 2 - day

			String directoryHierarchyName = String.Join("/", new string[] { item.CreatedAt.Year.ToString(), item.CreatedAt.Month.ToString(), item.CreatedAt.Day.ToString()});

			String fullyQualifiedSourceBlobName = testSourceBlobName + fileExtension;
			String fullyQualifiedTargetBlobName = $"{directoryHierarchyName}/{testTargetBlobName}{fileExtension}";

				//CopyOperation(fullyQualifiedSourceBlobName, fullyQualifiedTargetBlobName, containerName, targetContainerName).GetAwaiter().GetResult();
			try
			{
				MessageQueue messageQueue = new MessageQueue(@".\Private$\blobs");
				messageQueue.Send("", fullyQualifiedTargetBlobName);
			}
			catch (Exception ex)
			{
				ex.Dump();
				throw;
			}
		});
		//recentScans.Dump();

		//ConcurrentQueue<IQueryable> queue = new ConcurrentQueue<IQueryable>();


	}


}