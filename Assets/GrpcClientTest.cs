using System.Collections;
using System.Collections.Generic;
using Grpc.Core;
using UnityEngine;

public class GrpcClientTest : MonoBehaviour
{
    const string Host = "localhost";
    const int Port = 50051;

    Channel channel;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("GrpcClientTest Start");

        // after delay of 1 second, call the server

        StartCoroutine(CallServer());
        
    }

    IEnumerator CallServer()
    {
        yield return new WaitForSeconds(5);

        Debug.Log("CallServer");

        channel = new Channel(Host + ":" + Port, ChannelCredentials.Insecure);

        var client = new Helloworld.Greeter.GreeterClient(channel);

        var reply = client.SayHello(new Helloworld.HelloRequest { Name = "Unity" });

        Debug.Log("Reply: " + reply.Message);

        channel.ShutdownAsync().Wait();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
