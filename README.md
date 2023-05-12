# LongRunDurFun56
Long Running Durable Function -> Long running Activity

- Activity Function Delay is defaulted to 60 seconds for each of the three Activity executions performed on each Orchestration Execution
  - `local.settings.json` may look like this

``` Json
{
    "IsEncrypted": false,
    "Values": {
        "AzureWebJobsStorage": "UseDevelopmentStorage=true",
      "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
      "Delay": "15000"
    }
}
```

- In Azure use the following:
  - Add App Setting `delay` and set to the time in milliseconds intended for the Activity Delay
