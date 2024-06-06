using EggGeneratorTelegram.Settings;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Fonts;
using SixLabors.ImageSharp.Drawing.Processing;

namespace EggGeneratorTelegram
{
	public class Worker : BackgroundService
	{
		private TelegramBotClient _telegramClient;
		private FontCollection _fontCollection = new FontCollection();
		private FontFamily _fontfamily;
		private Font _font;

		public Worker()
		{
			_telegramClient = new TelegramBotClient(AppSettings.Current.TelegramToken);
			_fontfamily = _fontCollection.Add("Resources/precursoralpha.ttf");
			_font = _fontfamily.CreateFont(120, FontStyle.Regular);
		}

		public async override Task StartAsync(CancellationToken cancellationToken)
		{
			using CancellationTokenSource cts = new();

			ReceiverOptions receiverOptions = new()
			{
				AllowedUpdates = Array.Empty<UpdateType>() // receive all update types except ChatMember related updates
			};

			_telegramClient.StartReceiving(
				updateHandler: HandleUpdateAsync,
				pollingErrorHandler: HandlePollingErrorAsync,
				receiverOptions: receiverOptions,
				cancellationToken: cts.Token
			);

			User me = await _telegramClient.GetMeAsync();
			Console.WriteLine($"Hello, World! I am user {me.Id} and my name is {me.FirstName}.");
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				await Task.Delay(1000, stoppingToken);
			}
		}

		private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
		{
			long? chatID = update.Message?.Chat.Id;
			string chatMessage = update.Message.Text ?? string.Empty;

			if (chatID != null)
			{
				Console.WriteLine($"Received a message in chat {chatID}.");

				//Message sentMessage = await botClient.SendTextMessageAsync(
				//	chatId: chatID,
				//	text: chatMessage,
				//	cancellationToken: cancellationToken);

				TextOptions options = new TextOptions(_font)
				{
					Dpi = 72,
					KerningMode = KerningMode.Auto
				};

				FontRectangle rect = TextMeasurer.MeasureAdvance(chatMessage, options);

				using (Image image = new Image<Rgba32>((int)rect.Width, (int)rect.Height))
				{
					if (System.IO.File.Exists("test.bmp"))
					{
						System.IO.File.Delete("test.bmp");
					}

					image.Mutate(x => x.DrawText(
						chatMessage,
						_font,
						new SixLabors.ImageSharp.Color(Rgba32.ParseHex("#FFFFFFEE")),
						new PointF(image.Width - rect.Width,
								image.Height - rect.Height)));

					image.SaveAsBmp("test.bmp");

					using (StreamReader streamReader = new StreamReader("test.bmp"))
					{
						await botClient.SendPhotoAsync(
							chatId: chatID,
							photo: InputFile.FromStream(streamReader.BaseStream),
							caption: chatMessage,
							cancellationToken: cancellationToken);
					}

					if (System.IO.File.Exists("test.bmp"))
					{
						System.IO.File.Delete("test.bmp");
					}
				}
			}
		}

		private Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
		{
			var ErrorMessage = exception switch
			{
				ApiRequestException apiRequestException
					=> $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
				_ => exception.ToString()
			};

			Console.WriteLine(ErrorMessage);
			return Task.CompletedTask;
		}
	}
}
