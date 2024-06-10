# Core Ledger

Hello! 🤠

# What?

This project aims to work towards offering a **_bare minimum_** double-entry accounting esque ledger.

It exists purely to work through some concurrency control ideas.

The single main idea is guaranteeing a total order for a given grouping key.

To do this, we

* Load balance requests to ensure all are handled on the same instance
* Ensure all requests of a single group are performed sequentially

# The API

See inside [CoreLedger](./CoreLedger)

# Load Balancing

Earlier in this readme, we addressed the desire to be able to guarantee to send all requests of a specific identifier to the same server. We will do this using a load balancer. For the sake of this project, that is nginx.

To start our server now, using docker compose,

```shell
docker compose up --scale api=2
```

This brings up our api, with two duplicate instances. We load balance between the two instances, based on the hash of the grouping id header. This is done using nginx and the network configured in the [docker-compose](./docker-compose.yml) file.

```shell
curl --location '0.0.0.0:4000' --silent | jq
```

Will now return you something like the below. Note the hostname is actually the docker host, so we can verify we are hitting different containers.

```json
{
  "Hello": "World",
  "Host": "9dbdfff785c7"
}
```
