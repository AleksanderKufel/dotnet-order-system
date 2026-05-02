# Order Processing System (.NET)

A distributed order processing system built with .NET, featuring asynchronous messaging, background processing, and caching.

## Features

- REST API built with ASP.NET Core (Minimal API)
- Event-driven architecture using RabbitMQ
- Background processing with .NET Hosted Services
- PostgreSQL for persistent storage
- Redis caching for improved read performance
- Dockerized multi-service environment

## Application Flow

1. Client sends POST /orders
2. Order is saved to PostgreSQL
3. OrderCreated event is published to RabbitMQ
4. Worker consumes the event
5. Order is processed asynchronously
6. Status is updated and cached in Redis

## Running the project

### Requirements
- Docker

### Run
```bash
docker-compose up --build
