using System.Text.Json;
using TIL.DataContainers;

namespace TIL.Serialization;

static class DeviceTokenSerializer
{
    public static void SaveDeviceToken(TwitchDeviceToken twitchDeviceToken)
    {
        string appDataLocalDirPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        string appDir = Path.Combine(appDataLocalDirPath, "TwitchIntegrationLibrary");

        Directory.CreateDirectory(appDir);

        string fileDir = Path.Combine(appDir, "twitch_device_token.json");

        string deviceTokenJson = JsonSerializer.Serialize(twitchDeviceToken, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        File.WriteAllText(fileDir, deviceTokenJson);
    }
}