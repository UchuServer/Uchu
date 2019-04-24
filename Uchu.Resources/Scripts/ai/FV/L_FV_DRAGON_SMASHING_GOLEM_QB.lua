----------------------------------------------
-- Script for the golem QB which will need to be 
-- completed to expose the Maelstrom Dragon's weak 
-- point to the player

-- updated mrb... 3/10/11 - added checkUseRequirements for loot tag
----------------------------------------------


----------------------------------------------
-- Check to see if the player can use the golem
----------------------------------------------
function onCheckUseRequirements(self, msg)
	local tagObj = self:GetVar("lootTagOwner") or false
	
	if tagObj and tagObj:Exists() then
		if tagObj:GetID() ~= msg.objIDUser:GetID() and not msg.objIDUser:TeamIsOnWorldMember{i64PlayerID = tagObj}.bResult then		
			msg.bCanUse = false
		end
	end
    
    return msg
end

function onStartup(self)	
	local tagObj = self:GetVar("lootTagOwner") or false
	
	self:SetNetworkVar("lootTagOwner", tagObj)
	
	GAMEOBJ:GetTimer():AddTimerWithCancel( 10.5 , "GolemBreakTimer", self )
end

function onRebuildNotifyState(self, msg)
	local Dragon = self:GetParentObj().objIDParent 
	
	self:SetVar("userID", msg.player:GetID())
	
	if( Dragon == nil or Dragon:Exists() == false ) then
		return
	end

	-- a player just did the quickbuild.
	if (msg.iState == 2) then	
		-- Notify the Dragon that the build is done and using the player as the sender so we can update missions.
		 Dragon:NotifyObject{ ObjIDSender = msg.player, name = "rebuildDone" }
		 GAMEOBJ:GetTimer():CancelTimer("GolemBreakTimer", self);      -- Cancel the revive timer to restart it 
		 GAMEOBJ:GetTimer():AddTimerWithCancel( 10.5  , "GolemBreakTimer", self )
		 self:PlayAnimation{ animationID = "dragonsmash" }
    end
    
    -- the rebuild was cancelled
    if (msg.iState == 4) then                    
		Dragon:NotifyObject{ ObjIDSender = self, name = "rebuildCancel" }
    end
end

function onDie(self, msg)
    local Dragon = self:GetParentObj().objIDParent 
    
	if( Dragon == nil or Dragon:Exists() == false ) then
		return
	end

	Dragon:NotifyObject{ ObjIDSender = self, name = "rebuildCancel" }
end

function onTimerDone(self, msg) 
	if msg.name == "GolemBreakTimer" then
		self:RebuildCancel{bEarlyRelease = true, userID = GAMEOBJ:GetObjectByID(self:GetVar("userID"))}
        self:Die{}
    end
end
