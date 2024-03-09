namespace GDrive_test;

using System.IO;

public abstract class WDriveEntry:GenericEntry
{
    // File and Folder entries are created from distinct object unlike Google Drive
    // Use file/folder name in constructor instead of File/DriveInfo object ? => can be factorized
}


public class WDriveFolderEntry: GenericFolder
{
    // https://learn.microsoft.com/en-us/dotnet/api/system.io.directoryinfo?view=net-8.0

    public WDriveFolderEntry(DirectoryInfo di, WDriveFolderEntry? parent)
    {
        this.entries = [];

        this.name = di.Name;
        this.path = di.FullName;    // FIXME Get the path of the folder
    }
}


public class WDriveFileEntry: GenericFile
{
    // https://learn.microsoft.com/en-us/dotnet/api/system.io.fileinfo?view=net-8.0    

    public WDriveFileEntry(FileInfo fi, WDriveFolderEntry parent)
    {
        this.parent = (GenericFolder) parent;

        this.name = fi.Name;
        this.filesize = fi.Length;
        this.path = fi.DirectoryName??"";
    }
}
