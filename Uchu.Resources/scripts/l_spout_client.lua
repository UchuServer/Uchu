--------------------------------------------------------------
-- Client side script maintains the state of the spouts using 
-- player proximity
--------------------------------------------------------------
	
	
--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('o_mis')
require('c_Zorillo')


--------------------------------------------------------------
-- Locals and Constants
--------------------------------------------------------------

-- states for the water height
CONSTANTS["WATER_HEIGHT_NORMAL"] = 1		-- the normal amount of water that comes out when no spouts are plugged	
CONSTANTS["WATER_HEIGHT_INCREASED"] = 2		-- a little higher.  1 other spout is plugged, but this one isn't
CONSTANTS["WATER_HEIGHT_LAUNCHABLE"] = 3	-- high enough to launch the player above the fountain.  Both of the other spouts are plugged but this one isn't

CONSTANTS["effectName"] = "water"
CONSTANTS["effectNum"] = 216					-- the number and the names for the water particle effects

SPOUT_EFFECT_NAME = {}
SPOUT_EFFECT_NAME[CONSTANTS["WATER_HEIGHT_NORMAL"]] = "spoutLow"
SPOUT_EFFECT_NAME[CONSTANTS["WATER_HEIGHT_INCREASED"]] = "spoutMed"
SPOUT_EFFECT_NAME[CONSTANTS["WATER_HEIGHT_LAUNCHABLE"]] = "spoutHigh"
CONSTANTS["WATER_EFFECT_NAMES"] = SPOUT_EFFECT_NAME

CONSTANTS["BOUNCER_LOT"] = 3379



--------------------------------------------------------------
-- Get the enable state
--------------------------------------------------------------
function GetEnableState(self)

    return self:GetVar("SpoutEnabled")
    
end


--------------------------------------------------------------
-- Set the enable state
--------------------------------------------------------------
function SetEnableState(self, state)

    -- get prev state
    local bPrevEnabled = GetEnableState(self)
    
    -- calc new state and set
    local bIsEnabled = (state == CONSTANTS["ZONE_STATE_NO_INVASION"])
    self:SetVar("SpoutEnabled", bIsEnabled)

    -- perform actions based on new enable state
    if (bPrevEnabled ~= bIsEnabled) then

        -- try to change effects based on enable state
        if (bIsEnabled == true and self:GetVar("PlayerOnMe") == false) then
            
            RemoveEffect(self) 
            EnableEffect(self, GetStateEffectName(self))
            
            -- if state changed and is at max without a player on it try to add a bouncer
            if (tonumber(self:GetVar("SpoutState")) >= tonumber(CONSTANTS["WATER_HEIGHT_LAUNCHABLE"])) then
                -- make bouncer
                AddBouncer(self)
            end		            

        else -- disabled or someone standing on it
        
            RemoveEffect(self) 
            RemoveBouncer(self)
            
        end            

    end
    
end    


--------------------------------------------------------------
-- get the name from the array based on spout state
--------------------------------------------------------------
function GetStateEffectName(self)

    local state = self:GetVar("SpoutState")
    return CONSTANTS["WATER_EFFECT_NAMES"][state]

end


--------------------------------------------------------------
-- Enables an effect on the spout unless one is already present
--------------------------------------------------------------
function EnableEffect(self, action)

    -- return early if render is not ready or we are disabled
    if (self:GetVar("bRenderReady") == false or GetEnableState(self) == false) then
        return
    end

    -- return out if we already have an effect
    local myEffect = self:GetVar("waterEffect")
	if ( myEffect ) then
        return
	end
	
    -- make a new effect
	self:PlayFXEffect{ name = CONSTANTS["effectName"], effectID = CONSTANTS["effectNum"], effectType = action }

    -- save the effect
	self:SetVar("waterEffect", true )

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
	local myEffect = self:GetVar("waterEffect")

    
    -- remove the effect
	if ( myEffect ) then
		self:StopFXEffect{ name = CONSTANTS["effectName"] }
		self:SetVar( "waterEffect", false )
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
-- On startup
--------------------------------------------------------------
function onStartup(self)

	-- register ourself with the client-side zone script to be instructed later
    registerWithZoneControlObject(self)
    
    self:SetVar("waterEffect", false)
    self:SetVar("bRenderReady", false)
    self:SetVar("PlayerOnMe", false )    -- @TODO: leaveprox does not work on logout
    
    -- stores if the spout is currently enabled
    self:SetVar("SpoutEnabled", true) 
    
    -- start spout at normal level
    self:SetVar("SpoutState", CONSTANTS["WATER_HEIGHT_NORMAL"])

    self:SetProximityRadius{ radius = CONSTANTS["SPOUT_RADIUS"]}

    -- set state to No Info, waiting for state information
    SetEnableState(self, CONSTANTS["ZONE_STATE_NO_INFO"])
    
end


--------------------------------------------------------------
-- Called when proximity is updated
--------------------------------------------------------------
function onProximityUpdate(self, msg)

    -- if spout state is maxed out, do nothing
    local state = self:GetVar("SpoutState")
    if (tonumber(state) >= tonumber(CONSTANTS["WATER_HEIGHT_LAUNCHABLE"])) then
        return
    end

    -- a player enters proximity while no players are on it
    if ( msg.status == "ENTER" and msg.objId:GetFaction().faction == 1 and self:GetVar("PlayerOnMe") == false ) then
    
        NotifyGroupObjects(self, "SpoutOff")
        
	elseif ( msg.status == "LEAVE" and msg.objId:GetFaction().faction == 1 ) then
	
		GAMEOBJ:GetTimer():CancelAllTimers( self )
		GAMEOBJ:GetTimer():AddTimerWithCancel( 0.1, "ProxCheck",self )
	
	end

end

--------------------------------------------------------------
-- Determines if any players are currently in proximity
--------------------------------------------------------------
function ArePlayersInProximity(self)

	local objs = self:GetProximityObjects().objects
	local index = 1

	while index <= table.getn(objs)  do

		local target = objs[index]
		local faction = target:GetFaction()
		--verify that we are only bouncing players
		if faction and faction.faction == 1 then
			return true;
		end
		index = index + 1

	end
	return false;

end


--------------------------------------------------------------
-- Timers
--------------------------------------------------------------
function onTimerDone(self, msg)
	
	-- check for players in proximity
    if (msg.name == "ProxCheck") then
	    
	    -- no one left
        if (ArePlayersInProximity(self) == false) then
        
            NotifyGroupObjects(self, "SpoutOn")
            
		end

    end
	
end


--------------------------------------------------------------
-- tries to increase our state, returns true if it changes
--------------------------------------------------------------
function IncreaseSpoutState(self)

    local state = self:GetVar("SpoutState")
    
    -- no change if maxed
    if (tonumber(state) >= tonumber(CONSTANTS["WATER_HEIGHT_LAUNCHABLE"])) then
        return false
    
    else
        -- increase otherwise and return an update
        state = tonumber(state) + 1
        self:SetVar("SpoutState", state)
        return true
    end
    
end


--------------------------------------------------------------
-- tries to decrease our state, returns true if it changes
--------------------------------------------------------------
function DecreaseSpoutState(self)

    local state = self:GetVar("SpoutState")
    
    -- no change if min
    if (tonumber(state) <= tonumber(CONSTANTS["WATER_HEIGHT_NORMAL"])) then
        return false
    
    else
        -- increase otherwise and return an update
        state = tonumber(state) - 1
        self:SetVar("SpoutState", state)
        return true
    end
    
end


--------------------------------------------------------------
-- Notification to object
--------------------------------------------------------------
function onNotifyObject(self, msg)

    -- someone is turning a spout off
    if (msg.name == "SpoutOff") then
    
        -- if we were the sender
        if (msg.ObjIDSender:GetID() == self:GetID()) then

            -- someone is on us
            self:SetVar( "PlayerOnMe", true )
            
            -- remove our effect and any bouncer
            RemoveEffect(self)
            RemoveBouncer(self)
            
		else    -- we are not the sender
		
		    -- update our spout state    
		    local bStateChange = IncreaseSpoutState(self)
		    
		    -- change effects if needed
		    if (bStateChange and self:GetVar("PlayerOnMe") == false) then
		        RemoveEffect(self)
		        EnableEffect(self, GetStateEffectName(self))
		        
                -- if state changed and is at max without a player on it
                if (tonumber(self:GetVar("SpoutState")) >= tonumber(CONSTANTS["WATER_HEIGHT_LAUNCHABLE"])) then
                    
                    -- make bouncer
                    AddBouncer(self)
                    
                end		        
                
		    end
		
		end

    -- someone is turning a spout on
    elseif (msg.name == "SpoutOn") then
    
        -- if we were the sender
        if (msg.ObjIDSender:GetID() == self:GetID()) then
        
            -- someone is off us
            self:SetVar( "PlayerOnMe", false )
            
            -- turn our effect back on
            EnableEffect(self, GetStateEffectName(self))
            
		else    -- we are not the sender

		    -- update our spout state    
		    local bStateChange = DecreaseSpoutState(self)
		    
		    -- change effects if needed
		    if (bStateChange and self:GetVar("PlayerOnMe") == false) then
		        RemoveEffect(self)
		        EnableEffect(self, GetStateEffectName(self))
		        RemoveBouncer(self)
		    end

        end
        
    
    elseif (msg.name == "slashCommand") then
		
		-- if spout state is maxed out, do nothing
		local state = self:GetVar("SpoutState")
		if (tonumber(state) < tonumber(CONSTANTS["WATER_HEIGHT_LAUNCHABLE"])) then
		
			if ( self:GetVar( "PlayerOnMe" ) == true ) then
				NotifyGroupObjects(self, "SpoutOn")
			else
				NotifyGroupObjects(self, "SpoutOff")
			end
		end
		
    -- set the state
    elseif (msg.name == "zone_state_change") then
        SetEnableState(self, msg.param1)
	end
    
end        


--------------------------------------------------------------
-- send a message to all objects in group
--------------------------------------------------------------
function NotifyGroupObjects(self, notifyName)

    -- send a notify to everyone
    local objects = self:GetObjectsInGroup{ group = CONSTANTS["SPOUT_GROUP_NAME"] }.objects
    for i = 1, table.maxn (objects) do      
         objects[i]:NotifyObject{ name = notifyName, ObjIDSender = self }
    end  


end


--------------------------------------------------------------
-- adds a bouncer on the spout
--------------------------------------------------------------
function AddBouncer( self )
    -- return early if we are disabled
    if (GetEnableState(self) == false) then
        return
    end
    
	-- get the spout's position
	local spoutPos = self:GetPosition{}.pos
	
	-- we'll spawn the bouncer at the same position
	local bouncerPos = self:GetPosition{}.pos
	bouncerPos.x = spoutPos.x
	bouncerPos.y = spoutPos.y
	bouncerPos.z = spoutPos.z
	
	
    -- default bouncer information
    local landingPos = CONSTANTS["SPOUT_BOUNCER_DEST"]
    local bounceSpeed = CONSTANTS["SPOUT_BOUNCER_SPEED"]
    
	-- set up bouncer config data
	local vecString = self:GetVar("bouncer_destination")
	local config = { {"bouncer_speed", bounceSpeed} , {"objtype", CONSTANTS["HF_NODE_BOUNCER"]}, {"bouncer_destination", vecString } }
	
	RESMGR:LoadObject { objectTemplate = CONSTANTS["BOUNCER_LOT"],
						x = bouncerPos.x, y = bouncerPos.y, z = bouncerPos.z,
						owner = self,
						objType = CONSTANTS["HF_NODE_BOUNCER"],
						configData = config }
end


--------------------------------------------------------------
-- called when children are loaded
--------------------------------------------------------------
function onChildLoaded( self,msg )
    
    if ( msg.childID:GetLOT().objtemplate == CONSTANTS["BOUNCER_LOT"] ) then
		storeObjectByName( self, "bouncerID", msg.childID )
	end

end


--------------------------------------------------------------
-- removes a bouncer from the spout
--------------------------------------------------------------
function RemoveBouncer( self )

	local bouncerObj = getObjectByName( self, "bouncerID" )
	
	if( bouncerObj ~= nil and bouncerObj:Exists() ) then
        GAMEOBJ:DeleteObject( bouncerObj )
	end
	
end
