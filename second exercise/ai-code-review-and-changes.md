# Reviewing the generated code

The prompt (see [`prompt.md`](prompt.md)) generated a working controller — it compiled, and the
happy path worked fine in Swagger. Testing it for real turned up two real problems.

## 1. Any logged-in user could open anyone else's task

Generated:

```csharp
[HttpGet("{id}")]
public IActionResult GetById(int id)
{
    var task = _db.Tasks.First(t => t.Id == id);
    return Ok(task);
}
```

It never checked who was asking — the id was the only thing that mattered. Same pattern on
`Update` and `Delete`. Tested it: logged in as user A, created a task, then hit the same id with
user B's token — got the task back instead of a 404.

Fixed by scoping every lookup to the logged-in user:

```csharp
private Task<TaskItem?> FindOwnedTaskAsync(Guid id) =>
    _db.Tasks.FirstOrDefaultAsync(t => t.Id == id && t.UserId == CurrentUserId);
```

If the task exists but belongs to someone else, this returns `404`, same as if it didn't exist at
all.

## 2. `Status` accepted anything

Generated:

```csharp
public string Status { get; set; }
```

Nothing stopped `"donee"`, an empty string, or random casing from being saved.

Fixed by making it an enum:

```csharp
public TaskItemStatus Status { get; set; }
```

Also had to register a `JsonStringEnumConverter`, otherwise the API only accepted the enum as a
number (`0`, `1`, `2`) instead of `"Todo"` / `"InProgress"` / `"Done"` — found that by actually
posting a request, not by reading the code.

## How I checked all this

Ran the API and hit it with `curl`: no token → `401`, someone else's task → `404`, bad status
value → `400`. The working, corrected version is in [`TaskManagementApi/`](TaskManagementApi/).
