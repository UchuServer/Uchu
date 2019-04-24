--------------------------------------------------------------
-- Script on the Binoculars
-- plays an animation, the cinematic and updates the achievement
-- 
-- updated abeechler ... 7/27/11 - refactored scripts and added onHit exit interact functionality
--------------------------------------------------------------
--To set up 
--	each binocular cinematic must be named "binoc_##"
--	on each binocular, config data must be set for "number" and "0:##"
--	## should corrispond with the binocular being used, and the cinematic you want to play
--	## must be unique for the achievments to work, and there must be an entry for the binoc in the player flag
--		database table if that binoc is to be used for the achievement

-- Entries in the player plag table
--		Should corraspond with the map number
--		so all the binocs in the spaceship should start at 1001,1002,1003,etc...
--			AG with 1101,1102,1103, etc...
------------------------------------------------------------------------

----------------------------------------------
-- Set default variable state
----------------------------------------------
function onStartup(self, msg)
	-- Catch binocular number for cinematic existence check		
	local number = self:GetVar('number') or 0
	-- Check for existence of an appropriate cinematic path 
    local cName = "binoc_" .. number  	        
    local cTime = tonumber(LEVEL:GetCinematicInfo(cName))
            
    if(cTime) then
        local cName = "binoc_" .. number  		
        -- Save the cinematic name for reference
        self:SetVar("cName", cName) 
    end
	
end

----------------------------------------------
-- Turning on the effects on the binoculars based on 
-- whether the player has looked through them before or not
----------------------------------------------
function onRenderComponentReady(self,msg) 
	-- Get the player
	local player = GAMEOBJ:GetControlledID()
	if(not player:Exists()) then
		-- Tell the zone control object to tell the script when the local player is loaded
		self:SendLuaNotificationRequest{requestTarget = GAMEOBJ:GetZoneControlID() , messageName="PlayerReady"}
		return
	end
	
	-- Custom function
	CheckFlags(self,player)

end

----------------------------------------------
-- The zone control object says the player is loaded
----------------------------------------------
function notifyPlayerReady(self, zoneObj, msg)
	-- Get the player
	local player = GAMEOBJ:GetControlledID()
	
	-- Custom function to see if the players flag is set
	CheckFlags(self,player)
	-- Cancel the notification request
	self:SendLuaNotificationCancel{requestTarget=player, messageName="PlayerReady"}
	
end

----------------------------------------------
-- Determine whether or not to play object alert effects
-- based on player flag state
----------------------------------------------
function CheckFlags(self,player)

	local map = LEVEL:GetCurrentZoneID()
	-- Get the number on the binoc config data from HF
	local number = self:GetVar('number') or false
	
	-- Make sure the script doesnt fail if the binoc doesnt have config data		
	if(number) then 
	    -- Make player flag number
		local flagNumber = tonumber(string.sub(map,0,2) .. number) 
		-- Skips the rest of the function if there is no map number
		if(map) then               
			if(player:GetFlag{iFlagID = flagNumber}.bFlag == false) then 					
				-- If the player flag is false, the player hasnt looked though the binocs before
				-- Turn on the binocular effect
				self:PlayFXEffect{name = "binocular_alert" , effectType = "cast"}					
			end  
		end
	end    
	
end

----------------------------------------------
-- Check to see if the player can use the binoculars
----------------------------------------------
function onCheckUseRequirements(self, msg) 
	-- Check to make sure the player isnt currently using the binoculars
	if self:GetVar("beingUsed")  then	
		msg.bCanUse = false
		return msg		
	end
	
end

----------------------------------------------
-- Called on successful client use
----------------------------------------------
function onClientUse(self,msg)	
    -- 'SetUserCtrlCompPause' crashs the game when used in this function, so i moved 
    -- everything to a onTimerDone function to avoid crashes - not ideal, but it works
    GAMEOBJ:GetTimer():AddTimerWithCancel(0.1, "startBinoc", self)
    self:SetVar("beingUsed", true)
    
    -- Listen for if the current user gets hit
    self:SendLuaNotificationRequest{requestTarget = msg.user, messageName = "OnHit"}
    
end

----------------------------------------------
-- Process server-sent messages
----------------------------------------------
function onFireEventClientSide(self,msg)
	if msg.args == "achieve" then	
	    -- Turn off the binocular effect
		self:StopFXEffect{name = "binocular_alert"}	
	end
	
end

----------------------------------------------
-- Process timer events
----------------------------------------------
function onTimerDone (self,msg)
    -- Obtain a reference to the player
    local player = GAMEOBJ:GetControlledID()
    
    if (msg.name == "startBinoc" ) then	
		local oDir = self:GetRotation()
		
		GAMEOBJ:GetTimer():AddTimerWithCancel( 0.5, "lookanimation",self )
		player:SetStunned{StateChangeType = "PUSH", bCantMove = true, bIgnoreImmunity = true, bCantTurn = true, bCantEquip = true}
		player:SetRotation{ x = oDir.x , y = oDir.y, z =oDir.z , w=oDir.w}
		-- Temp animation while we wait for an animation of the player looking through the binoculars	
		player:PlayAnimation{animationID = "binoculars-idle"} 
	
	elseif (msg.name == "lookanimation") then
	
	    local cName = self:GetVar("cName")
	    
	    if(cName) then
	        -- Play cinematic
            player:PlayCinematic {pathName = cName} 
            -- Request notification from the player when the cinematic is done
	        self:SendLuaNotificationRequest{requestTarget = GAMEOBJ:GetZoneControlID(), messageName = "CinematicUpdate"}
            
            UI:SendMessage("pushGameState", {{"state", "cinematic"}})
            -- Grab the sound set in the config data of the binoc in HF
		    local soundName = self:GetVar('sound')
		    -- Let the script run even if there isn't sound config data
		    if(soundName) then
                self:PlayNDAudioEmitter{m_NDAudioEventGUID = soundName} 
            end
            
        else
            -- Utility function to handle necessary binocular exit events
	        binocularExit(self)
	        
	    end
	    	
		player:SetStunned{StateChangeType = "POP", bCantMove = true, bIgnoreImmunity = true, bCantTurn = true, bCantEquip = true}
		
	end	
	
end

----------------------------------------------
-- Catch and process cinematic update events
----------------------------------------------
function notifyCinematicUpdate(self, zoneObj, msg)
    -- Get the currently desired playing binocular cinematic
    local cName = self:GetVar("cName")
    
	-- Do nothing if the cinematic is not ending
	if msg.event ~= "ENDED" or msg.pathName ~= cName then return end
	
	-- Utility function to handle necessary binocular exit events
	binocularExit(self)
	
end

---------------------------------------------
-- Sent when the object checks its pick type
----------------------------------------------
function notifyOnHit(self, player, msg)
    -- Handle event interrupt processing
    player:SetStunned{StateChangeType = "POP", bCantMove = true, bIgnoreImmunity = true, bCantTurn = true, bCantEquip = true}
    GAMEOBJ:GetTimer():CancelAllTimers(self)
    player:EndCinematic()
	
	-- Utility function to handle necessary binocular exit events
	binocularExit(self)

end

----------------------------------------------
-- Utility function used to handle interaction
-- exit processing
----------------------------------------------
function binocularExit(self)
    -- Obtain a reference to the player
    local player = GAMEOBJ:GetControlledID()
    
    -- Remove cinematic bars
	UI:SendMessage("popGameState", {{"state", "cinematic"}})
	-- Exit interaction
	player:TerminateInteraction{type = 'fromInteraction', ObjIDTerminator = self}
    -- Reset animation
	player:PlayAnimation{animationID = "ben_is_king"} 
	self:SetVar("beingUsed", false)
	
	-- Cancel the notification requests
	self:SendLuaNotificationCancel{requestTarget = player, messageName = "OnHit"}
	self:SendLuaNotificationCancel{requestTarget = GAMEOBJ:GetZoneControlID(), messageName="CinematicUpdate"}
	
end

----------------------------------------------
-- Sent when the object checks its pick type
----------------------------------------------
function onGetPriorityPickListType(self, msg)  
	if(self:GetVar("beingUsed")) then
		msg.ePickType = -1
    else
		local myPriority = 0.8
		
		if ( myPriority > msg.fCurrentPickTypePriority ) then    
			msg.fCurrentPickTypePriority = myPriority 
 
			msg.ePickType = 14    -- Interactive pick type
		end
    end  
  
    return msg
      
end 
