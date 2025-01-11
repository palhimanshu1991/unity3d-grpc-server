using UnityEngine;
using Grpc.Core;
using System.Threading.Tasks;
using Truevision.Simulation;

public class Simulation
{
    public static Simulation Instance { get; } = new Simulation();
    
    private uint frameCount = 0;
        
    public void Tick()
    {
        frameCount++;
        
        Physics.Simulate(Time.fixedDeltaTime);
    }
}

public class GrpcServerTest : MonoBehaviour
{
    Server server;

    const string Host = "localhost";
    public int Port = 8080;
    
    private float deltaTime = 0.0f;
    private Simulation simulation = new Simulation();

    private void Awake()
    {
        Physics.simulationMode = SimulationMode.Script;
    }

    // Start is called before the first frame update
    void Start()
    {
        server = new Server
        {
            Services = {
                Truevision.Hello.Greeter.BindService(new GrpcServiceImpl()),
                Truevision.Simulation.SimulationService.BindService(new SimServiceImpl())
            },
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

public class GrpcServiceImpl : Truevision.Hello.Greeter.GreeterBase
{
    public override Task<Truevision.Hello.HelloReply> SayHello(Truevision.Hello.HelloRequest request, ServerCallContext context)
    {
        return Task.FromResult(new Truevision.Hello.HelloReply
        {
            Message = "Hello " + request.Name
        });
    }
}

public class SimServiceImpl : SimulationService.SimulationServiceBase
{
    public override Task<TickResponse> Tick(TickRequest request, ServerCallContext context)
    {
        MainThreadDispatcher.ScheduleAsync(() =>
        {
            Simulation.Instance.Tick();
        });

        return Task.FromResult(new TickResponse());
    }
}
