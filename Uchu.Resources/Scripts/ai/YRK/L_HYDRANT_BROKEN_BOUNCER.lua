

--------------------------------------------------------------
-- on startup
--------------------------------------------------------------
function onStartup( self )
	
	SendConfigInfoToClients( self )
	
end




--------------------------------------------------------------
-- send the bouncer config info client-side
--------------------------------------------------------------
function SendConfigInfoToClients( self )

	-- let the client-side know the bouncer config data
	local speed = self:GetVar( "bouncer_speed" )
	local destination = self:GetVar( "bouncer_destination" )
		
	--self:SetNetworkVar( "bouncer_speed", speed )
	--self:SetNetworkVar( "bouncer_destination", destination )
	--print( "server-side broken hydrant passing along config data" )
	
	-- Note: using network vars worked with only 1 client running,
	--but if 2 clients are running, then only 1 of them receives onScriptNetworkVarUpdate
	
	-- using this message instead, just as a way to get a float and a string to client-side
	self:PlayAnimation{ animationID = destination, fPriority = speed }
	
	-- set a timer to resend the info
	GAMEOBJ:GetTimer():AddTimerWithCancel( 0.5, "resendConfigTimer", self )
	
end




--------------------------------------------------------------
-- called when timers expire
--------------------------------------------------------------
function onTimerDone(self, msg)

	if (msg.name == "resendConfigTimer") then
	
		SendConfigInfoToClients( self )
	end
	
end


