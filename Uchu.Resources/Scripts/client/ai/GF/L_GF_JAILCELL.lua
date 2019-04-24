--------------------------------------------------------------
-- Client Script on the jail cell doors in Gnarled Forest
-- this script limits the doors to only being smashed by hammers from FV, 
-- and sends a message to the ninja inside that it can be interacted with

-- updated Brandi... 2/22/10
-- updated mrb... 10/20/10 -- added need dragon hammer message
--------------------------------------------------------------

--table of the 4 hammers the player could get in forbidden valley
local hammers = { 2963,3014,3015,3016}

function onRenderComponentReady(self)
	-- turn off the health bar above the jail cell door
	self:SetNameBillboardState{bState = false }
	self:SetVar("smashed",false)	
    self:SetProximityRadius{radius = 16, name = "Message_Distance"}
end

function onProximityUpdate(self, msg)
    if msg.name == "Message_Distance" and msg.status == "ENTER" then
        local player = msg.objId
        
        if not player:Exists() then return end
        
		local msgcheck = player:CheckPrecondition{PreconditionID = 96, iPreconditionType = 0}
        
        if not msgcheck.bPass then
            -- display the dragon hammer needed message
            player:DisplayTooltip { bShow = true, strText = msgcheck.FailedReason, iTime = 3000 }        
        end
    end
end

function onOnHit(self,msg)
	local player = GAMEOBJ:GetControlledID()
	
	--makes sure the player is the only client to see this 
	if msg.attacker:GetID() == player:GetID() and self:GetVar("smashed") == false then	
		local item = player:GetEquippedItemInfo{ slot = "special_r" }.lotID
		local smash = false
		
		-- check to see if the player had one of the hammers equipped, door can only be smashed with one of the hammers
		for k,v in ipairs(hammers) do		
			if item == v then			
				smash = true
				break
			end			
		end
		
		--if the player hit the door with one of the hammers
		if smash == true then			
			self:Smash()
			self:SetVar("smashed",true)
			GAMEOBJ:GetTimer():AddTimerWithCancel(15, "unSmashTimer", self )
			
			local mis385 = player:GetMissionState{missionID = 385}.missionState
			
			-- if the player is on the free or feed the ninjas mission, turn on the icon of the ninja behind the door
			if player:GetFlag{iFlagID = 68}.bFlag == true and mis385 >= 8 then
			
				local group = "Ninja"..self:GetVar("number")
				local ninja = self:GetObjectsInGroup{ group = group, ignoreSpawners = true }.objects[1]

				ninja:FireEventClientSide{ args = "doorbusted" }				
			end			
		end		
	end	
end

function UnSmashMe(self)
    if self:GetVar("smashed") == true then
		self:UnSmash{duration = 5}
		self:SetVar("smashed",false)
			
		local group = "Ninja"..self:GetVar("number")
		local ninja = self:GetObjectsInGroup{ group = group, ignoreSpawners = true }.objects[1]

		ninja:FireEventClientSide{ args = "doorRebuilt" }				
	end
end

function onNotifyObject(self,msg)	
	--notifyed from ninja that the player interacted with the ninja and stop the door from bring unsmashed
	if msg.name == "CancelTimer" then		
		GAMEOBJ:GetTimer():CancelTimer("unSmashTimer", self)	
	-- notify from the ninja to unsmash the jail door
	elseif msg.name == "unSmash" then		
		UnSmashMe(self)		
	end
end

function onTimerDone(self,msg)
	--timer on the door to unsmash itself
	if msg.name == "unSmashTimer" then
		UnSmashMe(self)
	end
end 