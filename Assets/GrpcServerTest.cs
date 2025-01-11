using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grpc.Core;
using System.Threading.Tasks;

public class Simulation
{
    private uint frameCount = 0;
        
    void Tick()
    {
        frameCount++;
    }
}

public class GrpcServerTest : MonoBehaviour
{
    Server server;

    const string Host = "localhost";
    public int Port = 8080;
    
    private float deltaTime = 0.0f;
    private Simulation simulation = new Simulation();

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
        // Update deltaTime to calculate FPS
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
    }

    private void OnGUI()
    {
        // Calculate FPS
        float fps = 1.0f / deltaTime;

        // Display FPS on the screen
        GUI.Label(new Rect(10, 10, 100, 20), $"FPS: {fps:0.}");
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