using Google.Apis.Drive.v3;

namespace GDrive_test;

/*
* Possibilite de d'effectuer plusieurs scans avec la même instance ? 
* Ajouter les scanOptions en parametre du scan ?
* Mettre le call back en parametre scan ou constructeur ?
*
*
*
*
*/

// Comment gérer l'heritage entre les notions file/folder et différents type de path (Windows, GDrive,...)
// https://www.geeksforgeeks.org/c-sharp-multiple-inheritance-using-interfaces/
// https://www.infoworld.com/article/2928719/when-to-use-an-abstract-class-vs-interface-in-csharp.html
// TODO Create a GenericDriveScan scan class to factorize common data
// Interface => pas de membres

public class GDriveScan(DriveService driveService)
{
    protected int fileCount;
    protected int folderCount;
	protected readonly DriveService driveService = driveService;

    public int FileCount { get { return fileCount; } }
    public int FolderCount { get { return folderCount; } }

	/// <summary>
	/// Scan entries in Google Drive from the startingId
	/// </summary>
	/// <param name="startingId">Id of a valid folder in Google Drive or null/empty string to start scan from root</param>
	/// <returns></returns>
    public GDriveFolderEntry? ScanFolder(string? startingId)
    {
        fileCount = 0;
        folderCount = 0;

        string Id;

        if (startingId == null || startingId == "")
            Id = "root";
        else
            Id = startingId;

        // retrieve a folder entry with minimal fields to instanciate a folder entry
        FilesResource.GetRequest getRequest = driveService.Files.Get(Id);
        getRequest.Fields = "name, id, mimeType, createdTime";

		// TODO Rewrite try catch to protect Get and Execute 
		try
		{
			var fileInfo = getRequest.Execute();
			if (fileInfo != null)
			{
				//Console.WriteLine($"DEBUG Name {fileInfo.Name} - Id {fileInfo.Id} - mimeType {fileInfo.MimeType}");
				if (fileInfo.MimeType == "application/vnd.google-apps.folder")
				{
					// it's a folder
					var f = new GDriveFolderEntry(fileInfo, null);

					//ScanFolder(driveService, f);
					ScanFolderRecursive(f);
					return f;
				}
				else
				{
					//TODO Exception ?
					Console.WriteLine($"ERROR ${startingId} is not the id of a folder {fileInfo.MimeType}");
					return null;
				}
			}
			else
			{
				Console.WriteLine($"ERROR Cannot get folder information from ${startingId}");
				return null;
			}
		}
		catch (System.Exception ex)
		{
			Console.WriteLine($"ERROR Getting File info for Id {startingId}: {ex.Message}\n{ex.StackTrace}");
			return null;
		}
    }


	/// <summary>
	/// Scans the given folder and recursivly scans all subfolders
	/// </summary>
	/// <param name="root"></param>
	private void ScanFolderRecursive(GDriveFolderEntry root)
	{
		ScanFolder(root);

		// Check for folders
		foreach (var e in root.Entries)
		{
			if (e.IsFolder())
			{
				ScanFolderRecursive((GDriveFolderEntry)e);
			}
		}
	}


	/// <summary>
	/// Scans entries in Google Drive from root parameter (eg. folder). Entries in this folder are added to the root object
	/// </summary>
	/// <param name="root">Folder to be scanned</param>
	private void ScanFolder(GDriveFolderEntry root)
	{
		// https://developers.google.com/drive/api/guides/search-files#python
		string PageToken = "";

		while (true)
		{
			FilesResource.ListRequest listFilesRequest = driveService.Files.List();
			// Concat search parts https://stackoverflow.com/questions/3575029/c-sharp-liststring-to-string-with-delimiter
			// https://developers.google.com/drive/api/guides/ref-search-terms
			//listFilesRequest.Q = "trashed = false and '0B4HGBSUcIIvlSmMtVUJfM18tR1E' in parents and mimeType='application/vnd.google-apps.folder'";
			//listFilesRequest.Q = "trashed = false and 'root' in parents and mimeType='application/vnd.google-apps.folder'";
			//listFilesRequest.Q = $"trashed=false and '{root.Id}' in parents and mimeType='application/vnd.google-apps.folder'";
			listFilesRequest.Q = $"trashed = false and '{root.Id}' in parents";

			// some fields need to be added to be returned like size, parents
			// TODO add dates
			listFilesRequest.Fields = "nextPageToken, files(name, id, size, parents, mimeType, md5Checksum)";
			listFilesRequest.PageToken = PageToken;

			// TODO Add try catch
			var request = listFilesRequest.Execute();

			foreach (var file in request.Files)
			{
				GDriveEntry fd;
				if (file.MimeType == "application/vnd.google-apps.folder")
				{
					fd = new GDriveFolderEntry(file, root);
					folderCount++;
				}
				else
				{
					// check file vs scan options
					fd = new GDriveFileEntry(file, root);
					fileCount++;
				}
				root.AddEntry(fd);
			}

			//Console.WriteLine($"DEBUG Next token = {request.NextPageToken}");
			PageToken = request.NextPageToken;
			if (PageToken == null)
				break;
		}
	}
}
