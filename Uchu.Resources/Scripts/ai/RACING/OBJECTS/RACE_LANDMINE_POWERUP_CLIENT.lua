
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
		
		print ("down by notify")		
		
		target:VehicleNotifyHitWeaponPowerup{ pickupID = self }
		--print ("past notify")	
		
		 local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
    
	   -- if player exists then display the floating text
		if player:Exists() then
			local tTextSize = {x = 0.5, y = 0.1}
		
			local text = "Landmines 3X!"
			print("player exists")
			
			-- yellow text
			player:Request2DTextElement{ni2ElementPosit = {x = 0.5, y = 0.5}, ni2ElementSize = tTextSize, 
									    fFloatAmount = 0.5,  uiTextureHeight = 200, uiTextureWidth = 1300,
										i64Sender = self, fStartFade = 1.0, 
										fTotalFade = 1.25, wsText = text, 
										uiFloatSpeed = 100, iFontSize = 5, 
										niTextColor = {r=0.0 ,g=0.0 ,b=1.0 ,a=0} }
		 end     
	end
end


function onTimerDone(self, msg)

	--print("timer done, going visible again")
	if msg.name == "visible" then
		self:SetVisible{visible = true, fadeTime = 0}
		self:SetVar("bIsDead", false)
	end
	
end