using Amazon;
using Amazon.CloudWatchLogs;
using Amazon.CloudWatchLogs.Model;
using Newtonsoft.Json;

namespace AWSCloudWatchLoggingWithAWSSDK
{
    public class CloudWatchLogger
    {
        private IAmazonCloudWatchLogs _client;
        private string _logGroup;
        private string _logStream;

        private CloudWatchLogger(string logGroup)
        {
            // If don't have an AWS Profile on your machine and application is hosted outside
            // of AWS infrastructure (where IAM roles cannot be assigned to infrastructure),
            // rather use:
            // _client = new AmazonCloudWatchLogsClient(awsAccessKeyId, awsSecretAccessKey, RegionEndpoint.AFSouth1);
            _client = new AmazonCloudWatchLogsClient();
            _logGroup = logGroup;
        }

        public static async Task<CloudWatchLogger> GetLoggerAsync(string logGroup)
        {
            var logger = new CloudWatchLogger(logGroup);

             // Create a log group
            await logger.CreateLogGroupAsync();

            // Create a log stream
            await logger.CreateLogStreamAsync();

            return logger;
        }

        private async Task CreateLogGroupAsync()
        {
            var existingLogGroups = await _client.DescribeLogGroupsAsync();
            if (existingLogGroups.LogGroups.Any(x => x.LogGroupName == _logGroup))
                return;

            _ = await _client.CreateLogGroupAsync(new CreateLogGroupRequest()
            {
                LogGroupName = _logGroup
            });
        }

        private async Task CreateLogStreamAsync()
        {
            _logStream = DateTime.UtcNow.ToString("dd/MM/yyyy");
            DescribeLogStreamsRequest req = new DescribeLogStreamsRequest(_logGroup);
            var LogStreamList = _client.DescribeLogStreamsAsync(req);
            var existingLogStream = LogStreamList.Result.LogStreams.Any(x=>x.LogStreamName == _logStream);
            if(existingLogStream)
                return;

            _ = await _client.CreateLogStreamAsync(new CreateLogStreamRequest()
            {
                LogGroupName = _logGroup,
                LogStreamName = _logStream
            });
        }

            public async Task LogMessageAsync(string message)
            {
                _ = await _client.PutLogEventsAsync(new PutLogEventsRequest()
                {
                    LogGroupName = _logGroup,
                    LogStreamName = _logStream,
                    LogEvents = new List<InputLogEvent>()
                    {
                        new InputLogEvent()
                        {
                            Message = message,
                            Timestamp = DateTime.UtcNow
                        }
                    }
                });
            }

            public async Task Info(string title, string description)
            {
                var message = JsonConvert.SerializeObject(
                new
                {
                    Title = title,
                    Desc = description
                });

                _ = await _client.PutLogEventsAsync(new PutLogEventsRequest()
                {
                    LogGroupName = _logGroup,
                    LogStreamName = _logStream,
                    LogEvents = new List<InputLogEvent>()
                    {
                        new InputLogEvent()
                        {
                            Message = "Info : " + message,
                            Timestamp = DateTime.UtcNow
                        }
                    }
                });
            }

            public async Task Error(string title, string description, object? data)
            {
                var message = JsonConvert.SerializeObject(
                new
                {
                    Title = title,
                    Desc = description,
                    Object = data
                });

                _ = await _client.PutLogEventsAsync(new PutLogEventsRequest()
                {
                    LogGroupName = _logGroup,
                    LogStreamName = _logStream,
                    LogEvents = new List<InputLogEvent>()
                    {
                        new InputLogEvent()
                        {
                            Message = "Error : " + message,
                            Timestamp = DateTime.UtcNow
                        }
                    }
                });
            }
    }
}