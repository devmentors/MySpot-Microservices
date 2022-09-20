# MySpot Microservices

## About

MySpot is a sample parking reservation app based on microservices. The overall architecture is mostly built using **event-driven** approach.

**How to start the solution?**
----------------

Start the infrastructure using [Docker](https://docs.docker.com/get-docker/):

```
cd compose
docker-compose -f infrastructure.yml build
docker-compose -f infrastructure.yml up -d
```

Start the separated services located under `src/Services` directory via `dotnet run` or with your favorite IDE or by using [Tye](https://github.com/dotnet/tye):

```
tye run
```

The services can be also started with Docker:

```
cd compose
docker-compose -f services.yml build
docker-compose -f services.yml up -d
```