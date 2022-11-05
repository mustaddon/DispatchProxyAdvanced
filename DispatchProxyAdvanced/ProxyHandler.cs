using System.Reflection;
namespace DispatchProxyAdvanced;


public delegate object? ProxyHandler(MethodInfo method, object?[] args);