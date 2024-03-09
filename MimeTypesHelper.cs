namespace GDrive_test;

public class MimeTypesHelper
{
    // Mime type management
    // https://mimetype.io/all-types
    // List mime type => List of extensions or string with concated extensions
    // https://www.occasoftware.com/blog/dictionary-initialization-c
    // there is dupplicate of .m4a
    private static readonly Dictionary<string, string> MimeTypesExtDict = new()
    {
        { "audio/3gpp2",	".3g2" },
        { "audio/aac",	".aac, .m4a" },
        { "audio/aacp",	".aacp" },
        { "audio/adpcm",	".adp" },
        { "audio/aiff",	".aiff, .aif, .aff" },
        { "audio/basic",	".au, .snd" },
        { "audio/flac",	".flac" },
        { "audio/midi",	".kar, .mid, .midi, .rmi" },
        { "audio/mp4",	".mp4, .m4a, .m4b, .m4p, .m4r, .m4v, .mp4v, .3gp, .3g2, .3ga, .3gpa, .3gpp, .3gpp2, .3gp2" },
        { "audio/mp4a-latm",	"" },
        { "audio/mpeg",	".m2a, .m3a, .mp2, .mp2a, .mp3, .mpga" },
        { "audio/ogg",	".oga, .ogg, .spx" },
        { "audio/opus",	".opus" },
        { "audio/vnd.digital-winds",	".eol" },
        { "audio/vnd.dts",	".dts" },
        { "audio/vnd.dts.hd",	".dtshd" },
        { "audio/vnd.lucent.voice",	".lvp" },
        { "audio/vnd.ms-playready.media.pya",	".pya" },
        { "audio/vnd.nuera.ecelp4800",	".ecelp4800" },
        { "audio/vnd.nuera.ecelp7470",	".ecelp7470" },
        { "audio/vnd.nuera.ecelp9600",	".ecelp9600" },
        { "audio/vnd.wav",	".wav" },
        { "audio/webm",	".weba" },
        { "audio/x-aiff",	".aif, .aifc, .aiff" },
        { "audio/x-matroska",	".mka" },
        { "audio/x-mpegurl",	".m3u" },
        { "audio/x-ms-wax",	".wax" },
        { "audio/x-ms-wma",	".wma" },
        { "audio/x-pn-realaudio",	".ra, .ram" },
        { "audio/x-pn-realaudio-plugin",	".rmp" },
        { "image/avif",	".avif, .avifs" },
        { "image/bmp",	".bmp" },
        { "image/cgm",	".cgm" },
        { "image/g3fax",	".g3" },
        { "image/gif",	".gif" },
        { "image/heic",	".heif, .heic" },
        { "image/ief",	".ief" },
        { "image/jpeg",	".jpe, .jpeg, .jpg, .pjpg, .jfif, .jfif-tbnl, .jif" },
        { "image/pjpeg",	".jpe, .jpeg, .jpg, .pjpg, .jfi, .jfif, .jfif-tbnl, .jif" },
        { "image/png",	".png" },
        { "image/prs.btif",	".btif" },
        { "image/svg+xml",	".svg, .svgz" },
        { "image/tiff",	".tif, .tiff" },
        { "image/vnd.adobe.photoshop",	".psd" },
        { "image/vnd.djvu",	".djv, .djvu" },
        { "image/vnd.dwg",	".dwg" },
        { "image/vnd.dxf",	".dxf" },
        { "image/vnd.fastbidsheet",	".fbs" },
        { "image/vnd.fpx",	".fpx" },
        { "image/vnd.fst",	".fst" },
        { "image/vnd.fujixerox.edmics-mmr",	".mmr" },
        { "image/vnd.fujixerox.edmics-rlc",	".rlc" },
        { "image/vnd.ms-modi",	".mdi" },
        { "image/vnd.net-fpx",	".npx" },
        { "image/vnd.wap.wbmp",	".wbmp" },
        { "image/vnd.xiff",	".xif" },
        { "image/webp",	".webp" },
        { "image/x-adobe-dng",	".dng" },
        { "image/x-canon-cr2",	".cr2" },
        { "image/x-canon-crw",	".crw" },
        { "image/x-cmu-raster",	".ras" },
        { "image/x-cmx",	".cmx" },
        { "image/x-epson-erf",	".erf" },
        { "image/x-freehand",	".fh, .fh4, .fh5, .fh7, .fhc" },
        { "image/x-fuji-raf",	".raf" },
        { "image/x-icns",	".icns" },
        { "image/x-icon",	".ico" },
        { "image/x-kodak-dcr",	".dcr" },
        { "image/x-kodak-k25",	".k25" },
        { "image/x-kodak-kdc",	".kdc" },
        { "image/x-minolta-mrw",	".mrw" },
        { "image/x-nikon-nef",	".nef" },
        { "image/x-olympus-orf",	".orf" },
        { "image/x-panasonic-raw",	".raw, .rw2, .rwl" },
        { "image/x-pcx",	".pcx" },
        { "image/x-pentax-pef",	".pef, .ptx" },
        { "image/x-pict",	".pct, .pic" },
        { "image/x-portable-anymap",	".pnm" },
        { "image/x-portable-bitmap",	".pbm" },
        { "image/x-portable-graymap",	".pgm" },
        { "image/x-portable-pixmap",	".ppm" },
        { "image/x-rgb",	".rgb" },
        { "image/x-sigma-x3f",	".x3f" },
        { "image/x-sony-arw",	".arw" },
        { "image/x-sony-sr2",	".sr2" },
        { "image/x-sony-srf",	".srf" },
        { "image/x-xbitmap",	".xbm" },
        { "image/x-xpixmap",	".xpm" },
        { "image/x-xwindowdump",	".xwd" },
        { "video/3gpp",	".3gp" },
        { "video/3gpp2",	".3g2" },
        { "video/h261",	".h261" },
        { "video/h263",	".h263" },
        { "video/h264",	".h264" },
        { "video/jpeg",	".jpgv" },
        { "video/jpm",	".jpgm, .jpm" },
        { "video/mj2",	".mj2, .mjp2" },
        { "video/mp2t",	".ts" },
        { "video/mp4",	".mp4, .mp4v, .mpg4" },
        { "video/mpeg",	".m1v, .m2v, .mpa, .mpe, .mpeg, .mpg" },
        { "video/ogg",	".ogv" },
        { "video/quicktime",	".mov, .qt" },
        { "video/vnd.fvt",	".fvt" },
        { "video/vnd.mpegurl",	".m4u, .mxu" },
        { "video/vnd.ms-playready.media.pyv",	".pyv" },
        { "video/vnd.vivo",	".viv" },
        { "video/webm",	".webm" },
        { "video/x-f4v",	".f4v" },
        { "video/x-fli",	".fli" },
        { "video/x-flv",	".flv" },
        { "video/x-m4v",	".m4v" },
        { "video/x-matroska",	".mkv" },
        { "video/x-ms-asf",	".asf, .asx" },
        { "video/x-ms-wm",	".wm" },
        { "video/x-ms-wmv",	".wmv" },
        { "video/x-ms-wmx",	".wmx" },
        { "video/x-ms-wvx",	".wvx" },
        { "video/x-msvideo",	".avi" },
        { "video/x-sgi-movie",	".movie" }
    };

    protected static Dictionary<string, string> ExtensionToMimeDict = [];

    // TODO generate a dict ext => mime type ? (static contructor https://www.google.com/search?client=firefox-b-d&q=c%23+static+constructor)
    
	/// <summary>
	/// Class constructor
	/// </summary>
    static MimeTypesHelper()
    {
        // Alim dict extension => mimetype (remove empty ext and duplicate)
        foreach (KeyValuePair<string,string> kvp in MimeTypesExtDict)
        {
            string[] mimeExtArray = kvp.Value.Split(',');
            foreach(var ext in mimeExtArray)
                if (ext.Trim() != "")
                    ExtensionToMimeDict.TryAdd(ext.Trim(), kvp.Key);
        }
    }


	/// <summary>
	/// Returns the mimetype associated to an file extension if it exists
	/// </summary>
   	/// <param name="ext">file extension to be searched (must contains the . like .png)</param>
    /// <returns>
    /// Returns the mimetype associated to the extension or empty string is the is none defined
    /// </returns>
    public static string GetMimeTypeFromExt(string ext)
    {
        if (ExtensionToMimeDict.TryGetValue(ext, out string? mimetype))
            return mimetype;
        else
            return "";
    }


    protected static void GetExtensionsFromMime(List<string> list_exts, string extsString)
    {
        string[] mimeExtArray = extsString.Split(',');

        foreach (var ext in mimeExtArray)
        {
            list_exts.Add(ext.Trim());
            Console.WriteLine($"Adding extension {ext.Trim()}");
        }
    }


    // TODO return a collection optimized for lookup/search https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.hashset-1?view=net-8.0&redirectedfrom=MSDN
    public static List<string> GetExtensionsFromMimeTypes(List<string> mimeTypesList)
    {
        List<string> list_exts = [];

        foreach (var mimetype in mimeTypesList)
        {
            // basic matching supported (* at the end of mime type like image/*)
            if (mimetype.EndsWith('*'))
            {
                string mimeTypePrefix = mimetype.TrimEnd('*');

                // Iterate over the dict to find matching mime types
                foreach (KeyValuePair<string,string> kvp in MimeTypesExtDict)
                {
                    // TODO Optimize: calling GetExtensionsFromMime will access dict a second time (get also value)
                    // https://www.codeproject.com/Tips/57304/Use-wildcard-Characters-and-to-Compare-Strings
                    if (kvp.Key.StartsWith(mimeTypePrefix))
                        GetExtensionsFromMime(list_exts, kvp.Value);
                }
            }
            else
                if (MimeTypesExtDict.TryGetValue(mimetype, out string? mimeExts))
                    GetExtensionsFromMime(list_exts, mimeExts);
        }

        return list_exts;
    }


	/// <summary>
	/// Compute the root mimetype (for ex. image) and its associated mimetypes (for ex. image/png, image/gif) into a dictionary
    /// Used to represent a tree of mimetypes
	/// </summary>
    /// <returns>
    /// dictionary of root mimetypes with its associated full mimetypes
    /// </returns>
    public static Dictionary<string, List<string>> GetMimeTypesTree()
    {
        Dictionary<string, List<string>> MimeTree = [];
        
        foreach (string mimetype in MimeTypesExtDict.Keys)
        {
            string[] mimetypesplit = mimetype.Split('/');
            // retrieve the existing list of associated mimetypes
            if (MimeTree.TryGetValue(mimetypesplit[0], out List<string>? mimetypeslist))
            {
                mimetypeslist.Add(mimetypesplit[1]);
            }
            else
            {
                // New mimetype root and new associated list
                //Console.WriteLine($"DEBUG New root type {mimetypesplit[0]}");
                mimetypeslist = [];
                mimetypeslist.Add(mimetypesplit[1]);
                MimeTree.Add(mimetypesplit[0], mimetypeslist);
            }
            
            //Console.WriteLine($"DEBUG Adding mimetype {mimetypesplit[1]}");
        }

        return MimeTree;
    }
}
