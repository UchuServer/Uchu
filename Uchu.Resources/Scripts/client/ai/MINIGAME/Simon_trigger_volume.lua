local counter = 0
function onCollisionPhantom(self,msg)
	local color = self:GetVar('color') 
	--print("trigger "..color)
	--print(msg.objectID:GetID())
	counter = counter + 1
	--print(counter)
	local manager = self:GetObjectsInGroup{ group = "manager" ,ignoreSpawners = true}.objects[1]
	local oPos = manager:GetPosition().pos
	msg.objectID:Teleport{pos = oPos } --SetPosition{pos = {x=oPos.pos.x , y=oPos.pos.y, z=oPos.pos.z, }}
	manager:NotifyClientObject{ name = "playerCollided" , paramStr = color}

end