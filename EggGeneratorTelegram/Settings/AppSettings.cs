using Newtonsoft.Json;
using System.Reflection;

namespace EggGeneratorTelegram.Settings
{
	public static class AppSettings
	{
		private static SettingsModel _currentSetting = new SettingsModel();
		public static SettingsModel Current
		{
			get { return _currentSetting; }
		}
		
		public static string GetSettingFilePath()
		{
			return Path.Combine(
				Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty,
				Constants.SettingFileName);
		}

		public static void Load()
		{
			_currentSetting = new SettingsModel();
			Save();	
			//if (File.Exists(GetSettingFilePath()))
			//{
			//	_currentSetting = JsonConvert.DeserializeObject<SettingsModel>(
			//		File.ReadAllText(GetSettingFilePath())
			//		) ?? new SettingsModel();
			//}
			//else
			//{
			//	_currentSetting = new SettingsModel();
			//	Save();	
			//}
		}

		public static void Save()
		{
			File.WriteAllText(
				GetSettingFilePath(),
				JsonConvert.SerializeObject(_currentSetting, Formatting.Indented));
		}
	}
}