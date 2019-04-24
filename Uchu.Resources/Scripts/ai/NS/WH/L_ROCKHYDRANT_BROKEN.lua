--------------------------------------------------------------
-- Server script for turning the bouncer geysers on and off.
--
-- updated jnf... 01/17/11
--------------------------------------------------------------

function onStartup(self)
	
	self:PlayFXEffect{ name = "water", effectID = 4737, effectType = "water" }
	GAMEOBJ:GetTimer():AddTimerWithCancel( 0.5, "bouncerOn",self )
	GAMEOBJ:GetTimer():AddTimerWithCancel( 10, "KillBroken",self )
	
end

function onTimerDone(self,msg)

	if (msg.name == "bouncerOn") then
	
		local hydrant = "hydrant0"..self:GetVar('hydrant')
		
		local bouncerObjs = self:GetObjectsInGroup{ group = hydrant, ignoreSpawners = true}.objects		
				
		GAMEOBJ:GetTimer():AddTimerWithCancel( 1, "WaterOff",self )
		
		-- Turn bouncer on so people can use it
		if not bouncerObjs[1] then return end
		
		bouncerObjs[1]:NotifyObject{name = "enableCollision"}		
	
	elseif (msg.name == "WaterOff") then
	
		self:StopFXEffect{ name = "water", effectID = 4737, effectType = "water" }
		
		local hydrant = "hydrant0"..self:GetVar('hydrant')
		
		local bouncerObjs = self:GetObjectsInGroup{ group = hydrant, ignoreSpawners = true}.objects
		
		-- Turn bouncer off
		if not bouncerObjs[1] then return end
		
		bouncerObjs[1]:NotifyObject{name = "disableCollision"}
		
	elseif (msg.name == "KillBroken") then	
		
		GAMEOBJ:DeleteObject(self)
		
	end

end


