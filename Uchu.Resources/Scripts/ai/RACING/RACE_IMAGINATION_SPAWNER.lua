--------------------------------------------------------------
-- Spawns in Imagination Spawner Networks based on player load.
-- this file should be required by the server zone race script.
-- require('ai/RACING/RACE_IMAGINATION_SPAWNER')
-- created mrb... 2/2/10
-- modified Steve Y... 4/12/10
-- modified Devon... 6/14/10 -- corrected spawning based on ZoneLoadedInfo.maxPlayersSoft
--------------------------------------------------------------

function onZoneLoadedInfo(self, msg)
    local playersNum = msg.maxPlayersSoft
	spawnImagination(self, playersNum)
end


function spawnImagination(self, playersNum)
    --print("playersnum = " .. playersNum)

	-- spawn minimum amount of imagination for 1-2 players
	--print("spawning MIN imagination")
    ActivateSpawner(LEVEL:GetSpawnerByName("ImaginationSpawn_Min"))

    if playersNum > 2 then
		-- spawn medium amount of imagination for 3-4 players
		--print("spawning MED imagination")
        ActivateSpawner(LEVEL:GetSpawnerByName("ImaginationSpawn_Med"))
    end
       
    if playersNum > 4 then
		-- spawn maximum amount of imagination for 5-6 players
		--print("spawning MAX imagination")
        ActivateSpawner(LEVEL:GetSpawnerByName("ImaginationSpawn_Max"))
    end
end


function ActivateSpawner(spawner)
    if spawner then
        spawner:SpawnerActivate()
    end
end