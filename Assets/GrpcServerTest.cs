using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grpc.Core;
using System.Threading.Tasks;

public class GrpcServerTest : MonoBehaviour
{
    Server server;

    const string Host = "localhost";
    public int Port = 8080;

    // Start is called before the first frame update
    void Start()
    {
        server = new Server
        {
            Services = { Helloworld.Greeter.BindService(new GrpcServiceImpl()) },
            Ports = { new ServerPort(Host, Port, ServerCredentials.Insecure) }
        };

        Debug.Log("GrpcServerTest Start");

        server.Start();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDestroy()
    {
        server?.ShutdownAsync().Wait();
    }

    private void OnApplicationQuit()
    {
        server?.ShutdownAsync().Wait();
    }
}

public class GrpcServiceImpl : Helloworld.Greeter.GreeterBase
{
    public override Task<Helloworld.HelloReply> SayHello(Helloworld.HelloRequest request, ServerCallContext context)
    {
        return Task.FromResult(new Helloworld.HelloReply
        {
            Message = "Hello " + request.Name
        });
    }
}