function onCollision (self,msg)

	self:PlayFXEffect{effectType = "down_g"}

end

function onOffCollision (self,msg)

	self:PlayFXEffect{effectType = "up"}

end
