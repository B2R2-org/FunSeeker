FROM ubuntu:20.04

RUN apt-get update && apt-get upgrade -y && \
    apt-get install -y apt-transport-https && \
    apt-get update && \
    apt-get install -y dotnet-sdk-6.0

RUN git clone https://github.com:B2R2-org/FunSeeker.git && \
    cd FunSeeker && \
    dotnet build -c Release
