
-- REBUILD NOTIFY

function onRebuildNotifyState(self, msg)

	if (msg.iState) == 3 then
	
		print("State Competed by "..msg.player:GetName().name)
		GAMEOBJ:GetZoneControlID():NotifyObject{name = "reBuild" , ObjIDSender = msg.player }
		
	end
	
end


function onDie(self,msg)
    
	if msg.killerID ~= nil then
	
		print("I was killed by "..msg.killerID:GetName().name)
		GAMEOBJ:GetZoneControlID():NotifyObject{name = "SmashQB" , ObjIDSender = msg.killerID }
	end
	
end


	
		
