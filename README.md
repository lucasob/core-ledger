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

