using System;
using System.Collections.Generic;
using System.Linq;
using RakDotNet.IO;

namespace Uchu.World
{
    public class BuffInfo
    {
        public uint BuffId;
        public uint DurationSecs;
        public bool CancelOnUnequip;
        public bool CancelOnRemoveBuff;
        public bool CancelOnUi;
        public bool CancelOnZone;
        public bool CancelOnDamaged;
        public bool CancelOnDeath;
        public bool CancelOnLogout;
        public bool ApplyOnTeammates;
        public bool UseRefCount;
    }

    public class BuffComponent : ReplicaComponent
    {
        public override ComponentId Id => ComponentId.BuffComponent;

        private readonly List<BuffInfo> _buffs = new List<BuffInfo>();

        protected BuffComponent()
        {

        }

        public void AddBuff(BuffInfo buffInfo)
        {
            _buffs.Add(buffInfo);

            if (!GameObject.TryGetComponent<DestroyableComponent>(out var destroyable)) return;
            if (buffInfo.CancelOnDeath)
            {
                Delegate onDeathDelegate = null;
                onDeathDelegate = Listen(destroyable.OnDeath, () =>
                {
                    ReleaseListener(onDeathDelegate);
                    RemoveBuff(buffInfo);
                });
            }

            if (buffInfo.CancelOnDamaged)
            {
                Delegate cancelOnDamaged = null;
                cancelOnDamaged = Listen(destroyable.OnAttacked, () => 
                {
                    ReleaseListener(cancelOnDamaged);
                    RemoveBuff(buffInfo);
                });
            }

            if (buffInfo.DurationSecs != 0)
                GameObject.Zone.Schedule(() =>
                {
                    RemoveBuff(buffInfo);
                }, buffInfo.DurationSecs * 1000);

            // TODO: handle other cancel events
            // if (buffInfo.CancelOnZone) ;
            // if (buffInfo.CancelOnUi) ;
            // if (buffInfo.CancelOnLogout) ;
            if (GameObject is Player player){
                player.Message(new AddBuffMessage {
                    AddedByTeammate = false, //we don't know the source yet, how will this affect gameplay?
                    ApplyOnTeammates = buffInfo.ApplyOnTeammates,
                    CancelOnDamageAbsorbRanOut = false, //no var
                    CancelOnDamaged = buffInfo.CancelOnDamaged,
                    CancelOnDeath = buffInfo.CancelOnDeath,
                    CancelOnLogout = buffInfo.CancelOnLogout,
                    CancelOnMove = false, //no var
                    CancelOnRemoveBuff = buffInfo.CancelOnRemoveBuff,
                    CancelOnUi = buffInfo.CancelOnUi,
                    CancelOnUnequip = buffInfo.CancelOnUnequip,
                    CancelOnZone = buffInfo.CancelOnZone,
                    IgnoreImmunities = false, //no var
                    IsImmunity = false, //no var
                    UseRefCount = buffInfo.UseRefCount,
                    Caster = GameObject, //still don't know source
                    AddedBy = GameObject, //i have zero clue what this is
                    BuffID = buffInfo.BuffId,
                    DurationMS = buffInfo.DurationSecs * 1000,
                });
            }
        }

        public bool HasBuff(uint buffId)
        {
            return _buffs.Exists(buffInfo => buffInfo.BuffId == buffId);
        }

        public void RemoveBuff(BuffInfo buffInfo)
        {
            if (GameObject is Player player){
                player.Message(new RemoveBuffMessage {
                    FromRemoveBehavior = false, //these three need to be investigated
                    FromUnequip = false,
                    RemoveImmunity = false,
                    BuffID = buffInfo.BuffId
                });
            }
            _buffs.Remove(buffInfo);
        }

        public void RemoveBuffById(uint buffId)
        {
            if (GameObject is Player player){
                player.Message(new RemoveBuffMessage {
                    FromRemoveBehavior = false,
                    FromUnequip = false,
                    RemoveImmunity = false,
                    BuffID = buffId
                });
            }
            _buffs.Remove(_buffs.FirstOrDefault(buffInfo => buffInfo.BuffId == buffId));
        }

        public override void Construct(BitWriter writer)
        {

        }

        public override void Serialize(BitWriter writer)
        {

        }
    }
}
