# webpage_change_monitor

- What if the target page or node no longer exists?
- Add a job schedule jitter?
- Notifications: Telegram, email?
- Import export resource / target configurations as json.

# Todo:

- Add error codes to all error logs like in ChangeDetector.cs.
- Add the theme manager (https://github.com/MudBlazor/ThemeManager)?
- Add input validation on UI (take the code from the existing endpoint filters).
- Restart jobs if target updates? Automagically or manually via a button?
- Add a button to manually pause jobs?
- Add a settings to auto expand all resource target panels on loading? Same for the dashboard page.
- Make sure validation works after migrating to minimal api. Probably need to wire up validation services.
- Add snapshot error handling and error state and message.
- Add sorting to all the endpoints.
- Enable attribute based endpoint validation when dotnet 10 comes out.
- Add sorting criteria with sortBy.
