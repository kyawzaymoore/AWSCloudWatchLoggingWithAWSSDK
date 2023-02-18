using AWSCloudWatchLoggingWithAWSSDK;
using Newtonsoft.Json;

var logger = await CloudWatchLogger.GetLoggerAsync("/dotnet/logging-demo/awssdk");

Console.WriteLine("Hello");

await logger.LogMessageAsync("This is my first message!");
await logger.LogMessageAsync("this is another message much like the first!");
await logger.Info("hihi","hello");
await logger.Error("Title","body", new {Name = "KZM", Job = "Developer"});
await logger.Error("Title","body", null);