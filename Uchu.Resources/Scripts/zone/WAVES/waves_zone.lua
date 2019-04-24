local waveTime = 20.0
local waveNum = 1
--local timeSurvived = 0


function onStartup(self)
    print("starting up")
end

function onRebuildNotifyState(self, msg)
    
	if (msg.iState == 2) then
        print("Wave " .. tostring(waveNum) .. " GO!")
        GAMEOBJ:GetTimer():AddTimerWithCancel(waveTime, "WaveTimer", self)     
        --local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
        
    end
end

function onPlayerDied(self)
    GAMEOBJ:GetTimer():CancelAllTimers(self)
    --timeSurvived = waveNum * 20 - waveTime
    print("You survived for " .. tostring(waveNum - 1) .. " waves!")
end
    
function onTimerDone(self, msg)
    waveNum = waveNum + 1
    print("Wave " .. tostring(waveNum) .. " GO!")    
    GAMEOBJ:GetTimer():AddTimerWithCancel(waveTime, "WaveTimer", self)
    --[[if waveTime == 10.0 then
        print("Only 10 seconds left in the wave!")
    end--]]
end