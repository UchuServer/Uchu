
function onStartup(self)
    self:SetVar("bIsDead", false)
end

function onCollisionPhantom(self, msg)

	if ( self:GetVar("bIsDead") == true ) then
		return
	end

	--print("onCollision")

	local target = msg.objectID
	local controlledObject = GAMEOBJ:GetControlledID()
	
	if ( target:GetID() == controlledObject:GetID() ) then
	
		--print ("controlled object")				
		controlledObject:PlayFXEffect{effectID = 1159, effectType = "pickup"}		

		-- assume they will soon collect and delete this object on the server, but just
		-- in case they don't, we need to turn visibility back on after a couple seconds
		self:SetVisible{visible = false, fadeTime = 0}
		GAMEOBJ:GetTimer():AddTimerWithCancel( 2.0 , "visible", self )

		self:SetVar("bIsDead", true)
		
		--print ("down by notify")		
		
		target:VehicleNotifyHitImagination{ pickupID = self }
		--print ("past notify")		
	
	end
end


function onTimerDone(self, msg)

	--print("timer done, going visible again")
	if msg.name == "visible" then
		self:SetVisible{visible = true, fadeTime = 0}
		self:SetVar("bIsDead", false)
	end
	
end