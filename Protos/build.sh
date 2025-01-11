#!/bin/bash

# Path to the Protos folder
# Output paths
C_SHARP_OUT=../Assets/Protos
PYTHON_OUT=../PythonClient/protos

# Ensure the output folder exists
mkdir -p $PYTHON_OUT

# Generate Python gRPC files with correct relative imports
python -m grpc_tools.protoc -I ./ --python_out=$PYTHON_OUT --grpc_python_out=$PYTHON_OUT ./*.proto

# Generate gRPC code for Unity (C# server)
protoc -I ./ --csharp_out=$C_SHARP_OUT --grpc_out=$C_SHARP_OUT --plugin=protoc-gen-grpc=$(which grpc_csharp_plugin) ./*.proto

echo "gRPC code generation complete."
