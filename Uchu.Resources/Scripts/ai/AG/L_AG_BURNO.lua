
--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('o_mis')
require('c_AvantGardens')




--------------------------------------------------------------
-- constants
--------------------------------------------------------------
CONSTANTS["HOTDOG_CART_RESPAWN_DELAY"] = 10.0	




--------------------------------------------------------------
-- startup
--------------------------------------------------------------
function onStartup(self)
	
	registerWithZoneControlObject(self)
	
	--self:PlayAnimation{ animationID = "idle" }

	burno1Time = 4.3333  -- time for burno1 animation to play before calling on burno2
	burno2Time = 5.2  -- time for burno2 animation to play before calling on burno3

end




--------------------------------------------------------------
-- child loaded (the hotdog cart)
--------------------------------------------------------------
-- store the hotdog cart so we can check its state
function onChildLoaded( self,msg )

    if ( msg.childID:GetLOT().objtemplate == CONSTANTS["LOT_BURNO_HOTDOG_CART"] ) then

            storeParent( self, msg.childID )   
            
    end

end






--------------------------------------------------------------
-- notify object
--the rebuild state changed
--or the zone script just let us know that we're ready to spawn the cart the first time
--------------------------------------------------------------
function onNotifyObject( self, msg )
	
	if ( msg.name == "loadCart" ) then
		LoadHotdogCart( self )
		

	-- if the rebuild is complete
	elseif ( msg.name == "rebuildDone" ) then
		-- start a timer to notify when to change animations
		GAMEOBJ:GetTimer():CancelAllTimers(self)
		GAMEOBJ:GetTimer():AddTimerWithCancel( burno1Time, "justFixedCart", self )
		-- play the burno1 love animation    
		self:PlayAnimation{ animationID = "burno1" }
		
	
	-- if the player is fixing the cart
	elseif ( msg.name == "rebuildFixing" ) then
		-- play the build hover happy animation
		self:PlayAnimation{ animationID = "build" }
		
	
	-- if the player fixing the cart used the UI button to cancel the quickbuild partway through
	elseif ( msg.name == "rebuildCancelled" ) then
		GAMEOBJ:GetTimer():CancelAllTimers(self)
		LoadHotdogCart( self )
	
	end

end





--------------------------------------------------------------
-- timers
--------------------------------------------------------------
-- when one of the animation timers is done
function onTimerDone(self, msg)
	-- when burno2 anim is done
	if msg.name == "burno2Timer" then
		-- play the burno3 anim
		self:PlayAnimation{ animationID = "burno3" }
		
	
	-- when the burno1 timer ends.  
	elseif msg.name == "justFixedCart" then
		-- play burno2 anim
		self:PlayAnimation{ animationID = "burno2" }
		-- start timer for anim
		GAMEOBJ:GetTimer():CancelAllTimers(self)
		GAMEOBJ:GetTimer():AddTimerWithCancel( burno2Time, "burno2Timer", self )
		GAMEOBJ:GetTimer():AddTimerWithCancel( CONSTANTS["HOTDOG_CART_RESPAWN_DELAY"], "respawnCart", self )
		
	elseif msg.name == "respawnCart" then
		LoadHotdogCart( self )
		
	end
	
end




--------------------------------------------------------------
-- spawn the hotdog cart
--------------------------------------------------------------
function LoadHotdogCart( self )

    --Get Burno's position and rotation for use in spawning the hotdog cart      
	local mypos = self:GetPosition().pos
    local myRot = self:GetRotation()
	
    --This is the offset distance for the cart
    local offsetDist = { x = -12.0, y = 0.0, z = 7.0 }

    --set the position for the cart
    local newPos = self:GetParallelPosition{ referenceObject = self, offset = offsetDist }.newPosition
    
    -- raise the y a bit to make sure that the bricks fall down onto the path and not through it

    --use this string to get the rebuild activator to spawn properly
    -- March 31, 2009 note:
    -- we need to offset the activator a little since the player is now repositioned based on where the activator is
    -- if we don't adjust the y, then the player moves below the pathway or the bricks fall through it
    -- we also adjust the x and z so that the player isn't standing in the middle of where the cart is built
    local posString = self:CreatePositionString{x = newPos.x + 1.25, y = newPos.y + 2.0, z = newPos.z + 1.25 }.string
    local config = {{"rebuild_activators", posString}}

    --load in the cart
    RESMGR:LoadObject { objectTemplate = 4046, 
    					x= newPos.x, 
    					y= newPos.y ,
    					z= newPos.z, 
    					owner = self, 
    					rw= myRot.w, 
    					rx= myRot.x, 
    					ry= myRot.y, 
    					rz = myRot.z,
    					configData = config}
end
