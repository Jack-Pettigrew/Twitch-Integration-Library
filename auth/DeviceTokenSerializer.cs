using System;
using System.IO;
using System.Text;
using System.Text.Json;
using TIL.DataContainers;

namespace TIL.Serialization;

static class DeviceTokenSerializer
{
    private const string DirectoryName = "TwitchIntegrationLibrary";
    private const string FileName = "twitch_device_token.json";
    private static readonly JsonSerializerOptions serializerOptions = new() {WriteIndented = true};

    public static void SaveDeviceToken(TwitchDeviceToken twitchDeviceToken)
    {
        if (twitchDeviceToken is null)
        {
            throw new ArgumentNullException(nameof(twitchDeviceToken));
        }

        string appDataLocalDirPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        string appDir = Path.Combine(appDataLocalDirPath, DirectoryName);

        Directory.CreateDirectory(appDir);

        string fileDir = Path.Combine(appDir, FileName);

        string deviceTokenJson = JsonSerializer.Serialize(twitchDeviceToken, serializerOptions);

        File.WriteAllText(fileDir, deviceTokenJson);
    }

    public static TwitchDeviceToken? LoadDeviceToken()
    {
        string appDataLocalDirPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        string appDir = Path.Combine(appDataLocalDirPath, DirectoryName);

        if (!Directory.Exists(appDir))
        {
            return null;
        }

        string fileDir = Path.Combine(appDir, FileName);

        if (!File.Exists(fileDir))
        {
            return null;
        }

        return JsonSerializer.Deserialize<TwitchDeviceToken>(File.ReadAllText(fileDir, Encoding.UTF8));
    }
}