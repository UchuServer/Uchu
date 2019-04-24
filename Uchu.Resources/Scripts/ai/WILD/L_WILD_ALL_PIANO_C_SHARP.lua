function onCollision (self,msg)

	self:PlayFXEffect{effectType = "down_csharp"}

end

function onOffCollision (self,msg)

	self:PlayFXEffect{effectType = "up"}

end
