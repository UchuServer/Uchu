function onCollision (self,msg)

	self:PlayFXEffect{effectType = "down_dsharp"}

end

function onOffCollision (self,msg)

	self:PlayFXEffect{effectType = "up"}

end
