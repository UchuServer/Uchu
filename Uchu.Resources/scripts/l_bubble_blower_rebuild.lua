require('o_mis')


local spawnDistance = 0.0
local entityTemplateID = 3363





-- Called anytime the rebuild object's state changes

function onRebuildNotifyState(self, msg)

    -- if we just hit the idle state
	if (msg.iState == 3) then
	

		--self:PlayFXEffect{effectType = "rebuild-complete"}



		self:SetRebuildState{iState = 0}



		-- get the heading and create a vector using spawn distance
		local heading = getHeading(self)
		
		heading.x = heading.x * spawnDistance
		heading.y = heading.y * spawnDistance
		heading.z = heading.z * spawnDistance

		-- add some offset
		local mypos = self:GetPosition().pos 
		mypos.x = mypos.x + heading.x
		mypos.y = mypos.y + heading.y
		mypos.z = mypos.z + heading.z
		
		RESMGR:LoadObject { objectTemplate = entityTemplateID , x = mypos.x , y = mypos.y , z = mypos.z ,owner = self }

	end
	
end     

-- Store the parent in the child
--function onChildLoaded(self, msg)
	
	--if msg.templateID == entityTemplateID then 
	    --storeParent(self, msg.childID)
	--end
--end


