using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Scripting.Hosting;

namespace Uchu.Python
{
    public class ManagedScript
    {
        public string Source { get; }
        
        private ScriptScope _scope;

        private readonly ManagedScriptEngine _engine;
        
        public ManagedScript(string source, ManagedScriptEngine engine)
        {
            Source = source;

            _engine = engine;
        }

        public bool Run() => Run(new KeyValuePair<string, dynamic>[0]);
        
        public bool Run(IEnumerable<KeyValuePair<string, dynamic>> variables)
        {
            var success = _engine.CompileScript(Source, out var code, out var scope);

            if (!success) return false;

            foreach (var (key, value) in variables)
            {
                scope.SetVariable(key, value);
            }
            
            try
            {
                code.Execute(scope);

                _scope = scope;

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                
                return false;
            }
        }

        public bool GetVariable<T>(string name, out T result, out Exception error)
        {
            if (_scope.GetItems().All(s => s.Key != name))
            {
                result = default;

                error = default;
                
                return false;
            }
            
            var variable = _scope.GetItems().FirstOrDefault(s => s.Key == name);

            try
            {
                result = (T) variable.Value;
                
                error = default;

                return true;
            }
            catch (Exception e)
            {
                result = default;
                
                error = e;
                
                return false;
            }
        }
        
        public bool Execute(string name, out Exception error)
        {
            if (_scope.GetItems().All(s => s.Key != name))
            {
                error = default;
                
                return false;
            }
            
            var variable = _scope.GetItems().FirstOrDefault(s => s.Key == name);

            try
            {
                variable.Value();

                error = default;

                return true;
            }
            catch (Exception e)
            {
                error = e;
                
                return false;
            }
        }

        public bool Execute<TOut>(string name, out TOut result, out Exception error)
        {
            if (_scope.GetItems().All(s => s.Key != name))
            {
                result = default;
                
                error = default;
                
                return false;
            }
            
            var variable = _scope.GetItems().FirstOrDefault(s => s.Key == name);

            try
            {
                result = (TOut) variable.Value();

                error = default;

                return true;
            }
            catch (Exception e)
            {
                result = default;
                
                error = e;
                
                return false;
            }
        }
        
        public bool Execute<TOut, TIn>(string name, out TOut result, TIn parameter, out Exception error)
        {
            if (_scope.GetItems().All(s => s.Key != name))
            {
                result = default;
                
                error = default;
                
                return false;
            }
            
            var variable = _scope.GetItems().FirstOrDefault(s => s.Key == name);

            try
            {
                result = (TOut) variable.Value(parameter);

                error = default;

                return true;
            }
            catch (Exception e)
            {
                result = default;
                
                error = e;
                
                return false;
            }
        }
    }
}