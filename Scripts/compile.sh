#!/bin/bash

# Path to the Protos folder
PROTO_PATH=../Protos
# Output paths
C_SHARP_OUT=../Assets/Scripts
PYTHON_OUT=../PythonClient

# Generate gRPC code for Python client
python3 -m grpc_tools.protoc -I $PROTO_PATH --python_out=$PYTHON_OUT --pyi_out=$PYTHON_OUT --grpc_python_out=$PYTHON_OUT $PROTO_PATH/*.proto

# Generate gRPC code for Unity (C# server)
protoc -I $PROTO_PATH --csharp_out=$C_SHARP_OUT --grpc_out=$C_SHARP_OUT --plugin=protoc-gen-grpc=$(which grpc_csharp_plugin) $PROTO_PATH/*.proto

echo "gRPC code generation complete."
