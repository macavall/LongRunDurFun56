using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;

namespace LongRunDurFun56
{
    public static class Function1
    {
        public static string Delay = Environment.GetEnvironmentVariable("delay") ?? "60000";

        [Function(nameof(Function1))]
        public static async Task RunOrchestrator(
            [OrchestrationTrigger] TaskOrchestrationContext context)
        {
            var options = TaskOptions.FromRetryPolicy(new RetryPolicy(
                maxNumberOfAttempts: 3,
                firstRetryInterval: TimeSpan.FromSeconds(5)));

            await context.CallActivityAsync(nameof(SayHello), options: options);

            ILogger logger = context.CreateReplaySafeLogger(nameof(Function1));
            logger.LogInformation("Saying hello.");
            //var outputs = new List<string>();

            // Replace name and input with values relevant for your Durable Functions Activity
            //outputs.Add(await context.CallActivityAsync<string>(nameof(SayHello), "Tokyo"));
            //outputs.Add(await context.CallActivityAsync<string>(nameof(SayHello), "Seattle"));
            //outputs.Add(await context.CallActivityAsync<string>(nameof(SayHello), "London"));

            // returns ["Hello Tokyo!", "Hello Seattle!", "Hello London!"]
            //return outputs;
        }

        [Function(nameof(SayHello))]
        public static async Task<string> SayHello([ActivityTrigger] string name, FunctionContext executionContext)
        {
            throw new Exception();

            ILogger logger = executionContext.GetLogger("SayHello");

            Thread.Sleep(Convert.ToInt32(Delay));

            logger.LogInformation("Saying hello to {name}.", name);
            return $"Hello {name}!";
        }

        [Function("Function1_HttpStart")]
        public static async Task<HttpResponseData> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req,
            [DurableClient] DurableTaskClient client,
            FunctionContext executionContext)
        {
            ILogger logger = executionContext.GetLogger("Function1_HttpStart");

            // Function input comes from the request content.
            string instanceId = await client.ScheduleNewOrchestrationInstanceAsync(
                nameof(Function1));

            logger.LogInformation("Started orchestration with ID = '{instanceId}'.", instanceId);

            // Returns an HTTP 202 response with an instance management payload.
            // See https://learn.microsoft.com/azure/azure-functions/durable/durable-functions-http-api#start-orchestration
            return client.CreateCheckStatusResponse(req, instanceId);
        }
    }
}
