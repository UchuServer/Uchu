
--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('o_mis')
require('c_AvantGardens')



--------------------------------------------------------------
-- Constants
--------------------------------------------------------------
CONSTANTS["MOSAIC_PORTAL_LOT"] = 4023
CONSTANTS["BROKEN_MOSAIC_LOT"] = 4044




--------------------------------------------------------------
-- startup
--------------------------------------------------------------
function onStartup( self )

	ForgetPortal( self )
	ForgetBroken ( self )
end




--------------------------------------------------------------
-- Handle notification of rebuild changes
--------------------------------------------------------------
function onRebuildNotifyState( self, msg )

	UpdateChildren( self, msg.iState )
	
end




--------------------------------------------------------------
-- Called when the render is ready on the client
--------------------------------------------------------------
function onRenderComponentReady( self )

    -- check what state the rebuild is in and add the broken mosaic or the portal if appropriate
	local rebuildState = self:GetRebuildState()
    
    -- if the state is idle we are active
    if (rebuildState ) then
		
		UpdateChildren( self, tonumber( rebuildState.iState ) ) 
		
	end
end




--------------------------------------------------------------
-- called when a child object is loaded up, in this case, the portal
--------------------------------------------------------------
function onChildLoaded( self,msg )
    
    if ( msg.childID:GetLOT().objtemplate == CONSTANTS["MOSAIC_PORTAL_LOT"] ) then

		storeObjectByName( self, "portalID", msg.childID )
		
		storeParent( self, msg.childID )
		
	elseif ( msg.childID:GetLOT().objtemplate == CONSTANTS["BROKEN_MOSAIC_LOT"] ) then

		storeObjectByName( self, "brokenID", msg.childID )
		
		storeParent( self, msg.childID )
		
	end

end




--------------------------------------------------------------
-- add or remove the portal and the broken mosaic, according to the current rebuild state
--------------------------------------------------------------
function UpdateChildren( self, rebuildState )

	--print( "rebuildState" )
	--print ( rebuildState )

	-- check whether we should have the mosaic portal
	if ( rebuildState == 3 ) then		
		AddPortal( self )
		
	else
		RemovePortal( self )

	end
	

	-- check whether we should have the broken mosaic
	if ( rebuildState == 4 or rebuildState == 0) then
		AddBroken( self )
	
	else
		RemoveBroken( self )
	end

	
end




--------------------------------------------------------------
-- clear out the portal variable
--------------------------------------------------------------
function ForgetPortal( self )

	self:SetVar( "portalID", CONSTANTS["EMPTY_ID_NAME"] )
end





--------------------------------------------------------------
-- spawn in the portal
--------------------------------------------------------------
function AddPortal( self )
   
    -- return out if we already have a portal
   if ( self:GetVar( "portalID" ) ~= CONSTANTS["NO_OBJECT"] ) then
		return
	end


	-- get our position and rotation
	local pos = self:GetPosition{}.pos
	local rotW = self:GetRotation{}.w
	local rotX = self:GetRotation{}.x
	local rotY = self:GetRotation{}.y
	local rotZ = self:GetRotation{}.z

	RESMGR:LoadObject { objectTemplate = CONSTANTS["MOSAIC_PORTAL_LOT"],
						x = pos.x, y = pos.y, z = pos.z,
						rw = rotW, rx = rotX, ry = rotY, rz = rotZ,
						owner = self }

end






--------------------------------------------------------------
-- remove the portal
--------------------------------------------------------------
function RemovePortal( self )

	local portalObj = getObjectByName( self, "portalID" )
	
	if( portalObj ~= nil and portalObj:Exists() ) then

		GAMEOBJ:DeleteObject( portalObj )
	
		ForgetPortal( self )
		
	end
	
end






--------------------------------------------------------------
-- clear out the portal variable
--------------------------------------------------------------
function ForgetBroken( self )

	self:SetVar( "brokenID", CONSTANTS["EMPTY_ID_NAME"] )
end





--------------------------------------------------------------
-- spawn in the portal
--------------------------------------------------------------
function AddBroken( self )
   
    -- return out if we already have a broken mosaic
   if ( self:GetVar( "brokenID" ) ~= CONSTANTS["NO_OBJECT"] ) then
		return
	end


	-- get our position and rotation
	local pos = self:GetPosition{}.pos
	local rotW = self:GetRotation{}.w
	local rotX = self:GetRotation{}.x
	local rotY = self:GetRotation{}.y
	local rotZ = self:GetRotation{}.z

	RESMGR:LoadObject { objectTemplate = CONSTANTS["BROKEN_MOSAIC_LOT"],
						x = pos.x, y = pos.y, z = pos.z,
						rw = rotW, rx = rotX, ry = rotY, rz = rotZ,
						owner = self }

end






--------------------------------------------------------------
-- remove the portal
--------------------------------------------------------------
function RemoveBroken( self )

	local brokenObj = getObjectByName( self, "brokenID" )
	
	if( brokenObj ~= nil and brokenObj:Exists() ) then

		GAMEOBJ:DeleteObject( brokenObj )
	
		ForgetBroken( self )
		
	end
	
end




