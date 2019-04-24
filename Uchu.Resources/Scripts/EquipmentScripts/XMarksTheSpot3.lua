--------------------------------------------------------------
-- X-Marks-The-Spot Rank 3
-- Server script spawns several powerup objects
-- Client script animates a chest getting dug up and opening to spawn the items.
--------------------------------------------------------------

require('o_mis')
local minLife = 5
local maxLife = 8
local minArmor = 0
local maxArmor = 4
local minImagination = 0
local maxImagination = 4

  

function onStartup(self) 
    GAMEOBJ:GetTimer():AddTimerWithCancel( 2.7 , "SpawnGoodies", self )
    
    --parent = self:GetParentObj{}.objIDParent
    storeObjectByName(self, "parent", self:GetParentObj{}.objIDParent)
end

onTimerDone = function(self, msg)    
    if msg.name == "SpawnGoodies" then
        SpawnGoodies(self)     
        GAMEOBJ:GetTimer():AddTimerWithCancel( 1.5 , "Die", self )
    end
    
    if msg.name == "Die" then
        self:Die{ killType = "SILENT" }
    end
end

function SpawnGoodies(self) 
        SpawnPowerups(self, minLife, maxLife, 177)
        
        SpawnPowerups(self, minArmor, maxArmor, 6431)
   
        SpawnPowerups(self, minImagination, maxImagination, 935)

end -- func SpawnGoodies

function SpawnPowerups(self, min, max, type)
        local i = 1
        local randomNum = math.random(min,max) 
        
        while (i <= randomNum) do
            local newID = GAMEOBJ:GenerateSpawnedID()
            self:DropLoot{itemTemplate = type,owner = getObjectByName(self, "parent"), lootID = newID, rerouteID = getObjectByName(self, "parent"), sourceObj = self} 
            i = i + 1
        end
end 
