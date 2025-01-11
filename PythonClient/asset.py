
import logging
import grpc
import protos.asset_pb2 as asset_pb2
import protos.asset_pb2_grpc as asset_pb2_grpc


def run():
    with grpc.insecure_channel("localhost:8080") as channel:
        # path = "/Users/himanshu/Downloads/DamagedHelmet.glb"
        # path = "/Users/himanshu/Code/glTF-Sample-Assets-main/Models/FlightHelmet/glTF/FlightHelmet.gltf"
        path = "/Users/himanshu/Code/glTF-Sample-Assets-main/Models/Sponza/glTF/Sponza.gltf"
        stub = asset_pb2_grpc.AssetServiceStub(channel)
        stub.LoadFromPath(asset_pb2.LoadFromPathRequest(path=path))
        print("Asset loaded from path", path)


if __name__ == "__main__":
    logging.basicConfig()
    run()
