using System;
using System.Linq;
using Microsoft.Scripting.Hosting;

namespace Uchu.Python
{
    public class ManagedScript
    {
        public string Source { get; }
        
        private ScriptScope _scope;

        private ManagedScriptEngine _engine;
        
        public ManagedScript(string source, ManagedScriptEngine engine)
        {
            Source = source;

            _engine = engine;
        }

        public bool Run()
        {
            var success = _engine.CompileScript(Source, out var code, out var scope);

            if (!success) return false;

            foreach (var (key, value) in _engine.Standard)
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

        public bool GetVariable<T>(string name, out T result)
        {
            if (_scope.GetItems().All(s => s.Key != name))
            {
                result = default;
                
                return false;
            }
            
            var variable = _scope.GetItems().FirstOrDefault(s => s.Key == name);

            try
            {
                result = (T) variable.Value;

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                
                result = default;

                return false;
            }
        }
        
        public bool Execute(string name)
        {
            if (_scope.GetItems().All(s => s.Key != name))
            {
                return false;
            }
            
            var variable = _scope.GetItems().FirstOrDefault(s => s.Key == name);

            try
            {
                variable.Value();

                return true;
            }
            catch 
            {
                return false;
            }
        }

        public bool Execute<TOut>(string name, out TOut result)
        {
            if (_scope.GetItems().All(s => s.Key != name))
            {
                result = default;
                
                return false;
            }
            
            var variable = _scope.GetItems().FirstOrDefault(s => s.Key == name);

            try
            {
                result = (TOut) variable.Value();

                return true;
            }
            catch
            {
                result = default;
                
                return false;
            }
        }
        
        public bool Execute<TOut, TIn>(string name, out TOut result, TIn parameter)
        {
            if (_scope.GetItems().All(s => s.Key != name))
            {
                result = default;
                
                return false;
            }
            
            var variable = _scope.GetItems().FirstOrDefault(s => s.Key == name);

            try
            {
                result = (TOut) variable.Value(parameter);

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                
                result = default;
                
                return false;
            }
        }
    }
}