using System.Reflection;
namespace DispatchProxyAdvanced;


public delegate object? ProxyHandler(IProxy proxy, MethodInfo method, object?[] args);