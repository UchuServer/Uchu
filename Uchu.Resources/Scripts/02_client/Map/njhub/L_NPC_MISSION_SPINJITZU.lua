--------------------------------------------------------------
-- client side script for the 4 elemental ninjas in the dojos
-- the script has the npc perform a spinjitzu then force the player to interact with the npc for the next mission offer

-- created by Brandi... 7/14/11
--------------------------------------------------------------

-- table of the mission based on the element
local missionT = { ["earth"] = 1796,
					["lightning"] = 1952,
					["ice"] = 1959,
					["fire"] = 1962
				 }
				 
function onStartup(self)

	-- preload the anim of the npc spinjitzuing 
	local element = self:GetVar("element")
	if not element then return end
	
	self:PreloadAnimation{animationID = "spinjitzu-canned-"..element, respondObjID = self}

end

function onMissionDialogueOK(self,msg)

	--get the element that is placed on the npc in hf
	local element = self:GetVar("element")
	if not element then return end
	
	-- the player has completed the mission to put on the elemental gear
	if msg.bIsComplete and msg.missionID == missionT[element] then
		
		GAMEOBJ:GetTimer():AddTimerWithCancel( .5, "StartCam",self ) 
		
	end
	
end

function onAnimationComplete(self, msg)	
	local element = self:GetVar("element")
	if not element then return end
		
	-- if we're the last animation in the table stop the cinematic
	if msg.animationID == "spinjitzu-canned-"..element then
		GAMEOBJ:GetTimer():AddTimerWithCancel(1, "stopCinematic", self)
	end
end

function onTimerDone (self,msg)

	if (msg.name == "OfferNextMission") then
	
		-- unstun the player
		local player = GAMEOBJ:GetControlledID()
		if not player:Exists() then return end
		
		player:SetStunned{StateChangeType = "POP", bCantMove = true, bIgnoreImmunity = true, bCantTurn = true, bCantEquip = true}

		-- end the camera
		player:EndCinematic()
		--forcing player interaction to offer next mission
		player:ForcePlayerToInteract{objToInteractWith = self}
		
	elseif msg.name == "PlayAnim" then
	
		-- play the spinjitzu animatation
		local element = self:GetVar("element")
		if not element then return end
		
		if self:GetCurrentAnimation().secondaryAnimationID ~= "" then
			GAMEOBJ:GetTimer():AddTimerWithCancel(0.5, "PlayAnim", self)
			
			return
		end
		
		self:PlayAnimation{ animationID = "spinjitzu-canned-"..element, bPlayImmediate = true } 
		
	elseif msg.name == "StartCam" then
	
		local element = self:GetVar("element")
		if not element then return end
		
		local player = GAMEOBJ:GetControlledID()
		if not player:Exists() then return end
		
		-- stun the player
		player:SetStunned{StateChangeType = "PUSH", bCantMove = true, bIgnoreImmunity = true, bCantTurn = true, bCantEquip = true}
		--push the camera to the mission camera
		player:HandleInteractionCamera{cameraTargetID = self, actionType = "ACTION_TYPE_ENABLE" }	
		
		-- timer to play the animation
		GAMEOBJ:GetTimer():AddTimerWithCancel( 1.5, "PlayAnim",self )
		-- timer to offer the next mission
		GAMEOBJ:GetTimer():AddTimerWithCancel( 7, "OfferNextMission",self )  
		
    end 
end