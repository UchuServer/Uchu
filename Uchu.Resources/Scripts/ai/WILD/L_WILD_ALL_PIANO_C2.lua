function onCollision (self,msg)

	self:PlayFXEffect{effectType = "down_c2"}

end

function onOffCollision (self,msg)

	self:PlayFXEffect{effectType = "up"}

end
