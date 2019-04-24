
--------------------------------------------------------------
-- Includes
require('o_mis')
require('c_Zorillo')



--------------------------------------------------------------
-- Constants
--------------------------------------------------------------
CONSTANTS["BROKEN_HYDRANT_LOT"] = 3999






--------------------------------------------------------------
-- Called when object is added to world
--------------------------------------------------------------
function onStartup(self)

	--print( "smashable hydrant: onStartup" )
	    
    self:SetVar( "hydrantPos", self:GetPosition{}.pos )
	
	self:SetPickType{ ePickType = 6 }	-- PICK_LIST_SMASHABLE in enum PICK_LIST_TYPE in lwoCommonVars.h
										-- (smashables currently use the regular arrow cursor, but specify this in case that changes later.)
    
    ForgetBrokenHydrant( self )
end
	
	
	

--------------------------------------------------------------
-- Called when object is smashed
--------------------------------------------------------------
function onDie( self, msg )
	
	ShowBroken( self )
	
end





--------------------------------------------------------------
-- called when a child object is loaded up, in this case, the bouncer
--------------------------------------------------------------
function onChildLoaded( self,msg )
    
	if ( msg.childID:GetLOT().objtemplate == CONSTANTS["BROKEN_HYDRANT_LOT"] ) then

		storeObjectByName( self, "brokenHydrantID", msg.childID )
		
		storeParent( self, msg.childID )
		
	end

end




--------------------------------------------------------------
-- clear out the broken hydrant variable
--------------------------------------------------------------
function ForgetBrokenHydrant( self )

	self:SetVar( "brokenHydrantID", CONSTANTS["EMPTY_ID_NAME"] )
end




--------------------------------------------------------------
-- spawn in the model of the hydrant all broken
--------------------------------------------------------------
function ShowBroken( self )
   
    -- return out if we already have a broken hydrant
	if ( self:GetVar( "brokenHydrantID" ) ~= CONSTANTS["NO_OBJECT"] ) then
		return
	end


	-- get the hydrant's position
	local myPos = self:GetVar( "hydrantPos" )

	
	RESMGR:LoadObject { objectTemplate = CONSTANTS["BROKEN_HYDRANT_LOT"],
						x = myPos.x, y = myPos.y, z = myPos.z,
						owner = self }

end






