using System.Runtime.ExceptionServices;
using Newtonsoft.Json.Serialization;

namespace GDrive_test;


// TODO Add scan options (folder, mimetype, filesize min, file ext, wildcard)
// put in a dict pour extended options, beware not all options have the same type (string, int64, bool)

public abstract class GenericDriveScan
{
    // scan options management settings
    // TODO Add settings and var for managing mimetype filtering, wildcard, ext filtering
    protected List<string>? MimeTypesFilter = null; // Init with empty list instead ?
    protected HashSet<string>? ExtensionsFilter = null; // Init with empty list instead ?
    protected bool UseExtensionFilter = false;
    protected bool UseWildcardFilter = false;
    // generic scan options
    /// <value>Indicates if only folders should be retrieved from the scan</value>
    public bool ScanFoldersOnly { get ;  set; } = false;
    public bool ScanRecursive { get ;  set; } = false;
    public Int64 MinFileSize { get ;  set; } = Int64.MaxValue; // put max(int64) to ease tests or -1 ?
    // Information set during scan
    public int FileCount { get ; protected set; } = 0;
    public int FolderCount { get ; protected set; } = 0;

    protected void SetExentions(List<string>? extslist)
    {
        if (extslist == null)
        {
            // TODO BEWARE of side effect with mime types filter
            UseExtensionFilter = false;
        }
        else
        {
            UseExtensionFilter = true;
            //TODO compute
        }
    }
    public abstract GenericFolder? ScanFolder(string startingId);

    // TODO Add a helper method to check scan options over a file name and filesize
    // TODO Rename
    protected bool MatchScanOptions(string filename, string extension, Int64 filesize)
    {
        if (this.ScanFoldersOnly)
            return false;

        if (this.MinFileSize > filesize)
            return false;

        // check extensions
        if (this.UseExtensionFilter)
        {
            if (this.ExtensionsFilter != null)
                if (! this.ExtensionsFilter.Contains(extension))
                    return false;
        }

        // check wildcard
        // https://www.codeproject.com/Articles/11556/Converting-Wildcards-to-Regexes

        // check mimetypes

        return true;
    }
}

