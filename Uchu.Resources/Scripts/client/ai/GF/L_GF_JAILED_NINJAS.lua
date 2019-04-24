--------------------------------------------------------------
-- Client Script on the ninjas in Gnarled Forest
-- this script sets the icons and picktype of the ninjas depending on mission the player is on 

-- updated Brandi... 2/22/10
-- updated brandi 8/2/10 - undid the rotation of the spawned in dummy
-- updated brandi 8/4/10 - added teleport animation to ninjas
--------------------------------------------------------------

--missions on ninjas

local feedMis = {LOT_6732 = 386,LOT_6733 = 387,LOT_6734 = 388,LOT_6736 = 390}
local freeMis = {LOT_6732 = 701,LOT_6733 = 702,LOT_6734 = 703,LOT_6736 = 704}

----------------------------------------------
-- When the render on Rutger is loaded for the client
----------------------------------------------

function onStartup(self)

	self:SetOverheadIconOffset{depthOffset = 3}

end

function onScopeChanged(self,msg)
	-- if the player entered ghosting range
    if msg.bEnteredScope then  
	-- get the player
		local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
		if not player:Exists() then 
			-- tell the zone control object to tell the script when the local player is loaded
			self:SendLuaNotificationRequest{requestTarget = GAMEOBJ:GetZoneControlID() , messageName="PlayerReady"}
			return
		end
		-- custom function
		CheckMissions(self,player)
	end
end

-- the zone control object says the player is loaded
function notifyPlayerReady(self,zoneObj,msg)
	-- get the player
	local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
	
	if not player:Exists() then return end
	-- custom function to see if the players flag is set
	CheckMissions(self,player)
	-- cancel the notification request
	self:SendLuaNotificationCancel{requestTarget=player, messageName="PlayerReady"}
end

function CheckMissions(self,player)
	-- check to see if the player has completed the free the ninja mission, and if so, replace the ninja with a dummy
	if player:GetMissionState{missionID = freeMis["LOT_"..self:GetLOT().objtemplate]}.missionState == 8 then
		
		HideNinja(self)
		
	end
end 

function onProximityUpdate(self,msg)

    if msg.status == "ENTER" and msg.name == "conductRadius" then

		local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
	
		if player:GetID() == msg.objId:GetID() then	
		
			local checkFeed = player:GetMissionState{missionID = feedMis["LOT_"..self:GetLOT().objtemplate]}.missionState
			local checkFree = player:GetMissionState{missionID = freeMis["LOT_"..self:GetLOT().objtemplate]}.missionState
			local check705 = player:GetMissionState{missionID = 705}.missionState
			
			-- if the player hasn't started the jailkeep missions
			if checkFeed < 2 or (checkFeed == 8 and checkFree < 2) then
				
				self:DisplayChatBubble{wsText = Localize("GF_NINJA_TALK_TO_JAILKEEP")}				
			
			-- if the player is done with the jailkeep missions, but hasnt started the free the ninjas missions
			elseif checkFeed > 4 and checkFree < 2 and check705 < 2 then
					
				self:DisplayChatBubble{wsText = Localize("GF_NINJA_GO_TO_FV")}
			
			-- if the player is on the jailkeep missions, but hasn't broken the jailcell door down
			elseif checkFree < 6 and check705 > 1 and player:GetMissionState{missionID = 385}.missionState == 8 and not self:GetVar("doorBusted")  then
				
				self:DisplayChatBubble{wsText = Localize("GF_NINJA_BREAK_DOOR_DOWN")}				
		
			end
			
		end
	end
end


function onCheckUseRequirements(self,msg)

	msg.preventRequirementsIcon = true
	local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
	
	if player:GetID() == msg.objIDUser:GetID() then	
	
	    local checkFeed = player:GetMissionState{missionID = feedMis["LOT_"..self:GetLOT().objtemplate]}.missionState
	    local checkFree = player:GetMissionState{missionID = freeMis["LOT_"..self:GetLOT().objtemplate]}.missionState
	    local check705 = player:GetMissionState{missionID = 705}.missionState
		
		-- if the player hasn't started the jailkeep missions
		if checkFeed < 2 or (checkFeed == 8 and checkFree < 2) then
			
			msg.bCanUse = false
			return msg
		
		-- if the player is done with the jailkeep missions, but hasnt started the free the ninjas missions
		elseif checkFeed > 4 and checkFree < 2 and check705 < 2 then
				
			msg.bCanUse = false
			return msg
		
		-- if the player is on the jailkeep missions, but hasn't broken the jailcell door down
		elseif checkFree < 6 and check705 > 1 and player:GetMissionState{missionID = 385}.missionState == 8 and not self:GetVar("doorBusted")  then
			
			msg.bCanUse = false
			return msg
	
		end
		
	end

end

function onClientUse(self,msg)

	--sends a message to the jail door to cancel the smash door timer so the door doesnt get rebuilt while the player is interacting with the missiongiver
	local group = "JailCell0"..self:GetVar("number")
	local door = self:GetObjectsInGroup{ group = group, ignoreSpawners = true }.objects[1]
		
	door:NotifyObject{name = "CancelTimer"}

end

function onGetInteractionInfo(self, msg)

	-- Purposely empty.

end

function onMissionDialogueOK(self,msg)
	
	-- if the player is completing a mission
	if  msg.iMissionState == 4 then
	
		-- check to see if the mission the player is completing is a feed me mission, if it is, hide their icons and skip the rest
		if msg.missionID == feedMis["LOT_"..self:GetLOT().objtemplate] then
			
			self:SetVar("doorBusted", false)
			GAMEOBJ:GetTimer():AddTimerWithCancel(5, "TellDoorToUnSmash", self )
			
			return
			
		end
			
		-- check to see if the mission is a free me mission
		if msg.missionID == freeMis["LOT_"..self:GetLOT().objtemplate] then
		
			--timer to let the ninja teleport effect play out
			GAMEOBJ:GetTimer():AddTimerWithCancel(2, "freeNinjas", self )
			
		end	
				
		
	end
end

function onMissionDialogueCancelled(self,msg)

	-- if the player canceled their mission, tell the door to unsmash
	GAMEOBJ:GetTimer():AddTimerWithCancel(5, "TellDoorToUnSmash", self )
	
end

function onFireEventClientSide(self, msg)

	-- this is broken, because fire event gets sent to everyone ones client script
	-- events sent from the script on the jail keep and the 'free the ninjas' ninja
 
	if msg.args == 'doorbusted' then
	
		self:SetVar("doorBusted", true)
		
	elseif msg.args == "doorRebuilt" then
	
		self:SetVar("doorBusted", false)
		
	end
end 

function HideNinja(self)

	local mypos = self:GetPosition().pos 
	local myRot = self:GetRotation()
	
	self:SetVisible{visible = false, fadeTime = 0}
	
	local config = { { "groupID" , "dummy"..self:GetVar("number") } , { "renderCullingGroup" , "3" } }
	--this will be taken out on real script when i have the real dummy asset
	RESMGR:LoadObject { objectTemplate = 8565 , x = mypos.x , y = mypos.y+0.5 , z = mypos.z ,rw= myRot.w, rx= myRot.x, ry= myRot.y, rz = myRot.z,owner = self, bIsSmashable = false , configData = config}
	
	

end


function onTimerDone(self, msg)


	-- set the ninja as invisible, spawn in the dummy ninja and cancel the timer on the door to reset
	if msg.name == "freeNinjas" then
			self:SetVar("doorBusted", false)
			self:PlayFXEffect{name = 'teleport', effectID = 98, effectType = "teleport"}
			self:PlayAnimation{ animationID = "brandi-is-queen" }
			GAMEOBJ:GetTimer():AddTimerWithCancel(self:GetAnimationTime{  animationID = "brandi-is-queen" }.time - 1.5, "HideNinja", self )
	end
	if msg.name == "HideNinja" then
		HideNinja(self)
		
		local group = "JailCell0"..self:GetVar("number")
		local door = self:GetObjectsInGroup{ group = group, ignoreSpawners = true }.objects[1]
		
		door:NotifyObject{name = "CancelTimer"}
		GAMEOBJ:GetTimer():AddTimerWithCancel(15, "TellDoorToUnSmash", self )
		
	end
	
	-- set the ninja as visible, and kill the dummy ninja 
	if (msg.name == "TellDoorToUnSmash") then
			
		local group = "JailCell0"..self:GetVar("number")
		local door = self:GetObjectsInGroup{ group = group, ignoreSpawners = true }.objects[1]
		
		door:NotifyObject{name = "unSmash"}
		
	end
	
	if (msg.name == "NoIcon") then
	
	    local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
		local state = self:GetMissionForPlayer{playerID = player}.missionState
				
		if ( state == 1 ) then
			--print("Icon OFF: " .. tostring(state))
			self:SetIconAboveHead{iconType = state, interactType = 0, bIconOff = true}
		end
		
	end

end

function onSetIconAboveHead(self, msg)

	if ( msg.iconType < 12 and not msg.bIconOff ) then

		--print("NEW ICON LOADED: " .. msg.iconType .. "Object " .. self:GetID())
		GAMEOBJ:GetTimer():AddTimerWithCancel(0.11, "NoIcon", self )
		
	end
	
end
