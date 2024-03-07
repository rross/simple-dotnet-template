using Temporalio.Client;

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

Console.WriteLine("Connecting to Temporal Server: " + address);

var client = await TemporalClient.ConnectAsync(
    new(address)  
    { 
        Namespace = temporalNamespace,
        Tls = tls,
    });

var firstParameter = new Parameters("aName");

Console.WriteLine("Calling the workflow with parameters: " + firstParameter);

var handle = await client.StartWorkflowAsync("SimpleWorkflow", [firstParameter], new() {
  TaskQueue = "simple-task-queue",
  Id = Guid.NewGuid().ToString(),
});

var response = await handle.GetResultAsync<Response>();

Console.WriteLine("The response is " + response.result);

