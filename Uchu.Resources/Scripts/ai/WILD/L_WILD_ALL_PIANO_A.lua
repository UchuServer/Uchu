function onCollision (self,msg)

	self:PlayFXEffect{effectType = "down_a"}

end

function onOffCollision (self,msg)

	self:PlayFXEffect{effectType = "up"}

end
