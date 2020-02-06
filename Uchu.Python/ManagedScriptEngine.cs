using System;
using System.Linq;
using Microsoft.Scripting.Hosting;

namespace Uchu.Python
{
    public class ManagedScriptEngine
    {
        public ScriptEngine Engine { get; private set; }
        
        public static string[] AdditionalPaths { get; set; }

        public void Init()
        {
            if (Engine != default) return;
            
            Engine = IronPython.Hosting.Python.CreateEngine();

            var paths = Engine.GetSearchPaths().ToList();

            paths.AddRange(AdditionalPaths);

            Engine.SetSearchPaths(paths);
        }

        public bool CompileScript(string script, out CompiledCode code, out ScriptScope scope)
        {
            Init();
            
            try
            {
                var source = Engine.CreateScriptSourceFromString(script);

                scope = Engine.CreateScope();

                code = source.Compile();

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                
                scope = default;
                
                code = default;
                
                return false;
            }
        }
    }
}