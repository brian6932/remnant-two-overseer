namespace RemnantOverseer.Utilities;
internal static class NotificationStrings
{
    public static string DefaultLocationNotFound = "Could not detect the location of the save folder. Set it manually in the settings";
    public static string DefaultLocationFound = "Save file location was found and set";

    public static string SaveFileParsingError = "An error was encountered while parsing the save file. Message:";
    public static string FileWatcherFolderNotFound = "The folder with the requested file was not found. Ensure that the path is correct and restart the application";

    public static string SaveFileLocationChanged = "Save file location was changed successfully";
}
