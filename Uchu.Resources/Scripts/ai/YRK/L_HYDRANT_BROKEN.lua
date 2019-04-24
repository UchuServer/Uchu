

--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('o_mis')
require('c_Zorillo')



--------------------------------------------------------------
-- Constants
--------------------------------------------------------------
CONSTANTS["HYDRANT_BOUNCER_LOT"] = 4008
CONSTANTS["REPAIRED_HYDRANT_LOT"] = 3997
CONSTANTS["HYDRANT_REPAIR_AFTER_SMASH_TIME"] = 10.0
CONSTANTS["HYDRANT_REPAIR_AFTER_USE_TIME"] = 2.0




--------------------------------------------------------------
-- Called when object is added to world
--------------------------------------------------------------
function onStartup(self)

    local originalHydrant = getParent(self)
    if( originalHydrant ~= nil and originalHydrant:Exists() ) then
		--print( "copying info from original" )
		
		self:SetVar( "hydrantPos", originalHydrant:GetVar( "hydrantPos" ) )
		
		local speed = originalHydrant:GetVar( "bouncer_speed" )
		local destination = originalHydrant:GetVar( "bouncer_destination" )
		
		self:SetVar( "bouncer_speed", speed )
		self:SetVar( "bouncer_destination", destination )
		
    --else
		--print( "don't know the original hydrant" )
    end
    
	ForgetBouncer( self )	    
	AddBouncer( self )
	
	--self:AddSkill{ skillID = CONSTANTS["DESTINK_SKILL"] }			-- done via ObjectSkills table
	
	-- if the player who smashed the hydrant doesn't use it soon enough, we'll replace it with a repaired hydrant again
	SetRepairTimer( self, CONSTANTS["HYDRANT_REPAIR_AFTER_SMASH_TIME"] )
	
end




--------------------------------------------------------------
-- called when a child object is loaded up, in this case, the bouncer
--------------------------------------------------------------
function onChildLoaded( self,msg )
    
    if ( msg.childID:GetLOT().objtemplate == CONSTANTS["HYDRANT_BOUNCER_LOT"] ) then
    
		--print( "bouncer loaded server-side" )

		storeObjectByName( self, "bouncerID", msg.childID )
		
		storeParent( self, msg.childID )	
	end

end





--------------------------------------------------------------
-- add the bouncer
--------------------------------------------------------------
function AddBouncer( self )  
   
    -- return out if we already have a bouncer
   if ( self:GetVar( "bouncerID" ) ~= CONSTANTS["NO_OBJECT"] ) then
		return
	end


	-- get the hydrant's position
	local myPos = self:GetVar( "hydrantPos" )

	
    local bounceSpeed = self:GetVar("bouncer_speed")
    
	-- set up bouncer config data
	local vecString = self:GetVar("bouncer_destination")
	local config = { {"bouncer_speed", bounceSpeed} , {"objtype", CONSTANTS["HF_NODE_BOUNCER"]}, {"bouncer_destination", vecString } }
	
	RESMGR:LoadObject { objectTemplate = CONSTANTS["HYDRANT_BOUNCER_LOT"],
						x = myPos.x, y = myPos.y, z = myPos.z,
						owner = self,
						objType = CONSTANTS["HF_NODE_BOUNCER"],
						configData = config }

end





--------------------------------------------------------------
-- clear out the bouncer variable
--------------------------------------------------------------
function ForgetBouncer( self )

	self:SetVar( "bouncerID", CONSTANTS["EMPTY_ID_NAME"] )
end





--------------------------------------------------------------
-- got a fireEvent message from client-side
--------------------------------------------------------------
function onFireEventServerSide( self, msg )

	if ( msg.args == "cleanPlayer" ) then
	
		--print( "server-side broken hydrant got msg to clean player" )

		--CleanPlayer( self, player)
		CleanPlayer( self, msg.senderID )
			
		-- get rid of the bouncer
		RemoveBouncer( self )
	
		-- set a timer to delete the broken hydrant itself
		SetRepairTimer( self, CONSTANTS["HYDRANT_REPAIR_AFTER_USE_TIME"] )
		
	end
	
end




--------------------------------------------------------------
--cast the destink still on the given player
--------------------------------------------------------------
function CleanPlayer( self, target )

	if ( target and target:Exists() and target:GetFaction().faction == 1 ) then
	
		--print( "casting destink skill" )
		
		self:CastSkill{ optionalTargetID = target, skillID = CONSTANTS["DESTINK_SKILL"] }
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
-- called when timers expire
--------------------------------------------------------------
function onTimerDone( self, msg )

	if (msg.name == "repairTimer") then
			
		RevertToRepairedHydrant( self )	
	
	end
end




--------------------------------------------------------------
-- get rid of the bouncer and the broken hydrant and spawn in a regular, repaired hydrant instead
--------------------------------------------------------------
function RevertToRepairedHydrant( self )

	-- get the hydrant's position
	local myPos = self:GetVar( "hydrantPos" )
				
	local bounceSpeed = self:GetVar( "bouncer_speed" )
	local bounceDest = self:GetVar("bouncer_destination")
		
	local config = { {"bouncer_speed", bounceSpeed} , {"objtype", CONSTANTS["HF_NODE_BOUNCER"]}, {"bouncer_destination", bounceDest } }
	
	RESMGR:LoadObject { objectTemplate = CONSTANTS["REPAIRED_HYDRANT_LOT"],
						x = myPos.x, y = myPos.y, z = myPos.z,
						objType = CONSTANTS["HF_NODE_BOUNCER"],
						configData = config }


	-- get rid of the bouncer
	RemoveBouncer( self )
	
	-- get rid of the broken hydrant
	GAMEOBJ:DeleteObject( self )
end





--------------------------------------------------------------
-- set a timer for when to revert to the regular, repaired hydrant even if the bouncer is never used
--------------------------------------------------------------
function SetRepairTimer( self, duration )

	GAMEOBJ:GetTimer():CancelTimer("repairTimer", self)
	GAMEOBJ:GetTimer():AddTimerWithCancel( duration, "repairTimer", self )
end




