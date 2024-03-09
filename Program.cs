using System.Management;
//using System.Net.NetworkInformation;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;

using GDrive_test;
using System.Reflection;
using System.Runtime.Serialization;
using System.Data.Common;
using System.Xml.Schema;
using Google.Apis.Util.Store;
//using System.ComponentModel.Design.Serialization;


// https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/identifier-names
// https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions

// Trouver des doublons
// Loop on files
//   Add filesize to list
//   If filesize already exists =>

// Step 1
// Gather all files with the same size (1 group per file size)
// Step2
// Use hash to check same files (in one group of same filesize there can be multiple groups of same files)
//


// ------ Trouver les différences entre 2 ensembles de fichiers ------
// "Source" vs "Destination"
// Comparaison par nom de fichier (et taille fichier) ou hash (mais suppose de les calculer pour chaque fichier ???)
// Parcourir la liste fichiers Source
// 	Si le fichier n'existe pas dans la destination (même nom) => ajouter a missing in dest list
//  Si le fichier existe (même nom) mais pas avec la même taille => ajouter a missing in dest list (mettre un flag pour indiquer le même nom ?) ou une autre liste ?
//  Sinon le rien faire
// Faire la même chose en inversant "Source" et "Destination"
// Afficher les 2 (ou 4) listes missing in source/dest
// Besoin d'une collection avec acces rapide (dict ou ??) https://tutorials.eu/c-sharp-collections-performance/
// Fonction qui prend chacun des FolderEntry pour alimenter un Dict<string, FileEntry>
// Fonction qui parcours un ensemble pour comparer avec l'autre (source, dest, missing list)
// Dans le cas d'une comparaison par hash, il n'est cesessaire de calculer le hash que pour les fichiers qui ont les mêmes tailles sur les 2 ensembles
// Autre solution plus rapide
// mettre les sources et dest dans des listes triées (en tenant compte des options:avec ou sans case sensitive, size comparison, ...)
// commencer au début de la liste
// si file(source) > file (dest)
//   parcourir file(dest) tant que > file(source) => missing(dest)
// si file(source) > file (dest)
//   parcourir file(source) tant que > file(dest) => missing(source)
// repeter jusqu'à la fin de 2 listes

// Comparison name to name
// Options
// - Case sensitive ?
// - Force size comparison ?
// Send warning when duplicates (based on options) in "Source" or "Destination"

// Une entrée pour être un fichier ou un dossier
//
// Attributs communs
// Nom, Path
// Dates (création, modif, access)
// "id"
//
// Attributs dossier
// Liste des entrées (dossier/fichier)
// Parent (dossier)
//
// Attributs fichier
// Filesize
// type (extension / mime)
// hash
//
// 2 branches différentes fichiers/dossier avec une entrée générique comme base classe ?
// methode absraite qui indique le type fichiers/dossier ?
// interface pour accéder aux objets ?


namespace Google_Drive_ListFiles
{
	/// <summary>
	/// Class to generate the filter to use for file.list()
	/// </summary>
	public class ScanFolderOptions
	{
		public long MinFileSize { get; set; } = -1;
		public bool FoldersOnly { get; set; } = false;	// Definir enum => attention si only file comment va marcher le récursif ? https://learn.microsoft.com/fr-fr/dotnet/csharp/language-reference/builtin-types/enum
		public bool Recursive { get; set; } = true;
		// https://learn.microsoft.com/en-us/dotnet/core/extensions/file-globbing
		public string FileNamePattern { get; set; } = "";
		// Specific Google Drive
		// https://developers.google.com/drive/api/guides/search-files
		public bool ScanTrashed { get; set; } = false;
		// Possible to use (mimeType contains 'image/' or mimeType contains 'video/') ?
		// TODO manage partial type like image/ (ending with /)
		public List<string>? MimeTypes { get; set; } = null;

		/// <summary>
		/// Generate a mimetype filter based on needed mimetype and some options
		/// </summary>
		/// <returns></returns>
		protected string GetMimeTypeFilter()
		{
			List<string> FilterList = [];
			// https://copyprogramming.com/howto/csharp-c-list-of-strings-to-single-string

			string filter = "";
			if (MimeTypes != null)
			{
				FilterList.Add("mimeType='application/vnd.google-apps.folder'");

				// add the other mimetpes to the folder
				// check if mimetype ends with / => contains instead of =
				foreach (var type in MimeTypes)
				{
					if (type.EndsWith('/'))
					{
						FilterList.Add($"mimeType contains '{type}'");
					}
					else
					{
						FilterList.Add($"mimeType='{type}'");
					}

				}
				filter = String.Join(" and ", FilterList);
			}
			Console.WriteLine($"DEBUG filter={filter}");
			return filter;
		}


		/// <summary>
		/// Generate the complete filter string to apply from all specified options ()
		/// </summary>
		/// <returns></returns>
		public string RequestFilter()
		{
			// Besoin du root id
			List<string> FilterTerms = [];
			
			FilterTerms.Add($"trashed = {ScanTrashed.ToString().ToLower()}");
			string filter = "";

			// compute mimetype filter
			string MimeFilter = GetMimeTypeFilter();
			if (MimeFilter != "")
				FilterTerms.Add(MimeFilter);

			filter = String.Join(" and ", FilterTerms);
			return filter;
		}
	}


    internal class Program
	{
		static readonly string CredentitalsPath = @"E:\Utilisateurs\Chris\Documents\Dev\CSharp\GDrive test\credentials.json";
		//static readonly string CredentitalsPath = @"H:\credentials.json";
		public static string[] Scopes = [DriveService.Scope.DriveReadonly, DriveService.Scope.DriveMetadataReadonly];

		private static UserCredential Login(string googleClientId, string googleClientSecret)
		{
			ClientSecrets secrets = new()
			{
				ClientId = googleClientId,
				ClientSecret = googleClientSecret
			};

			// FIXME need drive.metadata.readonly to get metadata anout the file (size, parents,...) https://stackoverflow.com/questions/12747239/google-drive-the-file-size-is-null
			return GoogleWebAuthorizationBroker.AuthorizeAsync(secrets, new [] { "https://www.googleapis.com/auth/drive.readonly",
			 "https://www.googleapis.com/auth/drive.metadata.readonly" }, "user", CancellationToken.None).Result;
		}

		// https://everyday-be-coding.blogspot.com/p/google-drive-api-uploading-downloading.html?zx=9a1236f192ff217d
		// https://www.daimto.com/google-net-filedatastore-demystified/
		// https://stackoverflow.com/questions/76793220/how-to-read-google-oauth2-usercredentials-from-an-existing-token
		private static UserCredential LoginFromFile(string CredentitalsPath)
		{
			UserCredential credential;
            using var stream = new FileStream(CredentitalsPath, FileMode.Open, FileAccess.Read);
            String FolderPath = @"H:\";
            String FilePath = Path.Combine(FolderPath, "DriveServiceCredentials.json");

            credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.FromStream(stream).Secrets,
                Scopes,
                "user",
                CancellationToken.None,
                new FileDataStore(FilePath, true)).Result;

			return credential;
        }

		// ScanFolder
		// param id du folder ou null/root pour commencer depuis la racine
		// récuperer les infos de base pour le starting id (via file.get) pour le créer
		// car les autres seront récupérés via le file.list

		/*
		* ScanFromId (public) => FolderEntry
		* Get info for the startingid and create a FolderEntry
		* run ScanFromEntry()
		*
		* ScanFromEntry (protected ?)
		* Get file.list
		* Loop on files
		*    Create entry
		*    Add entry to folder (automatique ?)
		*    If entry = Dir => ScanFromEntry()
		*
		* Add callback
		*/

		private static GDriveFolderEntry? ScanFolder(DriveService driveService, string? startingId)
		{
			string Id;

			if (startingId == null || startingId == "")
				Id = "root";
			else
				Id = startingId;
			
			// retrieve a file entry with minimal fields
			FilesResource.GetRequest getRequest = driveService.Files.Get(Id);
			getRequest.Fields = "name, id, mimeType, createdTime";
			try
			{
				var fileInfo = getRequest.Execute();

				// how to get error code ? exception ?
				if (fileInfo != null)
				{
					Console.WriteLine($"DEBUG Name {fileInfo.Name} - Id {fileInfo.Id} - mimeType {fileInfo.MimeType}");
					if (fileInfo.MimeType == "application/vnd.google-apps.folder")
					{
						// it's a folder
						var f = new GDriveFolderEntry(fileInfo, null);
						//ScanFolder(driveService, f);
						ScanFolderRecursive(driveService, f);
						return f;
					}
					else
					{
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


		private static void ScanFolderRecursive(DriveService driveService, GDriveFolderEntry root)
		{
			ScanFolder(driveService, root);

			// Check for folders;
			foreach (var e in root.Entries)
			{
				if (e.IsFolder())
				{
					//Console.WriteLine($"DEBUG Scanning {e.Name}");
					ScanFolderRecursive(driveService, (GDriveFolderEntry)e);
				}
			}
		}

		// besoin de credentials
		// creer le driveservice en dehors ?
		// mettre le scan dans une autre classe ?
		// option pour scanner que les folders / que les fichiers ? les deux ?
		// option pour filtrer le scan sur le nom (regex/wildcard) par ex ou mimetype ?
		// recursive
		
		// Get folder tree
		// start from root
		// use depth
		// get folder id
		// request folders from folder id
		// display folders
		// for each folder get folder tree depth + 1

		private static void ScanFolder(DriveService driveService, GDriveFolderEntry root)
		{
			// https://developers.google.com/drive/api/guides/search-files#python
			string PageToken = "";

			while (true)
			{
				FilesResource.ListRequest listFilesRequest = driveService.Files.List();
				// Concat search parts https://stackoverflow.com/questions/3575029/c-sharp-liststring-to-string-with-delimiter
				// https://developers.google.com/drive/api/guides/ref-search-terms
				// MimeType == "application/vnd.google-apps.folder
				//listFilesRequest.Q = "trashed = false and '0B4HGBSUcIIvlSmMtVUJfM18tR1E' in parents and mimeType='application/vnd.google-apps.folder'";
				//listFilesRequest.Q = "trashed = false and 'root' in parents and mimeType='application/vnd.google-apps.folder'";
				listFilesRequest.Q = $"trashed = false and '{root.Id}' in parents and mimeType='application/vnd.google-apps.folder'";
				//listFilesRequest.Q = $"trashed = false and '{root.Id}' in parents";

				// some fields need to be added to be returned like size, parents
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
					}
					else
					{
						fd = new GDriveFileEntry(file, root);
						fd.Print();
					}
					root.AddEntry(fd);
				}

				//Console.WriteLine($"DEBUG Next token = {request.NextPageToken}");
				PageToken = request.NextPageToken;
				if (PageToken == null)
					break;
			}
		}


		static void Main(string[] args)
		{
			// TEST
			// Console.WriteLine(Path.GetDirectoryName(@"C:\Windows"));
			// Console.WriteLine(Path.GetFullPath(@"C:\Windows"));
			// Console.WriteLine(Path.GetDirectoryName(@"C:\Windows\lsasetup.log"));
			// Console.WriteLine(Path.GetFullPath(@"C:\Windows\lsasetup.log"));
			// C:\
			// C:\Windows
			// C:\Windows
			// C:\Windows\lsasetup.log

			/// https://cloud.google.com/docs/authentication/application-default-credentials
			// TODO retreive credential from json file https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/deserialization
			// https://stackoverflow.com/questions/63558174/how-can-i-authenticate-users-to-their-own-google-drive-account
			// https://stackoverflow.com/questions/16157435/get-user-email-info-from-a-dotnet-google-api/16168206#16168206
			// https://www.daimto.com/google-drive-api-with-a-service-account/
			// https://developers.google.com/api-client-library/dotnet/guide/aaa_oauth?hl=fr
			// https://gist.github.com/cafeasp/84274427e0a202f48e1810e751983ba2
			string googleClientId="<googleClientId>";
			string googleClientSecret= "<googleClientSecret>";

			//UserCredential credential = Login(googleClientId, googleClientSecret);
			UserCredential credential = LoginFromFile(CredentitalsPath);

			var root = new GDriveFolderEntry(null, null);
			//Console.WriteLine($"DEBUG Path for root={root.Path}");
			using (var driveService = new DriveService(new BaseClientService.Initializer() { HttpClientInitializer = credential} ))
			{
				GDriveScan gds = new(driveService);
				//var r = gds.ScanFolder("root");
				var r = gds.ScanFolder("0B4HGBSUcIIvlaF9TWll6elUtRWs");	// Mon Drive/Photos/Thailande/Chiang Rai/

				if (r != null)
				{
					Console.WriteLine($"INFO Folders: {gds.FolderCount} Files: {gds.FileCount}");

					//r.PrintAll();
				}

				// https://gist.github.com/cafeasp/84274427e0a202f48e1810e751983ba2
				// https://developers.google.com/drive/api/guides/ref-search-terms
				//var r = ScanFolder(driveService, "root");

				// Console.WriteLine("INFO Scan over");
				// if (r != null)
				// {
				// 	//r?.Print();
				// 	foreach (var e in r.Entries)
				// 		if (e.isFolder())
				// 		e.Print();
				// }

				// TODE Move out of using
				// https://learn.microsoft.com/fr-fr/dotnet/api/system.collections.generic.sortedlist-2?view=net-7.0
				// Create a lost of sorted files per size
				if (r != null)
				{
					SortedList<GDriveFileEntry, long> FilesPerSize = new SortedList<GDriveFileEntry, long>(gds.FileCount, new GDriveFileEntryComparer());
	
					// Alim sorted list
					foreach (var f in r.Entries)
					{
						FilesPerSize.Add((GDriveFileEntry) f, ((GDriveFileEntry) f).FileSize);
					}

					//
					Console.WriteLine($"DEBUG Starting duplicate search");
					long prevSize=-1;
					bool firstDuplicate=true;	//TODO Rename
					GDriveFileEntry? prevEntry=null;
					foreach(var kvp in FilesPerSize)
					{
						if (kvp.Value == prevSize)
						{
							// Duplicate
							if (firstDuplicate)
							{
								// if firstDuplicate ther prevEntry is the first duplicate, kvp.Key is the second
								firstDuplicate = false;
								Console.WriteLine($"INFO Found duplicate - size {prevSize}");
								// process previous entry
								Console.WriteLine($"INFO {prevEntry?.Name}");
							}

							// process current entry
							Console.WriteLine($"INFO {kvp.Key.Name}");
						}
						else
							firstDuplicate = true;

						prevSize = kvp.Value;
						prevEntry = kvp.Key;
					}
					Console.WriteLine($"DEBUG duplicate search finished");

					// All possible duplicate entries are in the list of list of same size
					// Parse each duplicate entry
					// Each list of same size can contain 0 to n groups of real duplicate (same hash)
					// calculate hash
					// sorted list of hash ?
					// parse sorted hash to find the real duplicate groups
				}

				//
			}

            WindowsDriveScan swf = new()
            {
                ScanFoldersOnly = true
            };
            //WDriveFolderEntry? wroot = swf.ScanFolder("I:\\GamesArchives\\Morrowind");
            WDriveFolderEntry? wroot = swf.ScanFolder("I:\\Temp");
			Console.WriteLine($"Folders: {swf.FolderCount}");

			if (wroot != null)
			{
				Console.WriteLine($"Root: {wroot.Path} - {wroot.Name} - {wroot.FullPath}");

				foreach (var entry in wroot.Entries)
				{
					entry.Print();
				}
			}

			// ScanFolderOptions sco = new() {
			// 	Recursive = true,
			// 	MimeTypes = [ "image/", "video/mpeg" ],
			// };
			
			// Console.WriteLine(sco.RequestFilter());
			// Console.WriteLine("Test for Drive letter");
			// Console.WriteLine($"Fullpath {Path.GetFullPath("I:\\")}");
			// Console.WriteLine($"GetDirectoryName {Path.GetDirectoryName("I:\\")}");
			// Console.WriteLine($"GetFileName  {Path.GetFileName ("I:\\")}");

			List<string> lext = MimeTypesHelper.GetExtensionsFromMimeTypes( [ "video/mpeg", "video/x-ms-*", "image/jpeg", "audio/mp4" ]);
			Console.WriteLine("Press key to continue...");
			Console.ReadLine();

			//MimeTypesHelper.GetMimeTypesTree();
		}
	}
}