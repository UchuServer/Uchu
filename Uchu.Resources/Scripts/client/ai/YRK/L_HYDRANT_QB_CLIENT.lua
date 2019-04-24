
--------------------------------------------------------------
-- Client side script handles particle effect and bouncer
-- when rebuild states change
--------------------------------------------------------------


--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('o_mis')
require('c_Zorillo')



--------------------------------------------------------------
-- Constants
--------------------------------------------------------------
CONSTANTS["HYDRANT_BOUNCER_LOT"] = 3736




--------------------------------------------------------------
-- Called when object is added to world
--------------------------------------------------------------
function onStartup(self)

	self:SetVar("bRenderReady", false)
	
    self:SetPickType{ePickType = 14}
    
    self:SetVar("waterEffect", CONSTANTS["NO_OBJECT"])
    
    ForgetBouncer( self )

end





--------------------------------------------------------------
-- Returns true if the object is in the idle rebuild state
-- ( in the hydrant's case, this means there is no water coming out of it right now )
--------------------------------------------------------------
function IsActive(self)

    -- get the rebuild state
    local rebuildState = self:GetRebuildState()
    
    -- if the state is idle we are active
    if (rebuildState and tonumber(rebuildState.iState) == 3) then
        return true
    else
        return false
    end

end





--------------------------------------------------------------
-- Handled when rendering is ready
--------------------------------------------------------------
function onRenderComponentReady(self, msg) 

    self:SetVar("bRenderReady", true)
    
	if (IsActive(self) == false) then

		-- add the particle effect and bouncer
		BreakHydrant( self )

    end

end




--------------------------------------------------------------
-- Handle notification of rebuild changes
--------------------------------------------------------------
function onRebuildNotifyState(self, msg)

    -- if we just hit the idle state
	if (msg.iState == 3) then

	    -- cancel all timers		TODO do I need this?
	    GAMEOBJ:GetTimer():CancelAllTimers( self )

	    -- remove the particle effect and the bouncer
	    RepairHydrant( self )
	    
	else
	
		-- add the particle effect and bouncer
		BreakHydrant( self )
	    
	end
	
end 





--------------------------------------------------------------
-- break the hydrant so water comes out
--------------------------------------------------------------
function BreakHydrant( self )
		
	AddWaterEffect( self )
	AddBouncer( self )
end





--------------------------------------------------------------
-- repair the hydrant so water no longer comes out
--------------------------------------------------------------
function RepairHydrant( self )

	CancelWaterEffect( self )
	RemoveBouncer( self )
end


--------------------------------------------------------------
-- add the water particle effect
--------------------------------------------------------------
function AddWaterEffect( self )

    -- return early if render is not ready
    if (self:GetVar("bRenderReady") == false) then
        return
    end
    
    
    -- return out if we already have an effect
    local myEffect = self:GetVar("waterEffect")
	if ( myEffect ) then
        return
	end
	
	
    -- make a new effect
	self:PlayFXEffect{ name = "water", effectID = 384, effectType = "water" }

    -- save the effect
	self:SetVar("waterEffect" , true )
	
end




--------------------------------------------------------------
-- cancel the water particle effect
--------------------------------------------------------------
function CancelWaterEffect( self )

    -- return early if render is not ready
    if (self:GetVar("bRenderReady") == false) then
        return
    end
    
    -- get current effect
    local myEffect = self:GetVar("waterEffect")
    
    -- remove the effect
	if ( myEffect ) then
		self:StopFXEffect{ name = "water" }
		self:SetVar("waterEffect", false)
	end

end





--------------------------------------------------------------
-- add the bouncer
--------------------------------------------------------------
function AddBouncer( self )

    -- return early if render is not ready
    if (self:GetVar("bRenderReady") == false) then
        return
    end
    
   
    -- return out if we already have a bouncer
   if ( self:GetVar( "bouncerID" ) ~= CONSTANTS["NO_OBJECT"] ) then
		return
	end


	-- get the hydrant's position
	local hydrantPos = self:GetPosition{}.pos
	
	-- we'll spawn the bouncer at the same position
	local bouncerPos = self:GetPosition{}.pos
	bouncerPos.x = hydrantPos.x
	bouncerPos.y = hydrantPos.y
	bouncerPos.z = hydrantPos.z

	
    local bounceSpeed = self:GetVar("bouncer_speed")
    
	-- set up bouncer config data
	local vecString = self:GetVar("bouncer_destination")
	local config = { {"bouncer_speed", bounceSpeed} , {"objtype", CONSTANTS["HF_NODE_BOUNCER"]}, {"bouncer_destination", vecString } }
	
	RESMGR:LoadObject { objectTemplate = CONSTANTS["HYDRANT_BOUNCER_LOT"],
						x = bouncerPos.x, y = bouncerPos.y, z = bouncerPos.z,
						owner = self,
						objType = CONSTANTS["HF_NODE_BOUNCER"],
						configData = config }

end




--------------------------------------------------------------
-- called when a child object is loaded up, in this case, the bouncer
--------------------------------------------------------------
function onChildLoaded( self,msg )
    
    if ( msg.childID:GetLOT().objtemplate == CONSTANTS["HYDRANT_BOUNCER_LOT"] ) then

		storeObjectByName( self, "bouncerID", msg.childID )
		
		storeParent( self, msg.childID )
	end

end





--------------------------------------------------------------
-- remove the bouncer
--------------------------------------------------------------
function RemoveBouncer( self )

	local bouncerObj = getObjectByName( self, "bouncerID" )
	
	if( bouncerObj ~= nil and bouncerObj:Exists() ) then

		GAMEOBJ:DeleteObject( bouncerObj )
	
		ForgetBouncer( self )
		
	end
	
end




--------------------------------------------------------------
-- clear out the bouncer variable
--------------------------------------------------------------
function ForgetBouncer( self )

	self:SetVar( "bouncerID", CONSTANTS["EMPTY_ID_NAME"] )
end




--------------------------------------------------------------
-- called when a notification is sent
--------------------------------------------------------------
function onNotifyObject( self, msg )

	if (msg.name == "cleanPlayer") then
		
		-- the bouncer says it bounced somebody, so clean off their skunk stink
		
		self:FireEventServerSide{ senderID = msg.ObjIDSender, args = "cleanPlayer" }
	end
end
