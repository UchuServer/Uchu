function onCollision (self,msg)

	self:PlayFXEffect{effectType = "down_e"}

end

function onOffCollision (self,msg)

	self:PlayFXEffect{effectType = "up"}

end
