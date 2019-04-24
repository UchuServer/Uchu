

--------------------------------------------------------------
-- On startup
--------------------------------------------------------------
function onStartup( self )

	self:SetVar( "bRenderReady", false )
	
	self:PlayAnimation{ animationID = "balloon1" }
end




--------------------------------------------------------------
-- got a msg from the server-side script just for this client
--------------------------------------------------------------
function onNotifyClientRebuildSectionState( self, msg )

	-- the server-side zone script got onPlayerLoaded
	-- it passed that info to the balloon's server-side script
	-- which then called this on the newly-loaded player's client
	
	-- use the appropriate balloon anim based on how inflated it currently is
	
	-- (Note: this has nothing to do with rebuilds.
		-- We're just using this msg because it can send a msg to one particular client. )
	
	UpdateAnimation( self, msg.iState )
		
end




--------------------------------------------------------------
-- change the animantion to reflect how much stink is currently feeding into it
--------------------------------------------------------------
function UpdateAnimation( self, stinkAmount )

	if ( self:GetVar( "bRenderReady" ) == false ) then
		return
	end

	if ( stinkAmount == 0 ) then
		self:PlayAnimation{ animationID = "balloon1" }
	
	elseif ( stinkAmount == 1 ) then
		self:PlayAnimation{ animationID = "balloon2" }
	
	elseif ( stinkAmount == CONSTANTS["SUFFICIENT_STINK"] ) then
		self:PlayAnimation{ animationID = "balloon3" }
		
	end
end



--------------------------------------------------------------
-- Called when the render is ready on the client
--------------------------------------------------------------
function onRenderComponentReady(self, msg)

    self:SetVar( "bRenderReady", true )
    
end
