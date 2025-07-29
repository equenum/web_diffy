# web_diffy

- What if the target page or node no longer exists?
- Add a job schedule jitter?
- Import export resource / target configurations as json.
- MCP Server?
- Telegram bot?

# Todo:

- Add error codes to all error logs like in ChangeDetector.cs.
- Add input validation on UI (take the code from the existing endpoint filters).
- Persist UI user settings in the database?
- Update tests for the snapshot services and change detector to account for Outcome and Message.
- Implement notifications.
- Add new target property - status (active / paused) and new endpoints to resume and pause jobs.
- Grey out stopped jobs on the dashboard.
- Add attribution section to readme includeing all libraries and licenses.
- Outline dashboard targets what have changes detected.
- Wire up dashboard widgets.
- Make all datagrids outlined (also take a look if makes sense to outline other elements).
- Test diffs (especially added and removed lines).

# Attribution
- https://github.com/mmanela/diffplex/tree/master