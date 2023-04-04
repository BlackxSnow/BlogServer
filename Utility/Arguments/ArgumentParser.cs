using System.Reflection;

namespace Utility.Arguments;

public static class ArgumentParser
{
    private struct ArgumentDelegate
    {
        public readonly ArgumentAttribute Attribute;
        public readonly MethodInfo Method;

        public ArgumentDelegate(ArgumentAttribute attribute, MethodInfo method)
        {
            Attribute = attribute;
            Method = method;
        }
    }

    private static Dictionary<Type, Dictionary<string, ArgumentDelegate>> _ArgumentDelegatesByContext = new();
    // private static Dictionary<string, ArgumentDelegate> _ArgumentDelegates = new();

    static ArgumentParser()
    {
        GetArgumentDelegates();
    }

    private static void ParseMethodAttributes(MethodInfo method)
    {
        IEnumerable<ArgumentDelegate> attributes = method.GetCustomAttributes<ArgumentAttribute>()
            .Select(a => new ArgumentDelegate(a, method));
        foreach (ArgumentDelegate argumentDelegate in attributes)
        {
            // _ArgumentDelegates.Add(argumentDelegate.Attribute.Id, argumentDelegate);
            // if (argumentDelegate.Attribute.ContextType == null) continue;
                    
            if (!_ArgumentDelegatesByContext.TryGetValue(argumentDelegate.Attribute.ContextType,
                    out Dictionary<string, ArgumentDelegate>? argDictionary))
            {
                argDictionary = new Dictionary<string, ArgumentDelegate>();
                _ArgumentDelegatesByContext.Add(argumentDelegate.Attribute.ContextType, argDictionary);
            }
            argDictionary.Add(argumentDelegate.Attribute.Id, argumentDelegate);
        }
    }
    
    private static void GetArgumentDelegates()
    {
        // Get assemblies that depend on this one
        string currentAssembly = Assembly.GetExecutingAssembly().FullName!;
        IEnumerable<Assembly> assemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => a.GetReferencedAssemblies().Any(r => r.FullName == currentAssembly));
        
        foreach (Assembly assembly in assemblies)
        {
            // Get methods with ArgumentAttributes in assemblies
            IEnumerable<MethodInfo> methods = assembly.GetTypes()
                .SelectMany(t => t.GetMethods(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public))
                .Where(m => m.GetCustomAttributes<ArgumentAttribute>().Any());
            
            // Add an entry for each ArgumentAttribute found on the methods
            foreach (MethodInfo method in methods)
            {
                ParseMethodAttributes(method);
            }
        }
    }
    
    // private static Dictionary<string, ArgumentDelegate> GetArgumentDelegates()
    // {
    //     Dictionary<string, ArgumentDelegate> delegates = new();
    //     
    //     // Get assemblies that depend on this one
    //     string currentAssembly = Assembly.GetExecutingAssembly().FullName!;
    //     IEnumerable<Assembly> assemblies = AppDomain.CurrentDomain.GetAssemblies()
    //         .Where(a => a.GetReferencedAssemblies().Any(r => r.FullName == currentAssembly));
    //     
    //     foreach (Assembly assembly in assemblies)
    //     {
    //         // Get methods with ArgumentAttributes in assemblies
    //         IEnumerable<MethodInfo> methods = assembly.GetTypes()
    //             .SelectMany(t => t.GetMethods(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public))
    //             .Where(m => m.GetCustomAttributes<ArgumentAttribute>().Any());
    //         
    //         // Add an entry for each ArgumentAttribute found on the methods
    //         foreach (MethodInfo method in methods)
    //         {
    //             method.GetCustomAttributes<ArgumentAttribute>().Select(a => new ArgumentDelegate(a, method))
    //                 .AddToDictionary(delegates, del => del.Attribute.Id);
    //         }
    //     }
    //
    //     return delegates;
    // }

    private static object[] BuildArgumentArray<TContext>(MethodInfo method, string[] args, int currentIndex, TContext? context)
    {
        ParameterInfo[] parameters = method.GetParameters();

        int contextOffset = context != null && parameters[0].ParameterType == typeof(TContext) ? 1 : 0;
            
        int remainingArgs = args.Length - currentIndex;
        if (parameters.Length - contextOffset > remainingArgs)
        {
            throw new TargetParameterCountException($"Argument \"{args[currentIndex]}\" expected {parameters.Length} values but got {remainingArgs}.");
        }
        
        var methodArguments = new object[parameters.Length];
        if (contextOffset == 1) methodArguments[0] = context!;

        // Attempt to cast argument strings to delegate parameter types
        for (int j = contextOffset; j < parameters.Length; j++)
        {
            string arg = args[currentIndex + j + 1 - contextOffset];
            methodArguments[j] = Convert.ChangeType(arg, parameters[j].ParameterType);
        }

        return methodArguments;
    }

    public static void Parse(string[] args) => Parse<object>(args, null);
    public static void Parse<TContext>(string[] args, TContext? context)
    {
        for (var i = 0; i < args.Length; i++)
        {
            string currentArg = args[i];
            if (!currentArg.StartsWith("--"))
            {
                Console.WriteLine($"Unexpected value \"{currentArg}\"");
                continue;
            }

            currentArg = currentArg.Remove(0, 2);
            if (!_ArgumentDelegatesByContext.TryGetValue(typeof(TContext), out Dictionary<string, ArgumentDelegate>? contextDictionary)
                || !contextDictionary.TryGetValue(currentArg, out ArgumentDelegate currentDelegate))
            {
                Console.WriteLine($"Unknown argument \"{currentArg}\" in context \"{typeof(TContext)}\"");
                continue;
            }

            try
            {
                object[] methodArguments = BuildArgumentArray<TContext>(currentDelegate.Method, args, i, context);
                currentDelegate.Method.Invoke(null, methodArguments);
                i += methodArguments.Length;
            }
            catch (Exception e)
            {
                Console.WriteLine($"error: {e}");
            }
            
        }
    }
}