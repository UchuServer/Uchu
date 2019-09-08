--------------------------------------------------------------
-- Client script for single lamp posts. Handles their effect
-- based on the state of the zone
--------------------------------------------------------------

--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('o_mis')
require('c_Zorillo')


--------------------------------------------------------------
-- Get the state of the zone
--------------------------------------------------------------
function GetZoneState(self)

    return self:GetVar("ZoneState")
    
end


--------------------------------------------------------------
-- Set the state of the zone
--------------------------------------------------------------
function SetZoneState(self, state)

    -- get current state
    local prevState = GetZoneState(self)
    
    self:SetVar("ZoneState", state)

    -- perform actions based on zone state
    if (prevState and prevState ~= state) then

        if (state == CONSTANTS["ZONE_STATE_NO_INVASION"]) then

            -- if no invasion play the peace effect    
            RemoveEffect(self)
            EnableEffect(self, "peaceTime")

            -- play animation
            SetAnimation(self, "idle_light")

        elseif (state == CONSTANTS["ZONE_STATE_TRANSITION"]) then

            -- disable the effect in this state
            RemoveEffect(self)
            EnableEffect(self, "red")

            -- play animation with optional delay
            local delay = self:GetVar("anim_delay")
            if (delay and tonumber(delay) > 0) then
                GAMEOBJ:GetTimer():AddTimerWithCancel( tonumber(delay) , "PlayAnim_LightToAlarm", self )
            else
                SetAnimation(self, "light_to_alarm")
            end
            
        elseif (state == CONSTANTS["ZONE_STATE_HIGH_ALERT"]) then

            -- disable the effect in this state
            RemoveEffect(self)
            EnableEffect(self, "red")

            -- play animation
            SetAnimation(self, "idle_alarm")

        elseif (state == CONSTANTS["ZONE_STATE_MEDIUM_ALERT"]) then

            -- disable the effect in this state
            RemoveEffect(self)
            EnableEffect(self, "orange")

            -- play animation
            SetAnimation(self, "idle_alarm")

        elseif (state == CONSTANTS["ZONE_STATE_LOW_ALERT"]) then

            -- disable the effect in this state
            RemoveEffect(self)
            EnableEffect(self, "yellow")

            -- play animation
            SetAnimation(self, "idle_alarm")

        elseif (state == CONSTANTS["ZONE_STATE_DONE_TRANSITION"]) then

            -- if no invasion play the peace effect    
            RemoveEffect(self)

            -- play animation
            SetAnimation(self, "alarm_to_light")

        end
        
    end
    
end    


--------------------------------------------------------------
-- adds a detector to this object, if one does not exist
--------------------------------------------------------------
function SpawnDetector(self)

    -- if we have a child detector already, we are done
    local myChild = getObjectByName(self, "ChildDetector")
    if(myChild and myChild:Exists()) then
        -- send state information
        myChild:NotifyObject{ name="zone_state_change", param1 = GetZoneState(self) }
        return    
    end 
    
    -- get position
    local mypos = self:GetPosition().pos 

    -- Add a detector here
    RESMGR:LoadObject 
    { 
        objectTemplate = CONSTANTS["LAMP_DETECTOR_LOT"], 
        x= mypos.x, 
        y= mypos.y, 
        z= mypos.z, 
        owner = self, 
        rw= 0.7071, 
        rx= 0.0, 
        ry= -0.7071, 
        rz = 0.0 
    } 

end


--------------------------------------------------------------
-- removes a detector spawned by this object
--------------------------------------------------------------
function RemoveDetector(self)

    -- Remove the child
    local myChild = getObjectByName(self, "ChildDetector")
    if(myChild and myChild:Exists()) then
        myChild:Die{killerID = myChild, killType = "SILENT"}
    end 

end


--------------------------------------------------------------
-- called when a child is loaded
--------------------------------------------------------------
function onChildLoaded(self,msg)

	if msg.templateID == CONSTANTS["LAMP_DETECTOR_LOT"] then

        -- store current state
        msg.childID:SetVar("ZoneState", GetZoneState(self))

        -- store the object and it's parent
        storeObjectByName(self, "ChildDetector", msg.childID)
        storeParent(self, msg.childID)
        
     end

end


--------------------------------------------------------------
-- Plays and sets the animation
--------------------------------------------------------------
function SetAnimation(self, name)

    -- return early if render is not ready
    if (self:GetVar("bRenderReady") == false) then
        return
    end

    -- play animation
    self:PlayAnimation{animationID = name}
    
end


--------------------------------------------------------------
-- Enables an effect on the spout unless one is already present
--------------------------------------------------------------
function EnableEffect(self, action )

    -- return early if render is not ready
    if (self:GetVar("bRenderReady") == false) then
        return
    end

    -- return out if we already have an effect
    local myEffect = self:GetVar("currentEffect")
	if ( myEffect ) then
        return
	end
	
    -- make a new effect
	self:PlayFXEffect{ name="lamp", effectType = action }

    -- save the effect	
	self:SetVar( "currentEffect", true )

end

--------------------------------------------------------------
-- Removes an effect on the spout
--------------------------------------------------------------
function RemoveEffect(self)

    -- return early if render is not ready
    if (self:GetVar("bRenderReady") == false) then
        return
    end
    
    -- get current effect
    local myEffect = self:GetVar("currentEffect")
    
    -- remove the effect
   	if ( myEffect ) then
		self:StopFXEffect{ name = "lamp" }
		self:SetVar("currentEffect", false)
	end

end


--------------------------------------------------------------
-- Called when the render is ready on the client
--------------------------------------------------------------
function onRenderComponentReady(self, msg)

    self:SetVar("bRenderReady", true)

	-- let the zone control object know we are ready to be instructed
	GAMEOBJ:GetZoneControlID():FireEvent{ senderID=self, args="ZoneStateClientObjectReady" }

end


--------------------------------------------------------------
-- Called when object is added to world
--------------------------------------------------------------
function onStartup(self)

	-- register ourself with the client-side zone script to be instructed later
    registerWithZoneControlObject(self)
    
    self:SetVar("currentEffect", CONSTANTS["NO_OBJECT"])
    
    -- set state to No Info, waiting for state information
    SetZoneState(self, CONSTANTS["ZONE_STATE_NO_INFO"])
        
end



--------------------------------------------------------------
-- Called when object gets a notification
--------------------------------------------------------------
function onNotifyObject(self, msg)

    -- set the state
    if (msg.name == "zone_state_change") then
        SetZoneState(self, msg.param1)
    end
    
end


--------------------------------------------------------------
-- called when timer complete
--------------------------------------------------------------
function onTimerDone(self, msg)

    -- play an animation
    if (msg.name == "PlayAnim_LightToAlarm") then
        SetAnimation(self, "light_to_alarm")
    elseif (msg.name == "PlayAnim_AlarmToLight") then       
        SetAnimation(self, "alarm_to_light")
    end

end


