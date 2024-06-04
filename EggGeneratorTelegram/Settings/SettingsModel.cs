namespace EggGeneratorTelegram.Settings
{
	public class SettingsModel
	{
		public SettingsModel()
		{
			_telegramToken = Environment.GetEnvironmentVariable(Constants.TokenVariable) ?? string.Empty;
		}

		private string _telegramToken = string.Empty;
		public string TelegramToken
		{
			get { return _telegramToken; }
			set { _telegramToken = value; }
		}
	}
}
