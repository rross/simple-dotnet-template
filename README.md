# simple-dotnet-template
To be used as a template project to bootstrap other projects on the [Temporal .NET SDK](https://github.com/temporalio/sdk-dotnet)
## Run Worker Locally
```bash
dotnet run
```

## Start Workflow Locally
```bash
# run only once
temporal server start-dev
# trigger a workflow locally
temporal workflow start --type SimpleWorkflow --task-queue simple-task-queue --input '{"val":"foo"}'
```

## Run Worker using Temporal Cloud
```bash
# set up environment variables
export TEMPORAL_NAMESPACE=<namespace>.<accountId>
export TEMPORAL_ADDRESS=<namespace>.<accountId>.tmprl.cloud:7233
export TEMPORAL_TLS_CERT=/path/to/cert
export TEMPORAL_TLS_KEY=/path/to/key
# run the worker
dotnet run
```

## Start Workflow on Temporal Cloud
```bash
# set your temporal environment
temporal env set dev.namespace <namespace>.<accountId>
temporal env set dev.address <namespace>.<accountId>.tmprl.cloud:7233
temporal env set dev.tls-cert-path /path/to/cert
temporal env set dev.tls-key-path /path/to/key 
# trigger a workflow on Temporal Cloud
temporal workflow start --type SimpleWorkflow --task-queue simple-task-queue --input '{"val":"foo"}' --env dev
```