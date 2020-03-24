using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task", Justification = "<Pending>")]

// Database migrations
[assembly: SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "<Pending>", Scope = "namespaceanddescendants", Target = "CRM.Server.Migrations")]
[assembly: SuppressMessage("Style", "IDE0053:Use expression body for lambda expressions", Justification = "<Pending>", Scope = "namespaceanddescendants", Target = "CRM.Server.Migrations")]