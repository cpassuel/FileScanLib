namespace GDrive_test;

using System.ComponentModel;
using System.IO;


public class WindowsDriveScan: GenericDriveScan
{
    // https://stackoverflow.com/questions/12332451/list-all-files-and-directories-in-a-directory-subdirectories
    // https://learn.microsoft.com/en-us/dotnet/api/system.io.fileinfo?view=net-8.0
    // https://learn.microsoft.com/fr-fr/dotnet/api/system.io.directory.getfiles?view=net-8.0
    // https://learn.microsoft.com/fr-fr/dotnet/api/system.io.directory.enumeratefiles?view=net-8.0
    
    /// <summary>
    /// Scan the folder from the id
    /// </summary>
    /// <param name="startingId">Path to scan</param>
    public override WDriveFolderEntry? ScanFolder(string startingId)
    {
        FileCount = 0;
        FolderCount = 0;
      
        // ADD Exception if not a directory ?
        if (! Directory.Exists(startingId))
            return null;
        
        DirectoryInfo di = new(startingId);
        var folder = new WDriveFolderEntry(di, null);

        // Retrieve files
        // TODO Add test for scanFolder option
        if (! this.ScanFoldersOnly)
        {
            string [] fileEntries = Directory.GetFiles(startingId);
            foreach(string fileName in fileEntries)
            {
                // TODO Add filter on ext, wildcard and mime
                FileInfo fi = new(fileName);

                // TODO Refactor functions into one function
                // Apply all filters
                if (fi.Length < this.MinFileSize)
                    continue;

                // https://learn.microsoft.com/fr-fr/dotnet/api/system.io.path.getextension?view=net-8.0
                // extension = Path.GetExtension(fileName);

                //Console.WriteLine($"DEBUG File {fileName}");
                var file = new WDriveFileEntry(fi, folder);
                folder.Add(file);
                FileCount++;
            }
        }

        // Retrieve folders;
        string [] subdirectoryEntries = Directory.GetDirectories(startingId);
        foreach(string subdirectory in subdirectoryEntries)
        {
            //Console.WriteLine($"DEBUG Folder {subdirectory}");
            var subfolder = new WDriveFolderEntry(new DirectoryInfo(subdirectory), folder);
            folder.Add(subfolder);
            FolderCount++;

            if (this.ScanRecursive)
                ScanFolderRecursive(subdirectory);
        }

        // TODO Add recursive scan and check options

        return folder;
    }
    
    protected void ScanFolderRecursiveBis(WDriveFolderEntry f)
    {
        // retrieve the path
        string FolderPath = f.FullPath;

        // Retrieve files is needed
        if (! this.ScanFoldersOnly)
        {
            string [] fileEntries = Directory.GetFiles(FolderPath);
            foreach(string fileName in fileEntries)
            {
                FileInfo fi = new(fileName);

                // TODO Add filter on ext, wildcard and mime
                // https://learn.microsoft.com/fr-fr/dotnet/api/system.io.path.getextension?view=net-8.0
                // extension = Path.GetExtension(fileName);

                // TODO Refactor functions into one function
                // Apply all filters
                if (fi.Length < this.MinFileSize)
                    continue;

                // fi.Extension;

                //Console.WriteLine($"DEBUG File {fileName}");
                var file = new WDriveFileEntry(fi, f);
                f.Add(file);
                FileCount++;
            }
        }

        // Retrieve folders;
        string [] subdirectoryEntries = Directory.GetDirectories(FolderPath);
        foreach(string subdirectory in subdirectoryEntries)
        {
            //Console.WriteLine($"DEBUG Folder {subdirectory}");
            var subfolder = new WDriveFolderEntry(new DirectoryInfo(subdirectory), f);
            f.Add(subfolder);
            FolderCount++;

            if (this.ScanRecursive)
                ScanFolderRecursive(subdirectory);
        }
    }

  
    protected void ScanFolderRecursive(string targetDirectory)
    {
        // Process the list of files found in the directory.
        if (! this.ScanFoldersOnly)
        {
            string [] fileEntries = Directory.GetFiles(targetDirectory);
            foreach(string fileName in fileEntries)
            {
                FileCount++;
                ProcessFile(fileName);
            }
        }

        // Recurse into subdirectories of this directory.
        string [] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
        foreach(string subdirectory in subdirectoryEntries)
        {
            FolderCount++;
            ScanFolderRecursive(subdirectory);
        }
    }


    // Temporary
    protected void ProcessFile(string fileName)
    {
        return;
    }
}
