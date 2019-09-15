using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RakDotNet.IO;
using Uchu.Core;
using Uchu.Core.CdClient;

namespace Uchu.World.Behaviors
{
    public abstract class Behavior
    {
        private static Dictionary<BehaviorTemplateId, Type> _behaviors;

        public int BehaviorId;
        public BehaviorExecutioner Executioner;

        public SkillComponent SkillComponent;

        public GameObject GameObject => SkillComponent.GameObject;

        public List<Behavior> Branches { get; set; } = new List<Behavior>();

        public Dictionary<uint, Behavior> HandledBehaviors => SkillComponent.HandledBehaviors;

        public static Dictionary<BehaviorTemplateId, Type> Behaviors
        {
            get
            {
                if (_behaviors != default) return _behaviors;

                _behaviors = new Dictionary<BehaviorTemplateId, Type>();

                var behaviors = typeof(Behavior).Assembly.GetTypes().Where(
                    t => t.BaseType == typeof(Behavior) && t != typeof(Behavior)
                ).ToArray();

                foreach (var behavior in behaviors)
                    _behaviors.Add(((Behavior) Activator.CreateInstance(behavior)).Id, behavior);

                return _behaviors;
            }
        }

        public abstract BehaviorTemplateId Id { get; }

        public abstract Task Serialize(BitReader reader);

        public async Task StartBranch(int behaviorId, BitReader reader)
        {
            var templateId = (await GetTemplate(behaviorId)).TemplateID;
            if (templateId != null)
            {
                var id = (BehaviorTemplateId) templateId;

                Logger.Debug($"Starting Behavior Branch {id}");

                var instance = (Behavior) Activator.CreateInstance(_behaviors[id]);

                instance.Executioner = Executioner;
                instance.BehaviorId = behaviorId;
                instance.SkillComponent = SkillComponent;

                await instance.Serialize(reader);

                Branches.Add(instance);
            }
        }

        public static async Task<BehaviorParameter> GetParameter(int behaviourId, string name)
        {
            using (var cdClient = new CdClientContext())
            {
                return await cdClient.BehaviorParameterTable.FirstOrDefaultAsync(p =>
                    p.BehaviorID == behaviourId && p.ParameterID == name
                );
            }
        }

        public static BehaviorParameter[] GetParameters(int behaviourId)
        {
            using (var cdClient = new CdClientContext())
            {
                return cdClient.BehaviorParameterTable.Where(p =>
                    p.BehaviorID == behaviourId
                ).ToArray();
            }
        }

        public static async Task<BehaviorTemplate> GetTemplate(int behaviourId)
        {
            using (var cdClient = new CdClientContext())
            {
                return await cdClient.BehaviorTemplateTable.FirstOrDefaultAsync(p =>
                    p.BehaviorID == behaviourId
                );
            }
        }
    }
}