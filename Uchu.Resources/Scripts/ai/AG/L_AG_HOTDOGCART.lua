

function onStartup( self )

	self:SetVar( "bFinishedBuild", false )
	
end




function onRebuildNotifyState(self, msg)
	
	local Burno = self:GetParentObj().objIDParent 
	if( Burno == nil or Burno:Exists() == false ) then
		return
	end
	
	
	-- a player just did the quickbuild.
	if (msg.iState == 3) then                    
	
		self:SetVar( "bFinishedBuild", true )
		Burno:NotifyObject{ ObjIDSender = self, name = "rebuildDone" }

	
	-- a player is currently fixing me
	elseif (msg.iState == 1) then                     

	 	Burno:NotifyObject{ ObjIDSender = self, name = "rebuildFixing" }

	
	-- the quickbuild is being reset either
	-- because a player finished it or
	-- because a player was doing the quickbuild, but used the UI button to exit before finishing it
	elseif ( msg.iState == 4 ) then
	
		if ( self:GetVar( "bFinishedBuild" ) == true ) then
			self:SetVar( "bFinishedBuild", false )
		else
			Burno:NotifyObject{ ObjIDSender = self, name = "rebuildCancelled" }		
		end
	end
	
end




-- KEEPING THIS AROUND JUST IN CASE
	-- This is the state to tell Burno that the cart is broken.  Don't  need it due to timers and idles.
	--if (msg.iState == 0) then                     
	
	-- The cart(me) just broke
	--	local Burno = self:GetParentObj().objIDParent        

	--	if( Burno ~= nil and Burno:Exists() ) then

	--	 	Burno:NotifyObject{ ObjIDSender = self, name = "rebuildBroken" }
			
	--	end
	--end