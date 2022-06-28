using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Test")]
// Moq requires this to work with internals
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]