# Core Ledger

Hello! 🤠

# What?

This project aims to work towards offering a **_bare minimum_** double-entry accounting esque ledger.

It exists purely to work through some concurrency control ideas.

The single main idea is guaranteeing a total order for a given grouping key.

To do this, we

* Load balance requests to ensure all are handled on the same instance
* Ensure all requests of a single group are performed sequentially

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