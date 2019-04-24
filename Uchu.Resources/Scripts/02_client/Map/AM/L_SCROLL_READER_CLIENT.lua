--------------------------------------------------------------
-- client script on the scroll reader in the temple in aura mar
-- player has a mission to interact with the scroll reader 
-- scroll reader allows the player to page through 10 pages of ninjago history
--
-- brandi 10/28/2010 
-- updated mrb... 12/14/10 -- updated GUI messages and fixed icons
-- updated abeechler... 1/21/10 -- fixed UI call typo
--------------------------------------------------------------

-- leaving this in because VFX will probably want to add fx to this object
-------------------------------------------
-- turning on the effects on the binoculars based on whether the player has looked through 
-- them before or not
-------------------------------------------
-- function onRenderComponentReady(self,msg)	
	-- local player = GAMEOBJ:GetControlledID()
	-- -- make sure the player is ready
    -- if player:Exists() then
		-- if self:GetVar('storyText') then --make sure the script doesnt fail if the binoc doesnt have config data		
            -- local flagNumber = self:GetVar('altFlagID') or (10000 + LEVEL:GetCurrentZoneID() + tonumber(string.sub(self:GetVar('storyText'), -2))) --make player flag number
                           
			-- if (player:GetFlag{iFlagID = flagNumber}.bFlag == false) then 				
				-- --if the player flag is false, the player hasnt looked though the binocs before
				-- -- turn on the binocular effect
				-- self:PlayFXEffect{ name = "plaque_attract" , effectType = "attract" }			
			-- else			
				-- self:PlayFXEffect{ name = "plaquefx" , effectType = "display" }				
			-- end  
		-- end
	-- else 
		-- -- if the player isnt fully loaded to check the player flag status, create 'heartbeat timer'
		-- GAMEOBJ:GetTimer():AddTimerWithCancel(1.0, "CheckPlayer", self)
	-- end
-- end

-------------------------------------------
-- see if the plaque is in use before allowing the player from using it again
-------------------------------------------
function onCheckUseRequirements(self, msg)
	local player = msg.objIDUser
	
	if not player:Exists() then return end
	--check to make sure the player has at least accepted the  mission to the use the scroll reader
	if player:GetMissionState{missionID = 969}.missionState < 2 then 		
		msg.HasReasonFromScript = true  
		msg.Script_IconID = 4733  
		msg.Script_Reason = Localize("CP_TALK_TO_SENSEI_WU")
		msg.Script_Failed_Requirement = true  
		msg.bCanUse = false  
		
		return msg	
	end
end

----------------------------------------------
-- sent when the local player interacts with the
-- object
----------------------------------------------
function onClientUse(self, msg) 
    local player = GAMEOBJ:GetControlledID()
    -- check to see if we are the correct player
    if player:GetID() ~= msg.user:GetID() then return end
    
	if player:Exists() then
	    UI:SendMessage( "ToggleStoryBox", {{"visible", false }} ) 
		-- tell the Story Box UI element to open and what to display, then turn off the interaction icon		
		UI:SendMessage("pushGameState", {{"state", "Story"}, {"context", getContext(self, player)} })
		toggleActivatorIcon(self, true)
	end   
end 

----------------------------------------------
-- checks to see if there is config data set in
-- HF, if not this returns the default message
----------------------------------------------
function getContext(self, player)
	-- get the variables from HF or use the default for CP scroll reader
	local numOfScrolls = self:GetVar("TotalScrolls") or 10
	local scrollName = self:GetVar("ScrollName") or "STORY_BOX_NINJAGOSCROLL_"
    local textVar = {{"bScroll", true}, {"visible", true }, {"senderID", player}, {"callbackObj", self}}
    
    -- create and 
    for i = 1, numOfScrolls do
		-- set the scroll num
		local scrollNum = i
		-- ui uses 0 as first value so - 1
		local uiNum = i - 1
		
		-- format scrollNum if needed for 01, 02 localization formatting
		if scrollNum < 10 then
			scrollNum = "0"..scrollNum
		end
		
		-- insert the correctly formatted values, PAGE0 = localization key
		table.insert(textVar, {"PAGE"..uiNum, Localize(scrollName..scrollNum)})
	end
    
    return textVar
end

-----------------------------------------------
-- message sent from the server when the player flag is set
-----------------------------------------------
--function onFireEventClientSide(self,msg)
--	if msg.args == "achieve" then	
--		self:StopFXEffect{name = "plaque_attract" }
--		self:PlayFXEffect{ name = "plaquefx" , effectType = "display" }	
--	end
--end

-----------------------------------------------
-- called when the player closes the story plaque
-----------------------------------------------
function onTerminateInteraction(self,msg)
    -- player was hit close the UI element and turn on the icon
    UI:SendMessage( "ToggleStoryBox", {{"visible", false }} )   
    toggleActivatorIcon(self, false, true)
end

----------------------------------------------
-- sent when the object story box is closed;
-- this can be done by hitting the x, esc or enter
----------------------------------------------
function onMessageBoxRespond(self, msg)
	if msg.identifier == "Close" then
		-- UI element has been closed turn on the icon
		toggleActivatorIcon(self)
	end
end

----------------------------------------------
-- sent when the object checks it's pick type
----------------------------------------------
function onGetPriorityPickListType(self, msg)  
    local myPriority = 0.8
  
    if ( myPriority > msg.fCurrentPickTypePriority ) then    
        msg.fCurrentPickTypePriority = myPriority 
 
        if self:GetVar('isInUse') then
            msg.ePickType = -1
        else
            msg.ePickType = 14    -- Interactive pick type     
        end
    end  
  
    return msg      
end 

----------------------------------------------
-- sent when the requested local player is hit 
-- by something, this is used to close the story box
----------------------------------------------
function notifyOnHit(self, other, msg)
    -- player was hit close the UI element and turn on the icon
    UI:SendMessage( "ToggleStoryBox", {{"visible", false }} )   
    toggleActivatorIcon(self)
end

----------------------------------------------
-- toggles the activator Icon based on bHide, 
-- to toggle it on you dont have to pass bHide
----------------------------------------------
function toggleActivatorIcon(self, bHide, bFromTerminate)
    local player = GAMEOBJ:GetControlledID()
    
    if not bHide then -- show the icon, cancel notification, set isInUse to false
        self:SendLuaNotificationCancel{requestTarget=player, messageName="OnHit"}        
        self:SetVar('isInUse', false)
        
		if not bFromTerminate then
			player:TerminateInteraction{type = 'fromInteraction', ObjIDTerminator = self}   
		end     
    else -- hide the icon, request notification, set isInUse to true
        self:SetVar('isInUse', true)
        self:SendLuaNotificationRequest{requestTarget=player, messageName="OnHit"}
    end
    
    -- request the interaction update
    self:RequestPickTypeUpdate()
end 

