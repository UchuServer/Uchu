----------------------------------------------------------------
-- Description:
--
-- client script on bob in spaceship to show animations after completing first
--	mission and forcing second mission offer
-- updated mrb... 7/20/11
--------------------------------------------------------------
--//////////////////////////////////////////////////////////////////////////////////
local missionCamDist = 10 -- distance to teleport player away from npc
local cineNom = "Imagination_Cam"
--//////////////////////////////////////////////////////////////////////////////////
function onStartup(self)
    self:PreloadAnimation{animationID = "charge"}
end

function onInteractionAttemptFromOutOfRange(self, msg)
    -- We must always let the input system know we handled the interaction attempt
    msg.bHandled = true    
    self:DisplayChatBubble{wsText = "You need to move closer."}    
    return msg
end

function onMissionDialogueOK(self, msg)  
	if msg.missionID == 173 then
		if msg.bIsComplete then
		    -- Begin player celebration cinematic sequence
		    GAMEOBJ:GetTimer():AddTimerWithCancel( .5, "StartCam", self ) 
			
			-- get the local player and lock controls/movment
			local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
			
			-- set a variable indicating an "extra" stun is being applied to the player
			self:SetVar("ExtraAnimStun", true)
			
			-- stun the player so they don't move while the animation is playing
			player:SetStunned{StateChangeType = "PUSH", bCantMove = true, bCantTurn = true, bCantEquip = true, bCantInteract = true, bDontTerminateInteract = true}
            self:PlayAnimation{animationID = "charge"}
			
			-- set the player flag so that the imagination consoles near the launch stations work
			player:SetFlag{iFlagID = "66", bFlag = true}
			
			-- tell the consoles near the end of the ship that they can be interacted with
			local group = self:GetObjectsInGroup{group = "console", ignoreSpawners = true}.objects
			
			for i, object in pairs(group) do
				if object then
				   object:RequestPickTypeUpdate()
				end
			end
			
			-- get the player tele location object placed in HF and save the position and rotation
			local pLocObj = self:GetObjectsInGroup{ group = "Imag_Cam_Player_Loc" }.objects 
			local pPos = pLocObj[1]:GetPosition().pos
			local pRot = pLocObj[1]:GetRotation()
			
			-- move the player to the correct location and start the charge animation
			player:SetPosition {pos = {x=pPos.x,y=pPos.y,z=pPos.z}}
			player:SetRotation {x=pRot.x,y=pRot.y,z=pRot.z,w=pRot.w}
			player:PlayAnimation{animationID = "charge"}
			
			-- play Imagination_Cam cinematic using mission_text db      
			player:PlayFaceDecalAnimation { animationID = "Focused", useAllDecals = true } 
        end
	
	elseif msg.missionID == 660 and msg.iMissionState < 4 and not msg.responder:GetStunned().bCanMove then
		-- check the variable indicating an "extra" stun was applied to the player
		local extraAnimStun = self:GetVar("ExtraAnimStun")
		
		-- if the "extra" stun was applied, remove it now
		if extraAnimStun then
			self:SetVar("ExtraAnimStun", nil)
			msg.responder:SetStunned{StateChangeType = "POP", bCantMove = true, bCantTurn = true, bCantEquip = true, bCantInteract = true, bDontTerminateInteract = true}
		end
    end
end

function onMissionDialogueCancelled(self, msg)
    if not msg.responder:GetStunned().bCanMove and msg.missionID == 660 then return end
    
	-- remove the "extra" stun indicator. if there was one, the extra stun state 
	-- was cancelled when the mission offerer cinematic ended
	self:SetVar("ExtraAnimStun", nil)
	msg.responder:SetStunned{StateChangeType = "POP", bCantMove = true, bCantTurn = true, bCantEquip = true, bCantInteract = true, bDontTerminateInteract = true}
end

function onTimerDone (self,msg)
    local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
    
    -- depending on which timer is up play different animations and facial animations.
    if msg.name == "PlayCheer" then   
		-- play animations
        player:PlayFaceDecalAnimation { animationID = "Happy", useAllDecals = true }
        player:PlayAnimation{animationID = "cheer"}
        self:PlayAnimation{ animationID = "clap" }
    elseif msg.name == "OfferNextMission" then
        player:EndCinematic()
	    --forcing player interaction to offer next bob mission
        player:SetStunned{StateChangeType = "POP", bCantInteract = true}
        player:ForcePlayerToInteract{objToInteractWith = self}
	
	elseif msg.name == "StartCam" then
	    GAMEOBJ:GetTimer():AddTimerWithCancel( 4.5, "PlayCheer", self )
	    
	    player:EndCinematic()
	    player:HandleInteractionCamera{wsCameraPath = cineNom, cameraTargetID = player, actionType = "ACTION_TYPE_ENABLE"}
        local cineTime = tonumber(LEVEL:GetCinematicInfo(cineNom)) or 9
        GAMEOBJ:GetTimer():AddTimerWithCancel(cineTime, "OfferNextMission", self)
    end    
end
