using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Numerics;

namespace Uchu.Core.Scripting.Lua
{
    public class LuaGameObject
    {
        public Dictionary<string, dynamic> Variables { get; }
        public ISpawnable Object { get; }

        public LuaGameObject(ISpawnable obj)
        {
            Variables = new Dictionary<string, dynamic>();
            Object = obj;
        }

        // :SetVar(string, *) : void
        public void SetVar(string name, dynamic value)
            => Variables[name] = value;

        // :GetVar(string) : *
        public dynamic GetVar(string name)
            => Variables[name];

        // :GetID() : string
        public string GetID()
            => Object.ObjectId.ToString();

        // :IsCharacter() : { isChar = bool }
        public dynamic IsCharacter()
        {
            dynamic res = new ExpandoObject();

            res.isChar = Object is PlayerObject;

            return res;
        }

        // :DropLoot { itemTemplate = int, owner = object, lootID = string, rerouteID = object, sourceObj = object } : void
        public void DropLoot(dynamic obj)
        {
            int lot = obj.itemTemplate;
            LuaGameObject owner = obj.owner;
            string lootId = obj.lootID;
            LuaGameObject reroute = obj.rerouteID;
            LuaGameObject source = obj.sourceObj;

            if (source.Object is WorldObject worldObj && owner.Object is PlayerObject player)
                worldObj.DropLoot(lot, player);

            Console.WriteLine($"<{GetID()}>:DropLoot {{ itemTemplate = {lot}, owner = <{owner.GetID()}>, lootID = '{lootId}', rerouteID = <{reroute.GetID()}>, sourceObj = <{source.GetID()}> }}");
        }

        // :RequestDie { killerID = object, deathType = string } : void
        public void RequestDie(dynamic obj)
        {
            LuaGameObject killer = obj.killerID;
            string type = obj.deathType;

            Console.WriteLine($"<{GetID()}>:RequestDie {{ killerID = <{killer.GetID()}>, deathType = '{type}' }}");
        }

        // :PlayAnimation { animationID = string, bPlayImmediate = bool } : void
        public void PlayAnimation(dynamic obj)
        {
            string animation = obj.animationID;
            bool? playImmediately = obj.bPlayImmediate;

            Console.WriteLine($"<{GetID()}>:PlayAnimation {{ animationID = '{animation}', bPlayImmediate = {playImmediately} }}");
        }

        // :Knockback { vector = { x = int, y = int, z = int } } : void
        public void Knockback(dynamic obj)
        {
            var vec = new Vector3
            {
                X = obj.vector.x,
                Y = obj.vector.y,
                Z = obj.vector.z
            };

            Console.WriteLine($"<{GetID()}>:Knockback {{ vector = {{ x = {vec.X}, y = {vec.Y}, z = {vec.Z} }} }}");
        }

        // :PlayFXEffect { name = string, effectType = string, effectID = int } : void
        public void PlayFXEffect(dynamic obj)
        {
            string name = obj.name;
            string type = obj.effectType;
            int id = obj.effectID;

            Console.WriteLine($"<{GetID()}>:PlayFXEffect {{ name = '{name}', effectType = '{type}', effectID = {id} }}");
        }

        // :StopFXEffect { name = string } : void
        public void StopFXEffect(dynamic obj)
        {
            string name = obj.name;

            Console.WriteLine($"<{GetID()}>:StopFXEffect {{ name = '{name}' }}");
        }

        // :UpdateMissionTask { target = object, value = int, value2 = int, taskType = string } : void
        public void UpdateMissionTask(dynamic obj)
        {
            LuaGameObject target = obj.target;
            int? mission = obj.mission;
            int? param = obj.value2;
            string type = obj.taskType;

            Console.WriteLine($"<{GetID()}>:UpdateMissionTask {{ target = <{target.GetID()}>, value = {mission}, value2 = {param}, taskType = '{type}' }}");

            if (target.Object is PlayerObject player)
            {
            }
        }
    }
}