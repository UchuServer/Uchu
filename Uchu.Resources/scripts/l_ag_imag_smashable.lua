require('o_mis')

function onDie(self, msg)

    local BobMissionStatus = msg.killerID:GetMissionState{missionID = 173}.missionState

-- If the spaceman bob mission is complete.
	if ( BobMissionStatus >= 8 ) then
		local amount = math.random(1, 2)

		for i = 1, amount, 1 do
			local newID = GAMEOBJ:GenerateSpawnedID()
			self:DropLoot{ itemTemplate = 935, owner = msg.killerID, lootID = newID, rerouteID = msg.killerID, sourceObj = self }
		end
    end
    crateAnimal(self)

end

function crateAnimal(self)

    local mypos = self:GetPosition().pos 
-- How often do we spawn weird creatures
    local funnychance = math.random(0, 25)
-- What kind of creatures do we want?  Will be expanded in the future.
    local animaltype = 1

    if funnychance == 1 then
        if animaltype == 1 then
            -- Spawn the Crate Chicken
            RESMGR:LoadObject { objectTemplate = 8114 , x = mypos.x , y = mypos.y , z = mypos.z ,owner = self, bIsSmashable = false }
        end
    end
    
end
