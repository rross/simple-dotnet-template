﻿using DotNetTemplate;
using Temporalio.Client;
using Temporalio.Worker;

String getEnvVarWithDefault(String envName, String defaultValue) 
{
    String? value = Environment.GetEnvironmentVariable(envName);
    if (String.IsNullOrEmpty(value)) 
    {
        return defaultValue;
    }
    return value; 
}

var address = getEnvVarWithDefault("TEMPORAL_ADDRESS","127.0.0.1:7233");
var temporalNamespace = getEnvVarWithDefault("TEMPORAL_NAMESPACE","default");
var tlsCertPath = getEnvVarWithDefault("TEMPORAL_TLS_CERT","");
var tlsKeyPath = getEnvVarWithDefault("TEMPORAL_TLS_KEY","");
TlsOptions? tls = null;
if (!String.IsNullOrEmpty(tlsCertPath) && !String.IsNullOrEmpty(tlsKeyPath))
{
    tls = new() {
        ClientCert = await File.ReadAllBytesAsync(tlsCertPath),
        ClientPrivateKey = await File.ReadAllBytesAsync(tlsKeyPath),
    };
}

var client = await TemporalClient.ConnectAsync(
    new(address)  
    { 
        Namespace = temporalNamespace,
        Tls = tls,
    });

using var tokenSource = new CancellationTokenSource();
Console.CancelKeyPress += (_, eventArgs) =>
{
  tokenSource.Cancel();
  eventArgs.Cancel = true;    
};

var activities = new MyActivities(4);

using var worker = new TemporalWorker(
    client,
    new TemporalWorkerOptions("simple-task-queue").
    AddAllActivities(activities).
    AddWorkflow<SimpleWorkflow>());

// Run worker until cancelled
Console.WriteLine("Running worker...");
try
{
    await worker.ExecuteAsync(tokenSource.Token);
}
catch( OperationCanceledException)
{
    Console.WriteLine("Worker cancelled");
}


