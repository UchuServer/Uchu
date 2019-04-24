--------------------------------------------------------------
-- plays animations for the assistant
--
-- created mrb... 5/17/11
--------------------------------------------------------------

local misID = 1728	
-- camera path
local cinName = "LookAtNinjago"
-- animations & fx to play
local eventSet = { 	{fx = -1, anim = "missionState3"}, 
					{fx = -1, anim = "whirlwind-start-earth1"}, 
					{fx = -1, anim = "idle-misc2"}, 
					{fx = 6647, anim = "whirlwind-end"} }
-- mesh fx and when to play it
local meshFX = {fxID = 6776, fx = "cast", start = 2, stop = 4}

----------------------------------------------
-- Sent when the object checks its pick type
----------------------------------------------
function onGetPriorityPickListType(self, msg)  
	local myPriority = 0.8
	
	if ( myPriority > msg.fCurrentPickTypePriority ) then    
		msg.fCurrentPickTypePriority = myPriority 

		msg.ePickType = 14    -- Interactive pick type
	end
  
    return msg      
end 

function onClientUse(self, msg)
	-- do nothing if the player is not in the correct mission state
	if msg.user:GetMissionState{missionID = misID}.missionState < 8 then return end
	
	-- do the event
	RunEvent(self, false)
end

----------------------------------------------
-- The zone control object says when the player is loaded
----------------------------------------------
function notifyPlayerReady(self, zoneObj, msg)
    -- get the player
    local player = GAMEOBJ:GetControlledID()
    -- custom function to preload necessary animations
    preloadAnims(self)
    -- cancel the notification request
    self:SendLuaNotificationCancel{requestTarget=player, messageName="PlayerReady"}
end

function onScopeChanged(self, msg)		
	if msg.bEnteredScope then 
		-- do nothing if the player is not in the correct mission state
		if GAMEOBJ:GetControlledID():GetMissionState{missionID = misID}.missionState >= 8 then return end
		
		-- see if the staff should be there or not
		checkHideEquip(self)
		
		-- obtain the local player object
        local player = GAMEOBJ:GetControlledID()
		if(not player) then
            -- subscribe to a zone control object notification alerting the script
            -- when the local player object is ready
            self:SendLuaNotificationRequest{requestTarget = GAMEOBJ:GetZoneControlID(), messageName="PlayerReady"}
            return
        end
		
		-- preload all the animations that are going to be played
		preloadAnims(self)
	else
		-- staff is not hidden
		self:SetVar("bHidden", false)
	end
end

function checkHideEquip(self)
	-- do nothingn if the staff is not hidden
	if self:GetVar("bHidden") then return end
	
	-- keep checking until we're visible
	if not self:GetVisible().visible then
		GAMEOBJ:GetTimer():AddTimerWithCancel(0.25, "check", self)
		
		return
	end	
	
	-- set timer to hide weapon
	self:SetVar("bHidden",true)	
	GAMEOBJ:GetTimer():AddTimerWithCancel(2, "hide", self)	
end

function preloadAnims(self)
	-- preload the anims in the preAnims table
	for key, tSet in ipairs(eventSet) do
		self:PreloadAnimation{animationID = tSet.anim, respondObjID = self}
	end
end

function onNotifyClientObject(self, msg)
	-- do nothing if this isn't the correct message
	if msg.name ~= "switch" then return end
	
	-- do the event
	RunEvent(self, true)
end 

function RunEvent(self, bTransition)
	-- request notification from the player when the cinematic is done
	self:SendLuaNotificationRequest{requestTarget = GAMEOBJ:GetZoneControlID(), messageName = "CinematicUpdate"}
    
    	local type = "ACTION_TYPE_ENABLE"
       
	-- play the cinematic, this message also hides any players and npcs nearby the target ID
	GAMEOBJ:GetControlledID():HandleInteractionCamera{wsCameraPath = cinName, cameraTargetID = self, actionType = type }	
end 

function notifyCinematicUpdate(self, zoneObj, msg)
	-- do nothing if the cinematic is not starting
	if msg.event ~= "STARTED" or msg.pathName ~= cinName then return end
	
	if self:GetVar("bHidden") then
		-- unhide weapon
		self:UnHideEquipedWeapon()
		self:SetVar("bHidden", false)
	end
	
	-- cancel the luanotification and start the event
	self:SendLuaNotificationCancel{requestTarget = GAMEOBJ:GetZoneControlID(), messageName="CinematicUpdate"}
	GAMEOBJ:GetTimer():AddTimerWithCancel(0.5, "startAnim", self)
end

function playAnims(self, animID)
	-- loop through the event table and play the next set
	for key,tSet in ipairs(eventSet) do
		-- do nothing if the animID is the last in the table
		if key == table.maxn(eventSet) then return end
		
		-- goto next key if this isn't the correct set
		if not animID or animID == tSet.anim then
			local newKey = key + 1
			local newSet = eventSet[newKey]
			
			if not animID then 
				-- this is the first time through so adjust the set
				newSet = tSet
			elseif eventSet[key].fx then
				-- the last time through played an FX so stop it
				self:StopFXEffect{name = "cast"..eventSet[key].fx}
			end
			
			if animID then
				if meshFX.start == newKey then
					-- this is the correct set to start the meshFX
					self:PlayFXEffect{name = "tornadoMesh", effectType = meshFX.fx, effectID = meshFX.fxID}			
				elseif meshFX.stop == newKey then
					-- this is the correct set to stop the meshFX
					self:StopFXEffect{name = "tornadoMesh"}	
				end
			end
			
			-- play this set anim
			self:PlayAnimation{animationID = newSet.anim, fPriority = 4.0, bPlayImmediate = true, bTriggerOnCompleteMsg = true}
			
			if newSet.fx ~= -1 then
				-- there is an fx for this set so play it
				self:PlayFXEffect{name = "cast"..newSet.fx, effectType = "cast", effectID = newSet.fx}
			end
			
			break
		end
	end
end

function onAnimationComplete(self, msg)	
	-- if we're the last animation in the table stop the cinematic
	if msg.animationID == eventSet[table.maxn(eventSet)].anim then
		GAMEOBJ:GetTimer():AddTimerWithCancel(1, "stopCinematic", self)
	else
		-- run the correct animation/fx
		playAnims(self, msg.animationID)
	end
end

function onTimerDone (self,msg)
	if msg.name == "check" then
		-- check if visible
		checkHideEquip(self)
	elseif msg.name == "hide" then
		-- hide weapon
		self:HideEquipedWeapon()
	elseif msg.name == "startAnim" then
		if self:GetCurrentAnimation().secondaryAnimationID ~= "" then
			GAMEOBJ:GetTimer():AddTimerWithCancel(0.5, "startAnim", self)
			
			return
		end
		
		-- start the anims
		playAnims(self, nil)
	elseif msg.name == "stopCinematic" then
		local player = GAMEOBJ:GetControlledID()
		
		-- event is over, clean up
		player:EndCinematic{pathName = cinName}
		player:TerminateInteraction{type = 'fromInteraction', ObjIDTerminator = self} 
	end
end 