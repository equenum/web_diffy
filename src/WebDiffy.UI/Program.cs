using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MudBlazor.Services;
using WebDiffy.UI.Components;
using WebDiffy.UI.Infrastructure.Options;
using WebDiffy.UI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMudServices();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpClient();
builder.Services.AddTransient<IResourceService, ResourceService>();
builder.Services.AddTransient<ITargetService, TargetService>();
builder.Services.AddTransient<ITargetSnapshotService, TargetSnapshotService>();

builder.Services.Configure<ApiOptions>(
    builder.Configuration.GetSection(ApiOptions.Name));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
