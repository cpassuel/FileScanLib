namespace GDrive_test;

/// <summary>
/// Class <c>GenericEntry</c> is the base abstract class for entries in a file system (file/folder)
/// </summary>
public abstract class GenericEntry
{
	protected string name;
    protected string path;
    protected string id;
    protected GenericFolder? parent;

	public DateTimeOffset? ModifiedTime { get; }
	public DateTimeOffset? CreatedTime { get; }
    
    public string Name { get { return name; } }
    public string Path { get { return path; } }
    public string Id { get { return id; } }
    public GenericFolder? Parent { get { return parent; } }
    // FIXME Not working for Windows Drive (for ex I:\)
    public string FullPath { get { return this.Path + this.Name; } }

    public abstract bool IsFolderEntry();
    public abstract void Print();
}


/// <summary>
/// Class <c>GenericFile</c> is an abstract class for a file entry in a file system
/// </summary>
public abstract class GenericFile: GenericEntry
{
    protected Int64 filesize;

    public Int64 FileSize { get { return filesize; } }

    // ADD hashes ?
    // ADD extension ?
    // ADD mimetype

    /// <summary>
    /// Check if the entry is a folder or a file
    /// </summary>
    /// <returns>
    /// true if the entry is a folder, false if it's a file
    /// </returns>
    public override bool IsFolderEntry()
    {
        return false;
    }

    public override void Print()
    {
        Console.WriteLine($"{this.Name};{this.Path};{this.FullPath};{this.FileSize}");
    }
}



/// <summary>
/// Class <c>GenericFolder</c> is an abstract class for a folder entry in a file system
/// </summary>
public abstract class GenericFolder: GenericEntry
{
    protected List<GenericEntry> entries;

    /// <summary>
    /// <c>GenericFolder</c> constructor
    /// </summary>
    public GenericFolder()
    {
        this.entries = [];
    }

    /// <summary>
    /// Check if the entry is a folder or a file
    /// </summary>
    /// <returns>
    /// true if the entry is a folder, false if it's a file
    /// </returns>
    public override bool IsFolderEntry()
    {
        return true;
    }

	/// <summary>
	/// Iterate over entries in the folder entries
	/// </summary>
    public IEnumerable<GenericEntry> Entries
	{
		get
		{
            if (entries != null)
			    foreach (var e in entries)
    				yield return e;
		}
	}

    public void Add(GenericEntry entry)
    {
        this.entries.Add(entry);
    }


    public override void Print()
    {
        Console.WriteLine($"{this.Name};{this.Path};{this.FullPath};0");
    }
}
