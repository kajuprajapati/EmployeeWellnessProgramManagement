# EmployeeWellnessProgramManagement
This service will allow administrators to create wellness challenges, and employees to track their progress and earn rewards. The system needs to be scalable and resilient to handle a large number of concurrent users participating in various challenges.
Create and manage wellness challenges with Name, StartDate, EndDate, and Goal.
Employees can submit progress for challenges (steps, activity, etc.).
Top 10 leaderboard per challenge, updated in real-time using Redis.
Employees can view active challenges they are participating in.
Participants auto-created on first progress submission—no separate join endpoint needed.
High-volume submissions handled asynchronously via RabbitMQ.
Background worker updates DB and Redis leaderboard in batches.
Concurrency-safe updates ensure accurate cumulative progress.
Database indexes optimize leaderboard and participant queries.
DTO-based API separates request/response models from database entities.
Built to be scalable, resilient, and performant for large concurrent user loads.