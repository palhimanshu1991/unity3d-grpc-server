# Copyright 2015 gRPC authors.
#
# Licensed under the Apache License, Version 2.0 (the "License");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at
#
#     http://www.apache.org/licenses/LICENSE-2.0
#
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.
"""The Python implementation of the GRPC helloworld.Greeter client."""

from __future__ import print_function

import logging
import grpc
import protos.simulation_pb2 as simulation_pb2
import protos.simulation_pb2_grpc as simulation_pb2_grpc
import time

def run():
    with grpc.insecure_channel("localhost:8080") as channel:
        stub = simulation_pb2_grpc.SimulationServiceStub(channel)
        while True:
            response = stub.Tick(simulation_pb2.TickRequest())
            frame_response = stub.GetFrame(simulation_pb2.GetFrameRequest())
            print(frame_response.frame)
            time.sleep(0.02)


if __name__ == "__main__":
    logging.basicConfig()
    run()