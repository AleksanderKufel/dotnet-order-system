# Order Processing System

A small distributed system in .NET: an order comes in over HTTP, goes onto a queue, and is processed by a separate worker process. Built to practise event-driven messaging, background processing and caching outside of a single-process application.

## Stack

.NET 10 · ASP.NET Core Minimal API · RabbitMQ · .NET Hosted Services · PostgreSQL (EF Core) · Redis · Docker Compose

## How it works

1. `POST /orders` saves the order to PostgreSQL with status `Pending` and publishes an `OrderCreated` event to RabbitMQ.
2. The worker — a separate process — consumes the event, does the work, marks the order `Processed` and clears its cache entry.
3. `GET /orders/{id}` reads from Redis first and falls back to PostgreSQL, caching the result.

## Design notes

**The worker is its own deployable, not a background task inside the API.** It has a separate Dockerfile and runs as a separate container, so processing capacity scales independently of the HTTP front end and restarting one does not interrupt the other.

**Messages are acknowledged manually.** The consumer runs with `autoAck: false` and acknowledges only after the order has been saved, so a crash in the middle of processing leaves the message on the queue instead of dropping it.

**The consumer checks whether an order has already been processed** before doing the work, so a redelivered message does not process the same order twice.

**Redis holds read results, not state.** `GET /orders/{id}` is cache-aside with a short TTL, and the worker deletes the key when it changes the status — so a read straight after processing does not come back `Pending`.

**The domain object owns its transitions.** `Order` exposes `MarkAsProcessed()` and `MarkAsFailed()` instead of a public setter on `Status`, so an invalid status cannot be assigned from outside.

## Running it

Requirements: Docker.

```bash
cd OrderSystem
docker compose up --build
```

- API — http://localhost:5000
- RabbitMQ management UI — http://localhost:15672 (`guest` / `guest`)

Create an order and read it back:

```bash
curl -X POST http://localhost:5000/orders \
  -H "Content-Type: application/json" \
  -d '{"customerEmail":"test@example.com","amount":99.90}'

curl http://localhost:5000/orders/{id}
```

The first read after creation returns `Pending`; once the worker picks the event up, the status becomes `Processed`.

## Known limitations and next steps

Listed rather than hidden — these are the things I would add next, in this order.

- **Dual write.** The order is saved and the event published as two separate operations. If the broker is unreachable after the commit, the order stays `Pending` forever. The fix is a transactional outbox: write the event to a table in the same transaction and dispatch it from a background loop.
- **No delivery guarantees on publish.** The queue is not declared durable, messages are not persistent and there are no publisher confirms, so a broker restart loses in-flight events.
- **No dead-letter queue.** A message that always throws is requeued indefinitely. This needs bounded retries with backoff and a DLQ for what still fails.
- **Idempotency is a read-then-write check**, which holds for one consumer but not for several running in parallel. The robust version is a unique constraint on the message id.
- **No automated tests or CI here yet** — planned with Testcontainers for PostgreSQL and RabbitMQ, covering the full path from `POST` to `Processed`. Tests and a CI pipeline are in place in the sibling repo, [dotnet-reservation-system](https://github.com/AleksanderKufel/dotnet-reservation-system).
- **Operational gaps.** Logging goes to the console instead of `ILogger` with structured output, there are no health checks for PostgreSQL, RabbitMQ and Redis, and `GET /orders` returns entities with no paging or validation.
