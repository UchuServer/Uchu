require('o_mis')

--------------------------------------------------------------
-- sets the mouse over distance for interactions
--------------------------------------------------------------
function SetMouseOverDistance(self, dist)

    if (self and self:Exists()) then
    
        self:SetVar("interactDistance", dist)
    
    end

end


--------------------------------------------------------------
-- sets the proximity for the object
--------------------------------------------------------------
function SetProximityDistance(self, dist)

    if (self and self:Exists()) then
    
        self:SetVar("proxDistance", dist)
        self:SetProximityRadius{radius = self:GetVar("proxDistance"), name="chatBubbleProx"}	
    
    end

end

--------------------------------------------------------------
-- check for valid type
--------------------------------------------------------------
function IsValidType(type)

    if (type == "mouseOverEffect" or
        type == "mouseOverAnim" or
        type == "mouseOverText" or
        type == "interactionEffect" or
        type == "interactionAnim" or
        type == "interactionText" or
        type == "proximityEffect" or
        type == "proximityAnim" or
        type == "proximityText") then
        
        return true;
    end
    
    return false
    
end


--------------------------------------------------------------
-- adds an interaction, possible types include:
--------------------------------------------------------------
-- mouseOverEffect, mouseOverAnim, mouseOverText, 
-- interactionEffect, interactionAnim, interactionText, 
-- proximityEffect, proximityAnim, proximityText, 
--------------------------------------------------------------
function AddInteraction(self, type, action)

    if (self and self:Exists()) then
    
        -- check type
        if (IsValidType(type) == false) then
            print("Bad Type of Interaction")
            return
        end
        
        local table = self:GetVar(type)
        
        -- init table if need to
        if (table == nil) then
            table = {}
        end
        
        local num = #table + 1
        table[num] = action
        
        self:SetVar(type, table)
        
    end        

end


--------------------------------------------------------------
-- override pick type to be interactive
--------------------------------------------------------------
function onGetPriorityPickListType(self, msg)  

	local isVendor = self:HasComponentType{iComponent = 16}.bHasComponent
	if isVendor then return end

	-- if the npc is going to open a mission dialogue, this skips the ambient interact
	if self:GetMissionForPlayer{playerID = GAMEOBJ:GetControlledID()}.missionState ~= 0 then return end
	
    local myPriority = 0.8
  
    if ( myPriority > msg.fCurrentPickTypePriority ) then    
        msg.fCurrentPickTypePriority = myPriority 
 
		if self:GetMissionForPlayer{playerID = msg.user}.missionState ~= 0 then	
			msg.ePickType = 14  
		elseif self:GetVar('isInUse') or 
			not (  self:GetVar("interactionEffect") and  self:GetVar("interactionAnim")  and self:GetVar("interactionText") ) then
			msg.ePickType = -1
		else
			msg.ePickType = 14    -- Interactive pick type   
				
		end
    end  
  
    return msg      
end 


--------------------------------------------------------------
-- handle cursor over object
--------------------------------------------------------------
function onCursorOn(self, msg)

    -- do effects
	local effects = self:GetVar("mouseOverEffect")
	if effects and #effects > 0 and (msg.distance < self:GetVar("interactDistance")) then
		-- get a random effect
		local num = math.random(1, #effects)
		self:PlayFXEffect{effectType = effects[num]}
	end
		
    -- do anims
	local anims = self:GetVar("mouseOverAnim")
	if anims and #anims > 0 and (msg.distance < self:GetVar("interactDistance")) then
		-- get a random animation
		local num = math.random(1, #anims)
		self:PlayAnimation{animationID = anims[num]}
	end

    -- do effects
	local texts = self:GetVar("mouseOverText")
	if texts and #texts > 0 and (msg.distance < self:GetVar("interactDistance")) then
		-- get a random text
		local num = math.random(1, #texts)
		self:DisplayChatBubble{wsText = texts[num]}
	end
		
end


--------------------------------------------------------------
-- handle client use
--------------------------------------------------------------
function onClientUse(self, msg)
	
	local isVendor = self:HasComponentType{iComponent = 16}.bHasComponent
	if isVendor then return end
	
	-- if the npc is going to open a mission dialogue, this skips the ambient interact
	if self:GetMissionForPlayer{playerID = msg.user}.missionState ~= 0 then	return end
	
	-- we need to add this to keep this script from sending a packet to the server everytime a chat bubble pops up
	self:SetVar('isInUse', true) 
	GAMEOBJ:GetTimer():AddTimerWithCancel( 5 , "talking", self )
	msg.user:PlayAnimation{animationID = "talk", bPlayImmediate = true}
	
    -- do effects
	local effects = self:GetVar("interactionEffect")
	if effects and #effects > 0 then
		-- get a random effect
		local num = math.random(1, #effects)
		self:PlayFXEffect{effectType = effects[num]}
	end
		
    -- do anims
	local anims = self:GetVar("interactionAnim")
	if anims and #anims > 0 then
		-- get a random animation
		local num = math.random(1, #anims)
		self:PlayAnimation{animationID = anims[num]}
	else
		-- check to see if the npc has a default talk animation, if not, prints out a warning
		if self:GetAnimationTime{animationID = "talk"}.time == 0 and self:GetVersioningInfo().bIsInternal then
			print("***WARNING*** From script client/ai/AG/L_AG_NPC.lua This npc needs a talk animation. My lot is "..self:GetLOT().objtemplate)
		end
		self:PlayAnimation{animationID = "talk", bPlayImmediate = true}
	end

    -- do effects
	local texts = self:GetVar("interactionText")
	if texts and #texts > 0 then
		-- get a random text
		local num = math.random(1, #texts)
		self:DisplayChatBubble{wsText = texts[num]}
	end
	
end


--------------------------------------------------------------
-- handle proximity updates
--------------------------------------------------------------
function onProximityUpdate(self, msg)
	local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID()) 
	
	-- if the npc is going to open a mission dialogue, this skips the ambient interact
	if self:GetMissionForPlayer{playerID = player}.missionState ~= 0 then	return end
	
    if ( (player) and (player:Exists()) ) then
		if msg.status == "ENTER" and msg.objId:GetID() == player:GetID() and msg.name == "chatBubbleProx" then
		
            -- do effects
            local effects = self:GetVar("proximityEffect")
            if effects and #effects > 0 then
                -- get a random effect
                local num = math.random(1, #effects)
                self:PlayFXEffect{effectType = effects[num]}
            end
                
            -- do anims
            local anims = self:GetVar("proximityAnim")
            if anims and #anims > 0 then
                -- get a random animation
                local num = math.random(1, #anims)
                self:PlayAnimation{animationID = anims[num]}
            end

            -- do effects
            local texts = self:GetVar("proximityText")
            if texts and #texts > 0 then
                -- get a random text
                local num = math.random(1, #texts)
                self:DisplayChatBubble{wsText = texts[num]}
            end		
		
		end
		
	end
end


function onTimerDone(self, msg)

	if (msg.name == "talking") then 
		self:SetVar('isInUse', false)
		self:RequestPickTypeUpdate()
		local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
		player:TerminateInteraction{type = 'fromInteraction', ObjIDTerminator = self} 
	end
end
