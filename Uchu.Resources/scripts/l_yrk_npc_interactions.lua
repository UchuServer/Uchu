require('o_mis')



-- note: scripts that include this are responsibe for initializing 
	-- bUseInteractions and setting their own pick type


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
        self:SetProximityRadius{radius = self:GetVar("proxDistance")}	
    
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
-- handle cursor over object
--------------------------------------------------------------
function onCursorOn(self, msg)

	if ( UseInteractions( self ) == false ) then
		return
	end

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

	if ( UseInteractions( self ) == false ) then
		return
	end

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

	if ( UseInteractions( self ) == false ) then
		return
	end
	
	local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())  
    if ( (player) and (player:Exists()) ) then
		if msg.status == "ENTER" and msg.objId:GetID() == player:GetID() then
		
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




--------------------------------------------------------------
-- returns whether or not the player is allowed to interact with this object right now
--------------------------------------------------------------
function UseInteractions( self )

	-- if the flag has never been set, initialize it to true
	if ( self:GetVar( "bUseInteractions" ) == nil ) then
		self:SetVar( "bUseInteractions", true )
	end
	
	return self:GetVar( "bUseInteractions" )
end



--------------------------------------------------------------
-- allow the player to interact with this object
--------------------------------------------------------------
function ActivateInteractions( self )

	self:SetVar( "bUseInteractions", true )
	self:SetPickType{ePickType = 14}	-- PICK_LIST_INTERACTIVE
										-- from enum PICK_LIST_TYPE in lwoCommonVars.h
end




--------------------------------------------------------------
-- don't allow the player to interact with this object
--------------------------------------------------------------
function DeactivateInteractions( self )

	self:SetVar( "bUseInteractions", false )
	self:SetPickType{ePickType = 0}	-- PICK_LIST_GENERIC
									-- from enum PICK_LIST_TYPE in lwoCommonVars.h
end
