function onCollision (self,msg)

	self:PlayFXEffect{effectType = "down_d"}

end

function onOffCollision (self,msg)

	self:PlayFXEffect{effectType = "up"}

end
