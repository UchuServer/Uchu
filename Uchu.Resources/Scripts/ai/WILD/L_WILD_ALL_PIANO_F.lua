function onCollision (self,msg)

	self:PlayFXEffect{effectType = "down_f"}

end

function onOffCollision (self,msg)

	self:PlayFXEffect{effectType = "up"}

end
