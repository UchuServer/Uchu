function onCollision (self,msg)

	self:PlayFXEffect{effectType = "down_c2sharp"}

end

function onOffCollision (self,msg)

	self:PlayFXEffect{effectType = "up"}

end
