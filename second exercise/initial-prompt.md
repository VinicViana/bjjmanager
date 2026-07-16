# The prompt I used

```
Generate a REST API for a task management system using ASP.NET Core 8 (C#, controller-based,
not minimal APIs). This is backend/API only — no frontend, no UI, nothing to render.

Requirements:
- A Task entity with: Id, Title, Description, Status, DueDate, and a UserId linking it to an
  existing User (assume the User model/table already exists — don't generate it).
- Full CRUD endpoints: list, get by id, create, update, delete.
- Use Entity Framework Core with an in-memory provider so it runs without a real database.
- Add authentication: the API should require a valid JWT bearer token on every task endpoint,
  and the caller's user id should come from a claim on that token — not from the request body.
  Tasks belong to whichever user is authenticated on the request.

Give me the entity class, the DbContext, and the controller.
```
