# Data Product ⬡

[![codecov](https://codecov.io/gh/devantler/data-product/branch/main/graph/badge.svg?token=9lh1Z59deC)](https://codecov.io/gh/devantler/data-product)

![concept](https://github.com/devantler/data-product/assets/26203420/da456d38-d6e8-445c-8980-e1e855e955b9)

This repo contains a Data Product as defined by Zhamak Dehghani in the Book [Data Mesh](https://www.oreilly.com/library/view/data-mesh/9781492092384/). A data product is the central unit of the data mesh, and operates as a service that provides dedicated data storage, data processing, data discovery-, and data governance- tooling for a specific domain model. In this context, a domain model is the schema of some data that covers a concrete domain, e.g. Accounts, Books, Authors etc.

The data product is built to support most cloud providers and provisioning tools by being built as a container.

<details>
  <summary>Show/hide folder structure</summary>

<!-- readme-tree start -->
```
```
<!-- readme-tree end -->

</details>

## Prerequisites

- .NET 7.0+

Optionally the following infrastructure:

- Kafka
- Kafka Schema Registry
- Jaeger
- Grafana
- Prometheus
- Elasticsearch
- Redis
- LinkedIn DataHub
- PostgreSQL
- Keycloak
