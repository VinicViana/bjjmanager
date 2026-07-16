# Exercise 2 — Generative AI Tools: Prompt Engineering & AI Code Review

This exercise is not about shipping a production system — it's about demonstrating the
**process**: how I prompt a GenAI coding tool for a backend scaffold, what it hands back, and how
I critically review and correct that output before it could ever be trusted in a real codebase.

Tool used: **Claude Code** (Anthropic), same tool used throughout this repository.

## The scenario

> Generate a RESTful API for a simple task management system. CRUD for tasks. Each task has a
> `title`, `description`, `status`, and `due_date`. Tasks are associated with a user (assume a
> basic `User` model exists).

## Read in this order

1. [`prompt.md`](prompt.md) — the prompt I actually used, and why I wrote it the way I did.
2. [`code-review.md`](code-review.md) — the raw generated output, how I evaluated it, and every
   change I made to get it to something I'd actually merge.
3. [`TaskManagementApi/`](TaskManagementApi/) — the corrected version as a working project (see
   below for how to run it).

