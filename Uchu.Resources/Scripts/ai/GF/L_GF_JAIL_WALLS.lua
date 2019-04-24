function onStartup(self,msg)
	--print("Jail0"..self:GetVar("Wall"))
end

function onRebuildComplete(self,msg)
	local PirateSpawner = LEVEL:GetSpawnerByName("Jail0"..self:GetVar("Wall"))
	local CaptainSpawner = LEVEL:GetSpawnerByName("JailCaptain0"..self:GetVar("Wall"))
	PirateSpawner:SpawnerDeactivate()
	CaptainSpawner:SpawnerDeactivate()	
end

function onRebuildNotifyState(self, msg)
	--print(msg.iState)
	if (msg.iState == 4) then
		
		--print("Reset Spawners")
		
		local PirateSpawner = LEVEL:GetSpawnerByName("Jail0"..self:GetVar("Wall"))
		local CaptainSpawner = LEVEL:GetSpawnerByName("JailCaptain0"..self:GetVar("Wall"))
		
		PirateSpawner:SpawnerReset()
		CaptainSpawner:SpawnerReset()
		
		PirateSpawner:SpawnerActivate()
		CaptainSpawner:SpawnerActivate()
	end
end