namespace Uchu.World
{
    public enum ReplicaComponentsId
    {
        Invalid, // For components like stats which are not used by itself.
        
        //
        // Server only
        //
        Spawner = 10,
        QuestGiver = 73,
        Item = 11,
        
        Possesable = 108,
        ModuleAssembly = 61,
        ControllablePhysics = 1,
        SimplePhysics = 3,
        RigidBodyPhantomPhysics = 20,
        VehiclePhysics = 30,
        PhantomPhysics = 40,
        Destructible = 7, // Destructible, Stats
        Collectible = 23, // Stats, Collectible
        Pet = 26,
        Character = 4, // Character (Part 1-4)
        ShootingGallery = 19,
        Inventory = 17,
        Script = 5,
        Skill = 9,
        BaseCombatAi = 60,
        Rebuild = 48, //Stats, Rebuild
        MovingPlatform = 25,
        Switch = 49,
        Vendor = 16,
        Bouncer = 6,
        ScriptedActivity = 39,
        RacingControl = 71,
        Exhibit = 75,
        Model = 42,
        Render = 2,
        Component107 = 107,
        Trigger = 69
    }
}