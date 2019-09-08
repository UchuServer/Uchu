require('State')
require('o_StateCreate')
require('o_mis')
require('o_Main')




CONSTANTS = {}
CONSTANTS["RebuildStateOpen"] = 0
CONSTANTS["RebuildStateCompleted"] = 2
CONSTANTS["TickTime"] = 1
CONSTANTS["KillTime"] = 30

function onStartup(self) 
	----------------------------------
	-- Sets the turret's life timer and disables its AI which will only be active after rebuild.
	self:SetVar("TickTime", 1)
	self:SetVar("currentTime", 1)
	--disable AI here:
	self:EnableCombatAIComponent{bEnable = false}
	GAMEOBJ:GetTimer():AddTimerWithCancel( self:GetVar("TickTime")  , "TickTime", self )
	----------------------------------*
	self:SetVar("building", 0)
	self:SetGravityScale{scale = 0.0}
	self:SetStunImmunity{StateChangeType = "PUSH", bImmuneToStunAttack = true, bImmuneToInterrupt = true} -- Make immune to stuns
    self:SetStatusImmunity{ StateChangeType = "PUSH", bImmuneToPullToPoint = true, bImmuneToKnockback = true } -- Make immune to move/teleport behaviors
end

--------------------------------------------------------------------------------
-- onRebuildNotifyState
-- 
-- Notes: Whenever the rebuild state changes on the robotanist, this is called.
--------------------------------------------------------------------------------
function onRebuildNotifyState(self, msg)   
	if (msg.iState == CONSTANTS["RebuildStateCompleted"]) then
	  -- Enable AI here:
	  self:EnableCombatAIComponent{bEnable = true}
	    self:SetVar("building", 0)
	    GAMEOBJ:GetTimer():CancelAllTimers( self )
	    GAMEOBJ:GetTimer():AddTimerWithCancel( self:GetVar("TickTime")  , "TickTime", self )
	end
	if (msg.iState == CONSTANTS["RebuildStateOpen"]) then
			
	end
end

-------------------------
-- Tells the turret to die after the timer runs out.
-------------------------
function onTimerDone(self, msg)
    -- 1 second tick timer to increment an overall time (currentTime)
    if msg.name == "TickTime" then
       self:SetVar("currentTime", (self:GetVar("currentTime") + 1))
       --  When the currentTime exceeds 30 seconds and the player is not currently building, kill the turret
       if (self:GetVar("currentTime") >= CONSTANTS["KillTime"]) and (self:GetVar("building") == 0)then
           self:EnableCombatAIComponent{bEnable = false}
           self:Die()
           self:SetVar("currentTime", 0)
       end
       GAMEOBJ:GetTimer():AddTimerWithCancel( self:GetVar("TickTime")  , "TickTime", self )
    end
end

-----------------------------
-- Getting the playerID of the rebuilder and setting new timer for the life of the turret
-----------------------------
function onRebuildStart(self, msg)
	self:LockNodeRotation{nodeName = "base"}
    storeObjectByName (self, "playerID", msg.userID )
    self:SetVar("building", 1)
   
end

-----------------------------
-- Reroute Kill credit to the player
-----------------------------
function onUpdateMissionTask(self,msg)
	if msg then
		local player = getObjectByName(self, "playerID")
		if player then
			player:UpdateMissionTask{target = msg.target, value = msg.value, value2 = msg.value2, taskType = msg.taskType}
		end
	end
end
