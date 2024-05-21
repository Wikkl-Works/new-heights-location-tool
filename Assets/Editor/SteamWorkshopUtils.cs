using Steamworks;
using System;
using System.IO;

public static class SteamWorkshopUtils
{
    public static string GetWorkshopUrl(this PublishedFileId_t itemId)
    {
        return GetWorkshopUrl(itemId.ToString());
    }

    public static string GetWorkshopUrl(this string itemId)
    {
        return $"steam://url/CommunityFilePage/{itemId}";
    }

    public static string GetModsRootFolder()
    {
        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData", "LocalLow", "Wikkl Works", "New Heights", "Mods");
    }

    public static string GetContentPath(this PublishedFileId_t itemId)
    {
        return GetContentPath(itemId.ToString());
    }

    public static string GetContentPath(this string itemId)
    {
        return Path.Combine(GetModsRootFolder(), $"item_{itemId}");
    }
}
