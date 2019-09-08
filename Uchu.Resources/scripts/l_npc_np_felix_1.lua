--------------------------------------------------------------
-- (CLIENT SIDE) Friendly Felix in Scene 1
--
-- On proximity of the local character, will approach and 
-- present help screen information for playing the game.
-- When the player has finished reading the help, he will
-- leave and despawn.
--------------------------------------------------------------

--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('o_mis')
require('c_NimbusPark')

--------------------------------------------------------------
-- Startup of the object
--------------------------------------------------------------
function onStartup(self) 
	self:SetVar("bHelpShown", false)
    self:SetVar("bFollow", false)
end


--------------------------------------------------------------
-- Called when rendering is complete for this object
--------------------------------------------------------------
function onRenderComponentReady(self, msg) 

	self:SetProximityRadius { radius = CONSTANTS["FELIX_1_PROX_RADIUS"] }

end


--------------------------------------------------------------
-- Called when an entity gets within proximity of the object
--------------------------------------------------------------
function onProximityUpdate(self, msg)

	-- if the local player is close enough to us
	if (msg.status == "ENTER") and 
       (msg.objId:GetID() == GAMEOBJ:GetLocalCharID()) and 
       (self:GetVar("bFollow") == false) then

        -- set flag to prevent re-following
        self:SetVar("bFollow", true)

        -- get the local character
        local player = msg.objId

        -- follow the player
        local mySpeed = self:GetSpeed().speed
        self:FollowTarget
        { 
            targetID = player, 
            radius = CONSTANTS["FELIX_1_INTERACT_RADIUS"], 
            speed = mySpeed, 
            keepFollowing = false 
        }
        
        -- store the player in felix object to face on arrival
        storeObjectByName(self, "playerTarget", player)

	elseif (msg.status == "LEAVE") and (msg.objId:GetID() == GAMEOBJ:GetLocalCharID()) then
	
		-- @TODO: anything here? Despawn?

	end

end


--------------------------------------------------------------
-- Called when object reaches destination
--------------------------------------------------------------
function onArrived(self, msg)

    -- done following target
    if (msg.pathType == "FollowTarget") then
    
        ShowBigHelp(self)
    
    -- are we here because of a path?
    elseif (msg.pathType == "Waypoint") then
        
        if (msg.isLastPoint == true) then
            -- make felix leave/despawn, next frame
            self:SetVisible{ visible = false, fadeTime = 1.0 }
            GAMEOBJ:GetTimer():AddTimerWithCancel( 2.0, "Despawn",self )
        else
            self:ContinueWaypoints()
        end

    end
    
end


--------------------------------------------------------------
-- Sent from a player when responding from a messagebox
--------------------------------------------------------------
function onMessageBoxRespond(self, msg)

    -- make sure this was from the local character
	if (GAMEOBJ:GetLocalCharID() == msg.sender:GetID()) then
    
        -- flag the tooltip bit
        msg.sender:SetTooltipFlag{ iToolTip = CONSTANTS["PLAYER_FELIX_1_FLAG_BIT"], bFlag = true }

        -- unpause player
        msg.sender:SetUserCtrlCompPause{bPaused = false}
        
        -- set felix on his way
        self:SetVar("attached_path", CONSTANTS["FELIX_1_FLEE_PATHNAME"])
        self:SetVar("attached_path_start", 0)
        self:FollowWaypoints()
        local mySpeed = self:GetSpeed().speed
        self:SetPathingSpeed{ speed = mySpeed }
        
        self:DisplayChatBubble{ wsText = CONSTANTS["FELIX_1_FLEE_TEXT"] }

	end

end


--------------------------------------------------------------
-- Handle notifications
--------------------------------------------------------------
function onNotifyObject(self, msg)

    if (msg.name == "ForceHelp") then
        ShowBigHelp(self)
    end

end


--------------------------------------------------------------
-- Timers
--------------------------------------------------------------
onTimerDone = function(self, msg)
    
    if msg.name == "Despawn" then 
		GAMEOBJ:GetTimer():CancelAllTimers( self )
		GAMEOBJ:DeleteObject(self)
    end  
        
end

--------------------------------------------------------------
-- Try to show the big help
--------------------------------------------------------------
function ShowBigHelp(self)

    if (self:GetVar("bHelpShown") == false) then

        -- set flag
        self:SetVar("bHelpShown", true)
        
        -- get the player i'm following
        local player = getObjectByName(self, "playerTarget")
        
        if (player) and (player:Exists()) then

            -- show help
            
     
            -- pause player
            player:SetUserCtrlCompPause{bPaused = true}
           
            -- face the player
            self:FaceTarget{ target = player, degreesOff = 5, keepFacingTarget = false }
            
        end

    end

end





