function onCollision (self,msg)

	self:PlayFXEffect{effectType = "down_b"}

end

function onOffCollision (self,msg)

	self:PlayFXEffect{effectType = "up"}

end
