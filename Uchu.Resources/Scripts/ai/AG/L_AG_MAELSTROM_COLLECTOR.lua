require('State')
require('o_StateCreate')
require('o_mis')
require('o_Main')

CONSTANTS = {}
CONSTANTS["RebuildStateOpen"] = 0
CONSTANTS["RebuildStateCompleted"] = 2
CONSTANTS["EnemySmashableFaction"] = 16


function onStartup(self) 
    self:SetProximityRadius { radius = 0, name = "spiderProx" }
    self:SetVar("built", 0)
    self:SetVar("TickTime", 1)
end


--------------------------------------------------------------------------------
-- onRebuildNotifyState
-- 
-- Notes: Whenever the rebuild state changes on the robotanist, this is called.
--------------------------------------------------------------------------------
function onRebuildNotifyState(self, msg)
    
	if (msg.iState == CONSTANTS["RebuildStateCompleted"]) then
	
	  print("*******faction change ENABLED*******")
	  
	     -- Set to Darkling hated smashable faction
	    self:SetFaction{ faction = CONSTANTS["EnemySmashableFaction"] }
	    self:SetProximityRadius { radius = 60 }
	    --self:SetVar("TickTime", 1)
       -- GAMEOBJ:GetTimer():AddTimerWithCancel( self:GetVar("TickTime")  , "TickTime", self )
        
        self:SetVar("built", 1)
    
	end
	
	if (msg.iState == CONSTANTS["RebuildStateOpen"]) then
			
	end
	
end


-- Checks to see if the collector casts its stun and updates the mission if it did.
-- This is a temporary solution and only works for 1 player.
function onCastSkill(self, msg)
    local player = getObjectByName(self, "playerID")
    if player ~= nil then
        player:UpdateMissionTask{target = self, value = 1, value2 = 1, taskType = "kill"}
        local SurvivalMissionState = player:GetMissionState{ missionID = 335 }.missionState
       --- If the mission requirements are done, kill the collector and start a timer to zone the player out.
         if (SurvivalMissionState == 4 or SurvivalMissionState == 12)  and self:GetHealth{}.health < 500 then
            self:SetVar("ExitTime", 5)
            GAMEOBJ:GetTimer():AddTimerWithCancel( self:GetVar("ExitTime")  , "ExitTime", self )
            self:SetMaxHealth{health = 9999}
            self:SetHealth{health = 9999}
        end
    end
end


--Gets the player that built the collector so we can update the mission for that player.
function onRebuildStart(self, msg)
    storeObjectByName (self, "playerID", msg.userID )
   
end

-------------------------
-- Check if the timer for the force exit is done, then zone player.
-------------------------
onTimerDone = function(self, msg)
    if msg.name == "ExitTime" then
        local player = getObjectByName(self, "playerID")  
        if player ~= nil then 
        --player:TransferToZone{ zoneID = 22 }
         player:TransferToLastNonInstance{ playerID = player, bUseLastPosition = true } 
         end
    end
    if msg.name == "TickTime" then
      --  print("%%%%%%%%%%pulsing this prox")
        self:CastSkill{skillID = 193}
        GAMEOBJ:GetTimer():AddTimerWithCancel( self:GetVar("TickTime")  , "TickTime", self )
    end
end

-----------------------
--Checking Proximity for spiderlings
-----------------------
function onProximityUpdate(self, msg)
	if msg.status == "ENTER" and self:GetVar("built") == 1 then
		local faction = msg.objId:GetFaction()
		if faction and faction.faction == 35 then
		    GAMEOBJ:GetTimer():AddTimerWithCancel( self:GetVar("TickTime")  , "TickTime", self )
		   -- print("@@@@found critter in prox")
			--self:CastSkill{skillID = skillID, optionalTargetID = msg.objId}
		end
	end
	if msg.status == "LEAVE" and self:GetVar("built") == 1 then
		local faction = msg.objId:GetFaction()
		if faction and faction.faction == 35 then
		  --  print("+++++++++++++++++Critter left the prox")
		    GAMEOBJ:GetTimer():CancelTimer("TickTime", self)
		end
	end
end