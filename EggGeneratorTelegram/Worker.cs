using EggGeneratorTelegram.Settings;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace EggGeneratorTelegram
{
	public class Worker : BackgroundService
	{
		private TelegramBotClient _telegramClient;

		public Worker()
		{
			_telegramClient = new TelegramBotClient(AppSettings.Current.TelegramToken);
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

			if (chatID != null)
			{
				Console.WriteLine($"Received a message in chat {chatID}.");

				Message sentMessage = await botClient.SendTextMessageAsync(
					chatId: chatID,
					text: $"EGG: {Guid.NewGuid()}",
					cancellationToken: cancellationToken);
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
