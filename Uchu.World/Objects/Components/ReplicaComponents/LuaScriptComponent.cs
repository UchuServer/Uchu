using System;
using System.IO;
using System.Linq;
using System.Reflection;
using NLua;
using RakDotNet.IO;
using Uchu.Core;
using Uchu.Core.CdClient;
using Uchu.World.Collections;
using Uchu.World.Parsers;

namespace Uchu.World
{
    public class LuaScriptComponent : ReplicaComponent
    {
        public LegoDataDictionary Data { get; set; }

        private Lua _state;

        private LuaFunction[] _functions;
        
        private ScriptComponent _scriptComponent;
        
        public override ReplicaComponentsId Id => ReplicaComponentsId.Script;

        public override void FromLevelObject(LevelObject levelObject)
        {
            
        }

        public override void Construct(BitWriter writer)
        {
            var hasData = Data != null;
            
            writer.WriteBit(hasData);
            if (hasData) writer.WriteLdfCompressed(Data);
        }

        public override void Serialize(BitWriter writer)
        {
        }

        public override void Instantiated()
        {
            base.Instantiated();

            return;
            
            using (var cdClient = new CdClientContext())
            {
                var scriptComponentRegistryEntry = cdClient.ComponentsRegistryTable.FirstOrDefault(
                    r => r.Id == GameObject.Lot && r.Componenttype == (int) Id
                );
                
                if (scriptComponentRegistryEntry == default) return; // Script has been removed...

                _scriptComponent = cdClient.ScriptComponentTable.FirstOrDefault(
                    s => s.Id == scriptComponentRegistryEntry.Componentid
                );

                if (_scriptComponent == default)
                {
                    Logger.Error(
                        $"{GameObject.Lot} has an invalid script component entry: {scriptComponentRegistryEntry.Componentid}"
                    );
                    
                    return;
                }
            }

            var assembly = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);

            var scriptName = Path.GetFileName(_scriptComponent.Scriptname?.ToLower().Replace('\\', '/'));
            
            var scriptSource = Path.Combine(
                $"{assembly}/scripts/{scriptName}"
            ).Replace('\\', '/');

            if (_scriptComponent.Scriptname == default || assembly == default || !File.Exists(scriptSource))
            {
                if (_scriptComponent.Scriptname != default)
                    Logger.Debug($"{GameObject.Lot} has an invalid or removed script registered on it: {assembly}/scripts/{scriptName}");
                return;
            }
            
            _state = new Lua();
            
            //
            // TODO: Somehow load modules
            //

            object[] luaFeatures;
            try
            {
                luaFeatures = _state.DoFile(scriptSource);
            }
            catch (Exception e)
            {
                Logger.Error($"{_scriptComponent.Scriptname?.ToLower()} throw error:\n{e}");
                return;
            }

            if (luaFeatures != default)
            {
                _functions = luaFeatures.OfType<LuaFunction>().ToArray();

                Logger.Debug($"Equipped {scriptName} to {GameObject} [{GameObject.Lot}]");
            }
            else Logger.Debug($"No features on {scriptName} [{GameObject.Lot}]");
        }
    }
}