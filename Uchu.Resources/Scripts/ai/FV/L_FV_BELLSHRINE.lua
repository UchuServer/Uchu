
function onOnHit(self, msg)

    local player = msg.attacker
    local missionState = player:GetMissionState{missionID = 509}.missionState
   
    if missionState == 1 then  -- Mission has not been accepted yet
        return
    end
    
    if missionState > 7 then  -- Any mission 8 or higher is complete
        return
    end
    
    local BrickLot = -1
    local MyLot = self:GetLOT{}.objtemplate
    local PreconditionNum = -1
    
    if MyLot == 7606 then
        BrickLot = 7436
        PreconditionNum = 100
    elseif MyLot == 7620 then
        BrickLot = 7437
        PreconditionNum = 101
    elseif MyLot == 7621 then
        BrickLot = 7438
        PreconditionNum = 102
    elseif MyLot == 7622 then
        BrickLot = 7439
        PreconditionNum = 103
    end
    
    if player:CheckPrecondition{ PreconditionID = PreconditionNum,iPreconditionType = 0 }.bPass then -- player already has one, don't spawn another
        return 
    end
    
    
 
 
    if BrickLot ~= -1 then
        local newSpawner = GAMEOBJ:GenerateSpawnedID()
        self:DropLoot{owner = player, lootID = newSpawner, itemTemplate = BrickLot, rerouteID = player, sourceObj = self}
    end
end

 

