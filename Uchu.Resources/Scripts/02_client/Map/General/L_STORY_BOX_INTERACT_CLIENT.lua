--------------------------------------------------------------
-- Generic Story Box Interaction script, opens/closes the story
-- box UI based on the config data set in HF on the object running
-- this script. 
-- updated mrb... 5/04/10 -- added altFlagID to fix PC issue
-- updated brandi... 10/21/10 -- pulled set flag out this script and created a server script
--------------------------------------------------------------

-- ***********************************************************
-- HF config data format
-- storyText -> 0:stringName    -- needed to work
-- altFlagID -> 1:flagID        -- if the flag is different from 10000 + mapNum + storyText number use this
-- ***********************************************************

-------------------------------------------
-- turning on the effects on the binoculars based on whether the player has looked through 
-- them before or not
-------------------------------------------
function onRenderComponentReady(self,msg)	
	-- get the player
	local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
	if not player:Exists() then 
		-- tell the zone control object to tell the script when the local player is loaded
		self:SendLuaNotificationRequest{requestTarget = GAMEOBJ:GetZoneControlID() , messageName="PlayerReady"}
		return
	end
	-- custom function
	CheckFlags(self,player)
end

-- the zone control object says the player is loaded
function notifyPlayerReady(self,zoneObj,msg)
	-- get the player
	local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
	
	if not player:Exists() then return end
	-- custom function to see if the players flag is set
	CheckFlags(self,player)
	-- cancel the notification request
	self:SendLuaNotificationCancel{requestTarget=player, messageName="PlayerReady"}
end

function CheckFlags(self,player)
	if self:GetVar('storyText') then --make sure the script doesnt fail if the binoc doesnt have config data		
		local flagNumber = self:GetVar('altFlagID') or (10000 + LEVEL:GetCurrentZoneID() + tonumber(string.sub(self:GetVar('storyText'), -2))) --make player flag number
					   
		if (player:GetFlag{iFlagID = flagNumber}.bFlag == false) then 				
			--if the player flag is false, the player hasnt looked though the binocs before
			-- turn on the binocular effect
			self:PlayFXEffect{ name = "plaque_attract" , effectType = "attract" }			
		else			
			self:PlayFXEffect{ name = "plaquefx" , effectType = "display" }				
		end  
	end
end

-------------------------------------------
-- see if the plaque is in use before allowing the player from using it again
-------------------------------------------
function onCheckUseRequirements(self, msg) 
	--check to make sure the player isnt currently using the binoculars
	if self:GetVar("isInUse")  then	
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
    if player:GetID() ~= msg.user:GetID() or self:GetVar('isInUse') then return end
    
	if player:Exists() then
		-- tell the Story Box UI element to open and what to display, then turn off the interaction icon
		UI:SendMessage("pushGameState", {{"state", "Story"}, {"context", {{"visible", true }, {"text", getText(self) }, {"senderID", player}, {"callbackObj", self}}} })
		toggleActivatorIcon(self, true)
		player:UsedInformationPlaque{i64Plaque = self}
	end   
end 
-----------------------------------------------
-- message sent from the server when the player flag is set
-----------------------------------------------
function onFireEventClientSide(self,msg)
	if msg.args == "achieve" then	
		self:StopFXEffect{name = "plaque_attract" }
		self:PlayFXEffect{ name = "plaquefx" , effectType = "display" }	
	end
end

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
    -- UI element has been closed turn on the icon
    toggleActivatorIcon(self)
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
-- checks to see if there is config data set in
-- HF, if not this returns the default message
----------------------------------------------
function getText(self)
    local textVar = self:GetVar('storyText')
    
    -- default story box text message, tells the developer how to put in the localization string
    if not textVar then
        return [[Missing story text, set the correctly localization string in HF configdata.

Format = storyText -> 0:stringName]]
    end
    
    return Localize(textVar)
end


----------------------------------------------
-- toggles the activator Icon based on bHide, 
-- to toggle it on you dont have to pass bHide
----------------------------------------------
function toggleActivatorIcon(self, bHide, bFromTerminate)
    local player = GAMEOBJ:GetControlledID()
    
    if not bHide then -- show the icon, cancel notification, set isInUse to false
        bHide = false
        self:SetVar('isInUse', false)
        self:SendLuaNotificationCancel{requestTarget=player, messageName="OnHit"}
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

function onTimerDone (self,msg)
	if (msg.name == "CheckPlayer") then
	    onRenderComponentReady(self,msg)
	end
end