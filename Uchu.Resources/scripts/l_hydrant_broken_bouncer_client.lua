
--------------------------------------------------------------
-- includes
--------------------------------------------------------------
require('L_BOUNCER_BASIC')



--------------------------------------------------------------
-- Called when object is added to world
--------------------------------------------------------------
function onStartup(self)

	self:SetVar( "bGotConfig", false)
	
end



--------------------------------------------------------------
-- on collision functions
--------------------------------------------------------------
function onCollisionPhantom(self, msg)
	
	bounceNow( self, msg.objectID )
	
	return msg
end




--------------------------------------------------------------
-- bounces the player and tells the hydrant to clean off their skunk stink
--------------------------------------------------------------
function bounceNow( self, target )

	if ( self:GetVar( "bGotConfig" ) == false ) then
		return
	end

	--print( "client-side bounceNow" )
	
	bounceObj( self, target )
	
	NotifyBrokenHydrant( self, target )
		
end




--------------------------------------------------------------
-- tell the broken hydrant to clean off the player and to revert to the regular hydrant since the bouncer has been used
--------------------------------------------------------------
function NotifyBrokenHydrant( self, target )
	
	local hydrant = self:GetParentObj().objIDParent
	
	if( hydrant ~= nil and hydrant:Exists() ) then
	
		--print( "bouncer's parent exists" )
	
		-- ask the hydrant to check if the player has skunk stink, and if so, wash them off
		-- and to revert to a regular hydrant
		hydrant:NotifyObject{ ObjIDSender = target, name = "cleanPlayer" }
	end
		
end





--------------------------------------------------------------
-- got the bouncer config info from server-side
--------------------------------------------------------------
function onPlayAnimation( self, msg )

	-- using this message just as a way to get a float and a string from server-side
	-- to get the bouncer config data
	
	-- ( tried using onScriptNetworkVarUpdate but if 2 clients are running, only 1 of them receives that message )
	
	if ( self:GetVar( 'bGotConfig' ) == true ) then
		return
	end
	

	self:SetVar( "bouncer_speed", msg.fPriority )
	self:SetVar( "bouncer_destination", msg.animationID )
	
	--print( "client-side bouncer got config data" )
	--print( self:GetVar( "bouncer_speed" ) )
	--print( self:GetVar( "bouncer_destination" ) )

	self:SetVar( "bGotConfig", true)

end


