--****************************************************************
-- THIS SCRIPT IS NO LONGER IN USE. KEPT ONLY FOR REFERENCE
-- PLEASE DELETE BEFORE GOLD MASTER
--******************************************************************

-- local misID = 480

-- function onRespondToMission(self, msg)
    -- --print('mission: ' .. msg.missionID)
    -- if msg.missionID ~= misID  then return end
    
    -- local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())   
    -- local factionNPCs = self:GetObjectsInGroup{ group = 'DuckShowcaser', ignoreSpawners = true }.objects
    
    
    -- -- if the player is not on the mission or not completed the mission then set the icons on the faction npc's
    -- if not player:CheckPrecondition{PreconditionID = 50}.bPass and not player:CheckPrecondition{PreconditionID = 51}.bPass then 
        -- for k,v in ipairs(factionNPCs) do 
            -- --print('fireEvent to ' .. v:GetName().name)
            -- v:FireEvent{senderID = self, args = 'showIcon'}
        -- end
    -- end
-- end
