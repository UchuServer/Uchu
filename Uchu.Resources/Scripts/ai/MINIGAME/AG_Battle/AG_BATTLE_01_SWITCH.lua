-------------------------------------------------------------
-- Deactivate spawner networks when kill switch is activated
-- Updated 3/26 Darren McKinsey
-------------------------------------------------------------

local spawnerNames = {"strombieA","strombieB","strombieC","strombieD","mechA","mechB","spiderA","spiderB","spiderC"}
local timerTick = 0

function onStartup(self) 
	--print("startup")
end

function onObjectActivated(self, msg)
	--print("object activated")
	-- self:ActivityTimerSet{ name = "ticks",  duration = 10, updateInterval = 1 }
	GAMEOBJ:GetTimer():AddTimerWithCancel( .5 , "ticks", self )
end

function onTimerDone(self, msg)  
	--print("got update")
	if msg.name == "ticks" then
		--print("ticks")
		timerTick = timerTick + 1
		local i = timerTick
		local spawnerName = spawnerNames[i]
		local spawner = LEVEL:GetSpawnerByName(spawnerName)
		if spawner then
			--print("got spawner")
			spawner:SpawnerDestroyObjects{bDieSilent = false}
			spawner:SpawnerDeactivate()
		end
		if timerTick <= #spawnerNames then
			GAMEOBJ:GetTimer():AddTimerWithCancel( 1 , "ticks", self )
		end
	end
end 






	-- for i,spawnerName in ipairs (spawnerNames) do
		-- local spawner = LEVEL:GetSpawnerByName(spawnerName)
		-- if spawner then
			-- spawner:SpawnerDestroyObjects{bDieSilent = false}
			-- spawner:SpawnerDeactivate()
		-- end			
	-- end