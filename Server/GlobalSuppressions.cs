using System.Diagnostics.CodeAnalysis;

// Finally settled on not calling it anywhere since this isn't a library. I am going with defaults
[assembly: SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task", Justification = "<Pending>")]

// These need to be string for they are part of a bigger uri
[assembly: SuppressMessage("Design", "CA1056:Uri properties should not be strings", Justification = "<Pending>", Scope = "namespaceanddescendants", Target = "~N:CRM.Server.Areas.Identity.Pages")]
[assembly: SuppressMessage("Design", "CA1054:Uri parameters should not be strings", Justification = "<Pending>", Scope = "namespaceanddescendants", Target = "~N:CRM.Server.Areas.Identity.Pages")]

// Can't mark the following as static
[assembly: SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>", Scope = "member", Target = "~M:CRM.Server.Startup.Configure(Microsoft.AspNetCore.Builder.IApplicationBuilder,Microsoft.AspNetCore.Hosting.IWebHostEnvironment)")]
[assembly: SuppressMessage("Design", "CA1052:Static holder types should be Static or NotInheritable", Justification = "<Pending>", Scope = "type", Target = "~T:CRM.Server.Program")]

// TODO: Remove this suppression and add localized parameters
[assembly: SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>", Scope = "namespaceanddescendants", Target = "~N:CRM.Server")]

// Don't know what type of exceptions would be thrown
[assembly: SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>", Scope = "member", Target = "~M:CRM.Server.Program.Main(System.String[])")]