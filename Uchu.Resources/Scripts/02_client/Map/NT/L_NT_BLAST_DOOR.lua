--------------------------------------------------------------
-- client side script on the dukes blast door in NT
--
-- created by mrb... 5/4/11
--------------------------------------------------------------

function onCollisionPhantom(self, msg)	
	if self:GetVar("bUp") then return end
	
	-- if this isn't the local player then dont do anything
	if msg.objectID:GetID() ~= GAMEOBJ:GetLocalCharID() then return end	
	
	-- open the door
	OpenDoor(self, true)
end

function onOffCollisionPhantom(self, msg)	
	if not self:GetVar("bUp") then return end
	
	-- if this isn't the local player then dont do anything
	if msg.objectID:GetID() ~= GAMEOBJ:GetLocalCharID() then return end	
	
	-- close the door
	OpenDoor(self, false)
end 

function OpenDoor(self, bOpen)	
	-- set the bUp to bOpen so we'll know what to do when the animation is complete
	self:SetVar("bUp", bOpen)

	-- if we're moving then then dont do anything else
	if self:GetVar("bMoving") then return end
	
	-- get the right animation
	local animName = "nexus-window-up"
	
	if not bOpen then
		animName = "nexus-window-down"
	end
	
	-- set bMoving and play the animation
	self:SetVar("bMoving", true)
	self:PlayAnimation{animationID = animName, bPlayImmediate = true, bTriggerOnCompleteMsg = true}	
end

function onAnimationComplete(self, msg)
	-- door is not moving anymore
	self:SetVar("bMoving", false)
	
	-- set the play the correct animation based on what animation is complete
	if msg.animationID == "nexus-window-up" then
		-- see if we need to make the door go back down right now
		if not self:GetVar("bUp") then
			OpenDoor(self, false)
		else		
			self:PlayAnimation{animationID = "nexus-window-up-idle", bPlayImmediate = true}
		end
	elseif msg.animationID == "nexus-window-down" then
		-- see if we need to make the door go up right now
		if self:GetVar("bUp") then
			OpenDoor(self, true)
		else		
			self:PlayAnimation{animationID = "idle", bPlayImmediate = true}
		end
	end
end 

