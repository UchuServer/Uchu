
-- cinematics
local startCinematic = "FrakjawSummoning"
local playerWaiting = "PlayersLoadingIn"

-- groups
local ledgeFrakjawGroup = "LedgeFrakjaw"
local groundFrakjaw = "GroundFrakjaw"


----------------------------------------------------------------
-- the server told us to do something
----------------------------------------------------------------
function onNotifyClientObject(self,msg)

	local player = GAMEOBJ:GetControlledID()

	-- a player has loaded
	if msg.name ==  "PlayerLoaded" then
	

		-- make sure the player that loaded is the local player
		if not (msg.paramObj) and not ( msg.paramObj:GetID() == player:GetID() ) then return end
		-- player the plaeyrs waiting camera
		player:PlayCinematic { pathName = playerWaiting }
		
	-- ledge frakjaw died, need to swich the healthbar to new frakjaw
	elseif msg.name == "LedgeFrakjawDead" then
		
		SwitchHealthBar(self)
		
	-- Frakjaw is dead, turn the health bar off
	elseif msg.name == "GroundFrakjawDead" then
	
		UI:SendMessage( "ToggleEnemyStatusBar", { {"visible", false} } )
		self:SetVar("HealthBarOn",false)
		
	-- generic message from server to play a cinematic, the cinematic path should be sent with the message
	elseif msg.name == "PlayCinematic" then
	
		-- if theres no path sent, we cant do anything
		if not msg.paramStr then return end
		
		-- if the cinematic for the counterweight quickbuild, do extra math to see where in the quickbuild the player is at
		if string.sub(msg.paramStr,0,15) == "CounterweightQB" then
		
			-- if theres no path sent, we cant do anything
			if not msg.paramObj and msg.paramObj:Exists() then return end
		
			local AdvanceTime = msg.paramObj:GetBuildTimeDetails().buildTimeElapsed
			
			player:PlayCinematic { pathName = msg.paramStr, startTimeAdvance = AdvanceTime }
			return
			
		end			

		-- get the player and paly the cinematic

		player:PlayCinematic { pathName = msg.paramStr }
		
	-- servers wants to end any cienmatics playing
	elseif msg.name == "EndCinematic" then
	
		-- get the player and end their cinematics
		player:EndCinematic()
		
	elseif msg.name == "StartMusic" then
		player:ActivateNDAudioMusicCue{m_NDAudioMusicCueName = msg.paramStr}
		
		
	elseif msg.name == "StopMusic" then
		player:DeactivateNDAudioMusicCue{m_NDAudioMusicCueName = msg.paramStr}
	elseif msg.name == "FlashMusic" then
		player:FlashNDAudioMusicCue{m_NDAudioMusicCueName = msg.paramStr}
		
	elseif msg.name == "PlayerLeft" and msg.paramObj then
	
		if msg.paramObj:Exists() then
		
			-- make sure their health bar is on
			if not self:GetVar("HealthBarOn") then return end
						
			--turn off the health bar, we don't have a player near us anymore :(
			UI:SendMessage( "ToggleEnemyStatusBar", { {"visible", false} } )
			self:SetVar("HealthBarOn",false)
			
		end
	
	end
	
end

----------------------------------------------------------------
-- got an update from the cinematic
----------------------------------------------------------------
function onCinematicUpdate(self, msg)
	
	-- if the paths is the start cinematic, and it has ened
	if msg.pathName == startCinematic and msg.event == "ENDED" then
	
		-- make sure the health bar isnt already on
		if self:GetVar("HealthBarOn") then return end
		
		-- turn on the health bar
		TurnOnHealthBar(self)
		
	end		
		
end

----------------------------------------------------------------
-- if frakjaw has been hit
----------------------------------------------------------------
function notifyHitOrHealResult(self,frakjaw,msg)

	-- update the health bar
	UpdateHealthBar(self)	
	
end

----------------------------------------------------------------
-- custom function - turn the health hud on
----------------------------------------------------------------
function TurnOnHealthBar(self)

	-- get the player
	local player = GAMEOBJ:GetControlledID()
	
	-- if the player isnt ready, use a heart beat time until he is
	if not player:GetPlayerReady().bIsReady then 
		GAMEOBJ:GetTimer():AddTimerWithCancel(1, "CheckForLedgeFrakjaw", self)
		return
	end
	
	-- create a variable for frakjaw
	local ledgeFrakjaw = self:GetVar("Frakjaw")
	
	-- if theres no save frakjaw
	if not ( ledgeFrakjaw ) then --or ledgeFrakjaw:Exists() ) then
	
		-- get all objs in the group of ledge frakjaw
		ledgeFrakjaw = self:GetObjectsInGroup{ group = ledgeFrakjawGroup, ignoreSpawners = true }.objects
		
		-- if no objects return, start a heartbeat timer and wait for him
		if table.maxn(ledgeFrakjaw) < 1 then
			GAMEOBJ:GetTimer():AddTimerWithCancel(1, "CheckForLedgeFrakjaw", self)
			return
		end
		
		-- object returned, parse through them
		for k,v in ipairs(ledgeFrakjaw) do
			if v:Exists() then
				-- set valid obj as frakjaw
				ledgeFrakjaw = v
				break
			end
		end
		
		-- turn off frakjaws billboard
		ledgeFrakjaw:SetNameBillboardState{bState = false, bOverrideDefaultSetting = true}
		-- save out frakjaw
		self:SetVar("Frakjaw",ledgeFrakjaw)
		self:SendLuaNotificationRequest{requestTarget = ledgeFrakjaw, messageName = "HitOrHealResult"}
		
	end

	
	--turn on the health bar, using frakjaws stats
	UI:SendMessage( "ToggleEnemyStatusBar", { {"visible", true}, {"healthVisible", true},
		{"armorVisible", true},
		{"nameVisible", true},
		{"health", math.floor((ledgeFrakjaw:GetHealth().health / ledgeFrakjaw:GetMaxHealth().health) * 100) },
		{"armor", math.floor((ledgeFrakjaw:GetArmor().armor / ledgeFrakjaw:GetMaxArmor().armor) * 100) },
		{"nameTxt", ledgeFrakjaw:GetName().name},
		{"id", "|" .. ledgeFrakjaw:GetID()} } )
		
	-- we have turned on the healthbar
	self:SetVar("HealthBarOn",true)
	
end

----------------------------------------------------------------
-- custom function - update the health hud
----------------------------------------------------------------
function UpdateHealthBar(self)

	-- get the player
	local player = GAMEOBJ:GetControlledID()
	-- make sure their health bar is turned on
	if not self:GetVar("HealthBarOn") then return end

	
	local frakjaw = self:GetVar("Frakjaw")
	if not frakjaw:Exists() then return	end
	
	-- set armor vis to true
	local armorVis = true
	-- if there is no armor, set vis to false
	if frakjaw:GetArmor().armor == 0 then
		armorVis = false
	end
	
	-- update the health bar
	UI:SendMessage( "UpdateEnemyStatusBar", { {"healthVisible", true},
		
		{"armorVisible", armorVis},
		{"nameVisible", true},
		{"health", math.floor((frakjaw:GetHealth().health / frakjaw:GetMaxHealth().health) * 100) },
		{"armor", math.floor((frakjaw:GetArmor().armor / frakjaw:GetMaxArmor().armor) * 100) },
		{"nameTxt", frakjaw:GetName().name},
		{"id", "|" .. frakjaw:GetID()} } )

end

----------------------------------------------------------------
-- switch health bar to other frakjaw
----------------------------------------------------------------
function SwitchHealthBar(self)

	-- make sure there is a healthbar
	if not self:GetVar("HealthBarOn") then return end
	
	-- get all frakjaws in the group for frakjaw
	local Frakjaw = self:GetObjectsInGroup{ group = groundFrakjaw, ignoreSpawners = true }.objects
	
	-- if none are found, start a heartbeat timer and check again
	if table.maxn(Frakjaw) < 1 then
		GAMEOBJ:GetTimer():AddTimerWithCancel(1, "CheckForGroundFrakjaw", self)
		return
	end
	
	-- object returned, parse through them
	for k,obj in ipairs(Frakjaw) do
	
		if obj:Exists() then
		
			-- save out frakjaw
			self:SetVar("Frakjaw",obj)
			-- turn off frakjaws billboard
			obj:SetNameBillboardState{bState = false, bOverrideDefaultSetting = true}
			-- want to know when he gets hit to update the health bar
			self:SendLuaNotificationRequest{requestTarget = obj, messageName = "HitOrHealResult"}
			-- update the health bar
			UpdateHealthBar(self)
			break
			
		end
		
	end
	
end

----------------------------------------------------------------
-- when the local player exits
----------------------------------------------------------------
function onPlayerExit(self,msg)

	-- make sure their health bar is on
	if not self:GetVar("HealthBarOn") then return end
				
	--turn off the health bar, we don't have a player near us anymore :(
	UI:SendMessage( "ToggleEnemyStatusBar", { {"visible", false} } )
	self:SetVar("HealthBarOn",false)
	
end

----------------------------------------------------------------
-- a set timer is done.. ding
----------------------------------------------------------------
function onTimerDone(self,msg)
	
	-- heartbeat time to look for a ledge frakjaw
	if msg.name == "CheckForLedgeFrakjaw" then
		
		-- try turn on health bar again
		TurnOnHealthBar(self)
		
	-- heartbeat time to look for a ground frakjaw
	elseif msg.name == "CheckForGroundFrakjaw" then
	
		-- start custom function to switch out heath bar
		SwitchHealthBar(self)
		
	end
	
end


	