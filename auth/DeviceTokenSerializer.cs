using System.Text;
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

    public static TwitchDeviceToken? LoadDeviceToken()
    {
        string appDataLocalDirPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        string appDir = Path.Combine(appDataLocalDirPath, "TwitchIntegrationLibrary");

        if (!Directory.Exists(appDir))
        {
            return null;
        }

        string fileDir = Path.Combine(appDir, "twitch_device_token.json");

        return JsonSerializer.Deserialize<TwitchDeviceToken>(File.ReadAllText(fileDir, Encoding.UTF8));
    }
}