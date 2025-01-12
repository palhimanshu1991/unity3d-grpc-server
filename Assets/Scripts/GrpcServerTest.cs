using System;
using System.IO;
using UnityEngine;
using Grpc.Core;
using System.Threading.Tasks;
using GLTFast;
using Truevision.Asset;
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

    public uint GetFrameCounter()
    {
        return frameCount;
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
                Truevision.Simulation.SimulationService.BindService(new SimServiceImpl()),
                Truevision.Asset.AssetService.BindService(new AssetServiceImp()),
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
    public override async Task<TickResponse> Tick(TickRequest request, ServerCallContext context)
    {
        await MainThreadDispatcher.ScheduleAsync(() =>
        {
            Simulation.Instance.Tick();
        });

        return new TickResponse();
    }

    public override async Task<GetFrameResponse> GetFrame(GetFrameRequest request, ServerCallContext context)
    {
        var frame = await MainThreadDispatcher.ScheduleAsync(() => Simulation.Instance.GetFrameCounter());

        return new GetFrameResponse
        {
            Frame = frame
        };
    }
}

public class AssetServiceImp : AssetService.AssetServiceBase
{
    public override async Task<LoadFromPathResponse> LoadFromPath(LoadFromPathRequest request, ServerCallContext context)
    {
        try
        {
            // Schedule and wait for the Unity main-thread task
            await MainThreadDispatcher.ScheduleAsync(async () =>
            {
                // Load the GLB/GLTF file at runtime
                await ModelImporter.LoadGLBAsync(request.Path, Camera.main?.transform);
            });

            // Return success response
            return new LoadFromPathResponse();
        }
        catch (Exception e)
        {
            // Debug.LogError($"Failed to load GLB file: {e}");

            // Return failure response
            return new LoadFromPathResponse();
        }
    }
}

public abstract class ModelImporter
{
    public static async Task LoadGLBAsync(string filePath, Transform parentTransform)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            throw new System.ArgumentNullException(nameof(filePath), "Path cannot be null or empty.");
        }
        
        if (!File.Exists(filePath))
        {
            throw new System.IO.FileNotFoundException($"File not found : {filePath}");
        }
        
        var gltf = new GltfImport();

        // The URI of the original data is important for resolving relative URIs within the glTF
        var success = await gltf.Load(filePath);

        if (success) {
            success = await gltf.InstantiateMainSceneAsync(new GameObject("Instance 1").transform);
        }

    }
        
}