using EggGeneratorTelegram;
using EggGeneratorTelegram.Settings;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

var host = builder.Build();

AppSettings.Load();

host.Run();