using System.Data.Common;
using System.Dynamic;
using System.Reflection;

namespace GDrive_test;


// Get Google drive entry for Id https://developers.google.com/drive/api/reference/rest/v3/files/get
// Quand on fait un get sur avec id root => renvoie un autre id
// Returns only few fields (kind, id, name, mimeType, resourceKey), 

// File https://developers.google.com/drive/api/reference/rest/v3/files#File

// https://www.c-sharpcorner.com/UploadFile/955025/C-Sharpinterviewquestionpart7can-an-abstract-class-have-a-constr/
// https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/nullable-value-types

// TODO Create abstract property path https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/how-to-define-abstract-properties
// TODO Create a method to export all files from a GDriveFolderEntry

/// <summary>
/// Abstract class for an entry (file/folder) in Google Drive
/// </summary>
public abstract class GDriveEntry
{
	protected const string RootFolderName = "[Root Folder]";
	protected const string FolderSeparator = "/";

	//protected DateTime Modified;
	public string Id { get; }
	//protected string iD;
	public string Name { get; }
	public string mimeType { get; }

	public DateTimeOffset? modifiedTime { get; }
	public DateTimeOffset? createdTime { get; }
	//TODO Add url ?
	protected GDriveFolderEntry? parent;
	public bool IsFolder()
	{
		return this.mimeType == "application/vnd.google-apps.folder";
	}

	// TODO Add Drive notion ?
	public abstract string Path { get; }
	public string FullPath { get { return this.Path + this.Name + FolderSeparator; } }

	public GDriveEntry(Google.Apis.Drive.v3.Data.File? entry, GDriveFolderEntry? parent)
	{
		this.parent = parent;

		if (entry != null)
		{
			this.Id = entry.Id;
			this.Name = entry.Name;
			this.mimeType = entry.MimeType;
			this.createdTime = entry.CreatedTimeDateTimeOffset;
			this.modifiedTime = entry.ModifiedTimeDateTimeOffset;
			// https://www.appsloveworld.com/google-drive-api/4/get-createddate-from-google-drive-python-api-for-downloads
		}
		else
		{
			// root folder
			this.Id = "root";
			this.Name = RootFolderName;
			this.mimeType = "application/vnd.google-apps.folder";
		}

		// TODO Add the entry to its parent ? => auto alim
		// TODO Retrieve date creation modified
	}

	public abstract void Print();
}


/// <summary>
/// Class for folder entry in Google Drive
/// </summary>
public class GDriveFolderEntry : GDriveEntry
{
	protected string path;	// TODO factorize on base class
	protected List<GDriveEntry> entries;

	/// <summary>
	/// Create a GDriveFolderEntry entry from the Google Drive API File
	/// </summary>
	/// <param name="entry">GDrive API File structure retreived from a file.list or file.get</param>
	/// <param name="parent">Parent folder for a folder entry or containing folder for a file entry, can be null</param>
	public GDriveFolderEntry(Google.Apis.Drive.v3.Data.File? entry, GDriveFolderEntry? parent): base(entry, parent)
	{
		this.entries = [];

		//TODO in which case entry is null ?
		if (entry != null)
		{
			if (parent != null)
				this.path = parent.Path + parent.Name + FolderSeparator;
			else
				this.path = "";
		}
		else
			this.path = "";	// TODO use root name ?
	}

	// https://learn.microsoft.com/fr-fr/dotnet/csharp/iterators
	// iterator over files
	// iterator over folders
	// iterator over files and folders

	/// <summary>
	/// Iterator on folder entries
	/// </summary>
	public IEnumerable<GDriveEntry> Entries
	{
		get
		{
			foreach (var e in entries)
				yield return e;
		}
	}

	// Auto AddEntry or manual
	// TODO Add a safe add ?
	public void AddEntry(GDriveEntry entry)
	{
		this.entries.Add(entry);
	}

    public override void Print()
    {
        //base.print();
		Console.WriteLine($"Folder: {this.Name} - Id: {this.Id} - Path: {this.path}");
    }

    public override string Path {
		get { return path; }		
	}

	public void PrintAll()
	{
		foreach (var f in Entries)
		{
			if (! f.IsFolder())
				f.Print();
			else
				((GDriveFolderEntry) f).PrintAll();
		}
	}
}


/// <summary>
/// Class for file entry in Google Drive
/// </summary>
public class GDriveFileEntry : GDriveEntry
{
	public long FileSize { get; }
	//protected long size;
    protected string md5 = "";
	public GDriveFileEntry(Google.Apis.Drive.v3.Data.File? entry, GDriveFolderEntry? parent): base(entry, parent)
	{
		// TODO test null => Exception ?
		if (entry != null)
		{
			this.FileSize = (long)entry.Size.GetValueOrDefault();
			this.md5 = entry.Md5Checksum;
		}
	}

    public override void Print()
    {
        //base.print();
		//Console.WriteLine($"File: {this.Name} - Size: {size} - mimetype: {this.mimeType} - Path: {this.Path} - Id: {this.Id}");
		Console.WriteLine($"\"{this.Name}\";\"{this.Path}\";\"{this.mimeType}\";{this.FileSize};{this.Id};{this.md5}");
    }

    public override string Path {
		get
		{
			if (parent != null)
				return parent.Path + parent.Name + FolderSeparator;
			else
				// TODO Add exception?
				return "";
		}
	}
}

// IComparer ??
// https://stackoverflow.com/questions/6199029/how-to-make-a-sortedlist-sort-reversely-do-i-have-to-customize-a-icomparer
// https://chrisbitting.com/2015/03/31/using-sortedset-as-a-sorted-list-with-custom-comparer-c/
public class GDriveFileEntryComparer: IComparer<GDriveFileEntry>
{
	public int Compare(GDriveFileEntry? a, GDriveFileEntry? b)
	{
		// TODO Check if test null is needed
		if (a?.FileSize > b?.FileSize)
		{
			return -1;
		}
		else
			return 1;
	}
}
