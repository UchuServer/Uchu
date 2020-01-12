using System;
using System.Collections.Generic;
using Microsoft.Scripting.Hosting;

namespace Uchu.Python
{
    public class ManagedScriptEngine
    {
        public ScriptEngine Engine { get; private set; }

        public Dictionary<string, dynamic> Standard { get; } = new Dictionary<string, dynamic>();

        public void Init()
        {
            if (Engine != default) return;
            
            Engine = IronPython.Hosting.Python.CreateEngine();
        }

        public bool CompileScript(string script, out CompiledCode code, out ScriptScope scope)
        {
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