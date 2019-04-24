
-------------------------------------------------------------
-- Deactivate spawner networks when kill switch is activated
-- Updated 3/26 Darren McKinsey
-------------------------------------------------------------

-- table of spawner networks
-- local spawnerNames = {"strombieA","strombieB","strombieC","strombieD","mechA","mechB","spiderA","spiderB","spiderC"} --,"wallEffect_03"}

-- msg sent from switch in level
-- function onFireEvent(self, msg)
	-- print("Switch On!!!!")
	-- for i,spawnerName in ipairs (spawnerNames) do
		-- local spawner = LEVEL:GetSpawnerByName(spawnerName)
		-- if spawner then
			-- spawner:SpawnerDestroyObjects{bDieSilent = false}
			-- spawner:SpawnerDeactivate()
		-- end			
	-- end
-- end

