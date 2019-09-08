require('o_mis')

-- Called anytime the rebuild object's state changes

function onRebuildNotifyState(self, msg)

    -- if we just hit the idle state
	if (msg.iState == 2 ) then
		
		local ActivityObj = self:GetObjectsInGroup{ group = "ActivityObj" ,ignoreSpawners = true }.objects
		local spider = getObjectByName(ActivityObj[1], "bossObj")
	
	    spider:PlayFXEffect{name = "moviespotlight", effectID = 503, effectType = "create"}
		--
	    ActivityObj[1]:NotifyObject{name = "RebuildTrigger" }
	end
	
end 
