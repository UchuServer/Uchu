function onCollision (self,msg)

	self:PlayFXEffect{effectType = "down_asharp"}

end

function onOffCollision (self,msg)

	self:PlayFXEffect{effectType = "up"}

end
