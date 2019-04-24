function onStartup (self)
	--print ("Lap trigger starting up")
end

function onFireEvent (self,msg)
	-- --print("received FireEvent from " .. msg.senderID:GetLOT().objtemplate)
-- tell the RaceController that a player collided with it
	local GroupMsg = self:GetObjectsInGroup {group = "RaceController", ignoreSpawners = true}
	for index,object in pairs(GroupMsg.objects) do
		object:NotifyObject {name = "TriggerEntered"}
	end
end




--------------------------------------------------------------
-- On Collision
--------------------------------------------------------------
function onCollision(self, msg)



	--print("****************************** Collision *******************************")


	msg.ignoreCollision = true
	return msg
  
end