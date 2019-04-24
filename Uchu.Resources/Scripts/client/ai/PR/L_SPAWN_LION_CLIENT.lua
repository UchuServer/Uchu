--------------------------------------------------------------
-- Client side script on the object to spawn the lion pet
-- this script controls the interactablity of the object

-- created by Brandi... 2/11/10
--------------------------------------------------------------

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
		self:RequestPickTypeUpdate()
	end
end

-- the zone control object says the player is loaded
function notifyPlayerReady(self,zoneObj,msg)
	-- get the player
	local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
	
	if not player:Exists() then return end
	
	-- custom function to see if the players flag is set
	self:RequestPickTypeUpdate()
	-- cancel the notification request
	self:SendLuaNotificationCancel{requestTarget=player, messageName="PlayerReady"}
end

function onGetPriorityPickListType(self, msg)

	local myPriority = 0.8
	local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
	
    if ( myPriority > msg.fCurrentPickTypePriority ) then
	
		msg.fCurrentPickTypePriority = myPriority
		
		--check to see if the player has the flag set to spawn the lion
		if player:Exists() then
		
			if player:GetFlag{iFlagID = 67}.bFlag == true then --and #lions < 1 then
			
				msg.ePickType = 14 
				-- Interactive pick type 
				
			end 
			
		else
		
			msg.ePickType = -1
			
		end

    end
	
	--set the picktype on the object for the check within the timer to make sure not to run the script unnecessarily
	self:SetVar{"picktype",msg.ePickType}
	
    return msg
	

end

function onFireEventClientSide(self,msg)

	--display popup window letting the player know they can't spawn another lion
	--two messages, one saying the player cant spawn more than one lion at a time and one saying that there are too many lions spawned
	--right now both messages are the same, but eventually they should be different
	local player =  GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
	
	if msg.args == "playerLion" then
	
		if player:GetID() == msg.senderID:GetID() then
		
			--print("You already spawned a lion")
			player:DisplayTooltip{ bShow = true, strText = Localize("LION_SUMMON_FAIL"), iTime = 300 }
			
		end
		
	end
	
	if msg.args == "tooManyLions" then
	
		if player:GetID() == msg.senderID:GetID() then
		
			--print("There are too many lions right now, try again in a few minutes")
			player:DisplayTooltip{ bShow = true, strText = Localize("LION_SUMMON_FAIL"), iTime = 300 }
			
		end
		
	end
	
end

