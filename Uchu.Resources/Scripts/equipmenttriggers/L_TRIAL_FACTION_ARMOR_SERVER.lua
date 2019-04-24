--------------------------------------------------------------

-- L_TRIAL_FACTION_ARMOR_SERVER.lua

-- Runs server side faction trial armor equipment functions
-- created abeechler - 6/27/11
--------------------------------------------------------------

local equipFlag = 126    -- To be set upon wearing trial faction armor for the first time in a session

----------------------------------------------------------------
-- Capture initial equip events for processing
----------------------------------------------------------------
function onFactionTriggerItemEquipped(self, msg)
    -- Player attempting equip
    local player = msg.playerID
    -- Determine via flag info if this is a session first time equip
    local bEquipped = player:GetFlag{iFlagID = equipFlag}.bFlag
    
    if(not bEquipped) then
        local owner = self:GetItemOwner().ownerID
        -- This is the first time in a session we are attempting to equip trial armor
        if(player:GetID() == owner:GetID()) then
            -- Set our flag and heal our stats
            player:SetFlag{iFlagID = equipFlag, bFlag = true}
            
            -- Save a hook to the equipping player
            self:SetVar("player", player)
            
            -- Prepare for stat refill
            GAMEOBJ:GetTimer():AddTimerWithCancel(1, "FillStats", self)
            
        end
    end
    
end

----------------------------------------------------------------
-- Called when timers are done
----------------------------------------------------------------
function onTimerDone(self,msg)
    
    -- This is our first time donning the armor, apply all necessary stat refills
	if msg.name == "FillStats" then
	    -- Obtain a hook to the equipping player
	    local player = self:GetVar("player")
	    
	    -- Fill the health to capacity
        local maxHealth = player:GetMaxHealth().health
        player:SetHealth{health = maxHealth}
        -- Fill the armor to capacity
        local maxArmor = player:GetMaxArmor().armor
        player:SetArmor{armor = maxArmor}
         -- Fill the imagination to capacity
        local maxImagination = player:GetMaxImagination().imagination
        player:SetImagination{imagination = maxImagination}
        
	end

end
