require('o_mis')
-- Spawns powerups on the object at 3 seconds after startup and every 10 seconds thereafter. Object smashes when it runs out of powerups
CONSTANTS = {}
CONSTANTS["NO_OBJECT"] = "0"
function onTemplateStartup(self) 
    self:SetVar("parentID", self:GetParentObj{}.objIDParent:GetID())
    self:SetVar( "currentCycle", 1)
    GAMEOBJ:GetTimer():AddTimerWithCancel( self:GetVar("delayToFirstCycle") , "TimeToSpawn", self )
end

function onTimerDone(self, msg) 
    if msg.name == "TimeToSpawn" then
        SpawnPowerups(self, self:GetVar("numberOfPowerups"), self:GetVar("LootLOT"))
        if (self:GetVar("currentCycle") < self:GetVar("numCycles")) then
            GAMEOBJ:GetTimer():AddTimerWithCancel( self:GetVar( "secPerCycle" ) , "TimeToSpawn", self )
            self:SetVar( "currentCycle",  self:GetVar("currentCycle") + 1)
        end
        if (self:GetVar("currentCycle") >= self:GetVar("numCycles")) then
            GAMEOBJ:GetTimer():AddTimerWithCancel( self:GetVar( "deathDelay" ) , "Die", self )
        end
    end
        
    if msg.name == "Die" then
        self:RequestDie{}
    end
end

function SpawnPowerups(self, numPowerups, type)
    for i = 1, numPowerups do
        local newID = GAMEOBJ:GenerateSpawnedID()
        local parentID = GAMEOBJ:GetObjectByID(self:GetVar("parentID"))
        DoObjectAction(self, "effect", "cast")
    	self:DropItems{owner = parentID, itemTemplate = type, iAmount = 1, bUseTeam = true, bFreeForAll = true}
	end
end 










