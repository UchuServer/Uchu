-----------------------------------------------------------
-- This script is on the faction rep
-- after selecting a faction, the player does a celeb and given
-- the correct faction flags and achievements
-- 
-- updated mrb... 3/9/11 - changed to db celebration logic
----------------------------------------------------------------
local misID = 474	-- Join a faction mission
  
function onRespondToMission(self, msg)   
    if msg.missionID ~= misID then return end
    
    local player = msg.playerID
    local celebrationID = -1
    
    -- if the player is not on the mission or not completed the mission then set the icons on the faction npc's
    if msg.rewardItem and msg.rewardItem ~= -1 then
        local tUpdateMissions = {}

        -- set the players new Faction and sets the corrisponding celebration
        if msg.rewardItem == 6980 then      -- Venture <-> 7090 ************************************************
            player:SetPlayerKitFaction{iFactionNum = 1}
            tUpdateMissions = { 555, 556, 778 }   -- setup the Venture Achievements that need to be completed
			celebrationID = 14
        elseif msg.rewardItem == 6979 then  -- Assembly <-> 7091 ***********************************************
            player:SetPlayerKitFaction{iFactionNum = 2}
            tUpdateMissions = { 544, 545, 778 }   -- setup the Assembly Achievements that need to be completed
			celebrationID = 15
        elseif msg.rewardItem == 6981 then  -- Paradox <-> 7092 ************************************************
            player:SetPlayerKitFaction{iFactionNum = 3}
            tUpdateMissions = { 577, 578, 778 }   -- setup the Paradox Achievements that need to be completed
			celebrationID = 16
        elseif msg.rewardItem == 6978 then  -- Sentinel <-> 7093 ***********************************************
            player:SetPlayerKitFaction{iFactionNum = 4}
            tUpdateMissions = { 566, 567, 778 }   -- setup the Sentinel Achievements that need to be completed
			celebrationID = 17
        end
		
		-- play the celebration
		if celebrationID ~= -1 then
			player:StartCelebrationEffect{rerouteID = player, celebrationID = celebrationID}
		end
        
        -- add and complete the achievements
        for k,v in ipairs(tUpdateMissions) do    
			 player:AddMission{missionID = v}
			 player:CompleteMission{missionID = v}
        end        
    end
end 