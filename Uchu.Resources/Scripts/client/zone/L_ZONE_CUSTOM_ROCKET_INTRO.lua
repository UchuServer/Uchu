--------------------------------------------------------------
-- Rocket Rebuild Script
--------------------------------------------------------------


--------------------------------------------------------------
--
-- INSTRUCTIONS:
--
-- To use this script, you must first include it in a zone
-- control object script.
--
-- Then, implement the following methods and inside each one
-- make sure to call the CUSTOMROCKET_* version:
--
--		- onChildRenderComponentReady
--		- onChildBuildAssemblyComplete
--		- onAnimationFinishedPreloading
--		- onTimerDone
--
-- (You can choose to put your own custom stuff on your functions
-- if you so choose... it won't interfere with this system)
--
-- Lastly, call CUSTOMROCKET_BeginTransition with the appropriate
-- parameters
--
-- (see scripts/client/zone/AG/L_ZONE_AG_CLIENT.lua for an example)
--
--------------------------------------------------------------

function CUSTOMROCKET_BeginTransition(self, charID, spawnPos, spawnRot, spawnCinematic, spawnTransitionAnim)

    charID:SetPosition{pos = spawnPos}
    charID:SetRotation{x = spawnRot.x, y = spawnRot.y, z = spawnRot.z, w = spawnRot.w}
    charID:ActivatePhysics{bActivate = false}
	charID:SetVisible{visible = false, fadeTime = 0.0}

    local config = { {"playerID","|"..charID:GetID()}, {"cinematic", spawnCinematic}, {"transitionAnim", spawnTransitionAnim} }
    RESMGR:LoadObject { objectTemplate = 6398, x = spawnPos.x, y = spawnPos.y, z = spawnPos.z, rw = spawnRot.w, rx = spawnRot.x, ry = spawnRot.y, rz = spawnRot.z, owner = self, configData = config}
    
end


function CUSTOMROCKET_onChildRenderComponentReady(self, msg)
    if msg.childLOT == 6398 then

        local playerID = GAMEOBJ:GetObjectByID(msg.childID:GetVar("playerID"))

		--print("--------------------- (Child ID: " .. tostring(msg.childID:GetID()) .. ")")
		--print("--------------------- (Child LOT: " .. tostring(msg.childID:GetLOT().objtemplate) .. ")")
		--print("--------------------- (Player ID: " .. tostring(msg.childID:GetVar("groupID")) .. ")")
		--print("--------------------- (Player (Actual) ID: " .. tostring(playerID:GetID()))

		-- fail if the player doesn't exist anymore
		if (playerID:Exists() == false) then
			return
		end

		-- Attach whatever we last built (assumedly a rocket or similar) as the custom build to show.
		local lastBuildTokenList = playerID:GetLastCustomBuild().tokenizedLOTList
		msg.childID:SetBuildAssembly{ tokenizedLOTList = lastBuildTokenList }

    end
end

function CUSTOMROCKET_onChildBuildAssemblyComplete(self, msg)
    if msg.childLOT == 6398 then
        local playerID = GAMEOBJ:GetObjectByID(msg.childID:GetVar("playerID"))

		-- fail if the player doesn't exist anymore
		if (playerID:Exists() == false) then 
			return 
		end

		-- the animation we need to play depends on whether the owner is the local player
		local playerTransitionAnim
		if (msg.childID:GetVar("transitionAnim") ~= nil) then
			playerTransitionAnim = msg.childID:GetVar("transitionAnim")
		else
			playerTransitionAnim = "rocket-transition_default"
		end

        -- First, preload the anims, to get them as in-sync as we can
        playerID:PreloadAnimation{animationID = playerTransitionAnim, respondObjID = self, userData = { {"playerID","|"..playerID:GetID()}, {"rocketID","|"..msg.childID:GetID()} } }
	end
end

function CUSTOMROCKET_onAnimationFinishedPreloading(self, msg)

	if (msg.userData["rocketID"] ~= nil) then

		local playerID = msg.userData["playerID"]
		local rocketID = msg.userData["rocketID"]

		-- if either the rocket or the player no longer exist, bail out
		if (rocketID:Exists() == false or playerID:Exists() == false) then
			if (playerID:Exists() and playerID:GetID() == GAMEOBJ:GetLocalCharID()) then
				StartZoneSummary(self)
			end
			return
		end

		-- if we've finished pre-loading the player's animation, then preload the rocket's animation next
		if (msg.animObjID:GetID() == playerID:GetID()) then

			rocketID:PreloadAnimation{animationID = msg.animationID, respondObjID = self, userData = { {"playerID","|"..msg.userData["playerID"]}, {"rocketID","|"..msg.userData["rocketID"]} } }

		else
			-- Force the player to be visible, so that it can't miss its animation calls / doesn't look funny during them
			playerID:PCreateEffectFinished()
			playerID:SetVisible{visible = true, fadeTime = 0.0}

			-- Enable the rocket to animate off-screen, since this is a cinematic, and off-screen is required
			rocketID:SetOffscreenAnimation{bAnimateOffscreen = true}
			playerID:SetOffscreenAnimation{bAnimateOffscreen = true}

			-- if we're done pre-loading animations, we're ready to kick off the final animations
			-- (they differ depending on if we're the local character or just another player)

			if (playerID:GetID() == GAMEOBJ:GetLocalCharID()) then

				GAMEOBJ:GetTimer():AddTimerWithCancel( 10, "deleteLocalRocket_" .. rocketID:GetID(),self )    
				GAMEOBJ:GetTimer():AddTimerWithCancel( 0.1, "CustomLocalRocketTimer",self )

				-- change ui state to CinematicBars
				UI:SendMessage( "pushGameState", {{"state", "cinematic" }} )    

				-- Now, start the animations
				playerID:PlayAnimation{animationID = msg.animationID}
				rocketID:PlayAnimation{animationID = msg.animationID}
				if (rocketID:GetVar("cinematic") ~= nil) then
					playerID:PlayCinematic { pathName = rocketID:GetVar("cinematic") }
				end

			else

				GAMEOBJ:GetTimer():AddTimerWithCancel( 10, "deleteOtherRocket_" .. rocketID:GetID() .. "_" .. playerID:GetID(), self )    

				-- Now, start the animations
				playerID:PlayAnimation{animationID = msg.animationID}
				rocketID:PlayAnimation{animationID = msg.animationID}
				
			end

		end
	end
end

function CUSTOMROCKET_onTimerDone(self, msg)

    if (string.find(msg.name, "deleteLocalRocket") ~= nil) then  
		objids = {}
		for w in string.gmatch(msg.name, "%d+") do
			objids[#objids + 1] = w
		end
        GAMEOBJ:DeleteObject(GAMEOBJ:GetObjectByID(objids[1]))
        StartZoneSummary(self)
    end
    
	if (string.find(msg.name, "deleteOtherRocket") ~= nil) then  
		objids = {}
		for w in string.gmatch(msg.name, "%d+") do
			objids[#objids + 1] = w
		end
		EndOtherCharacterTransition(self, GAMEOBJ:GetObjectByID(objids[1]), GAMEOBJ:GetObjectByID(objids[2]))
	end

end


----------------------------------------------------------------------------
-- Begin Custom Helper Functions (local just to the custom rocket intro):
----------------------------------------------------------------------------

-- Kick off the Zone Intro UI, mark the transition as complete
function StartZoneSummary(self)
    -- change ui state to normal
    UI:SendMessage( "popGameState", {{"state", "cinematic"}} )
    
    local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
    
    -- set flag to true so we know the player has already done this
    player:SetFlag{iFlagID = 32, bFlag = true}
    
    -- reactivate their physics
    player:ActivatePhysics{bActivate = true}
    
    -- display zone summary
    player:DisplayZoneSummary{sender = self, isZoneStart = true}
end

-- function to call when any non-local characters finish their rocket transitions
function EndOtherCharacterTransition(self,rocketID,playerID)

	--print("EndOtherCharTransition: Found RocketID " .. tostring(rocketID:GetID()))
	--print("EndOtherCharTransition: Found PlayerID " .. tostring(playerID:GetID()))

	if (playerID:Exists()) then
		playerID:SetOffscreenAnimation{bAnimateOffscreen = false}
		playerID:ActivatePhysics{bActivate = true}
	end
	GAMEOBJ:DeleteObject(rocketID)
	
end
