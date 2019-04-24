function onCollision (self,msg)

	self:PlayFXEffect{effectType = "down_fsharp"}

end

function onOffCollision (self,msg)

	self:PlayFXEffect{effectType = "up"}

end
