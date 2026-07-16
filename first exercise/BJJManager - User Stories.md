# BJJManager — User Stories

## 1. Login & Authentication
**As a** jiu-jitsu practitioner,
**I want** to create an account and log in securely,
**so that** only I can access my trainings and techniques.

- Sign up with name and password
- Login with JWT
- No access to any screen without being authenticated

---

## 2. Trainings (Training Log)
**As a** practitioner,
**I want** to log my trainings with a date and a self-evaluation score (1 to 5),
**so that** I can track my progress over time.

- Full CRUD for trainings
- Filter by exact date, month, or year
- Attach a photo/video of the training

---

## 3. Techniques (Technique Library)
**As a** practitioner,
**I want** to catalog the techniques I learn, with step-by-step instructions and position,
**so that** I have a personal reference to check later.

- Full CRUD for techniques
- Ordered step list
- Position field (dropdown or free text)
- Attach a photo/video of the technique

---

## 4. Dashboard (Totals & Chart)
**As a** practitioner,
**I want** to see an overall summary of my trainings and techniques,
**so that** I can quickly understand my progress without digging through lists.

- Totals (trainings, techniques, etc.)
- Line chart with the daily average self-evaluation score over the last 30 days

---

## 5. AI Coach (Chat)
**As a** practitioner,
**I want** to ask jiu-jitsu questions to an AI assistant right in the app,
**so that** I get quick help without leaving the app.

- Floating chat widget
- Replies in the same language as the question
- Nothing about the conversation is saved
