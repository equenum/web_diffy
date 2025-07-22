# webpage_change_monitor

- What if the target page or node no longer exists?
- Add a job schedule jitter?
- Notifications: Telegram, email?
- Import export resource / target configurations as json.

# Todo:

- Add error codes to all error logs like in ChangeDetector.cs.
- Add input validation on UI (take the code from the existing endpoint filters).
- Restart jobs if target updates? Automagically or manually via a button?
- Add a button to manually pause jobs?
- Enable attribute based endpoint validation when dotnet 10 comes out.
- Persist UI user settings in the database?
- Update tests for the snapshot services and change detector to account for Outcome and Message.
- Implement notifications.
