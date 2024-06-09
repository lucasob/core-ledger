# API

# Requirements

* Dotnet 8.0

# Building

For some reason, the .Net Core template docker file expects you to run from the root, and point at the nested
Dockerfile.

So, from the repository **_root_**

```shell
docker build -t lucasob/coreledger:latest -f CoreLedger/Dockerfile .
```

# Running

## From Docker

Assuming you've built,

```shell
docker run --rm --name api -p 8080:8080 --expose 8080 lucasob/coreledger
```