local Laps = 0

function onStartup (self)
	print ("Racing Manager started up")
	self:ActivityTimerSet {name = "LapTimer"}
end

function onNotifyObject (self,msg)
	if msg.name == "TriggerEntered" then
		if Laps <= 3 then
			Laps = Laps + 1
			print ("Lap: " .. Laps)
			print ("Received event from trigger")
			local LapTimeElapsed = self:ActivityTimerGet {name = "LapTimer"}.timeElapsed
			print ("Lap time elapsed: " .. tostring(LapTimeElapsed))
			self:ActivityTimerStop {name = "LapTimer"}
			if Laps < 3 then
				self:ActivityTimerSet {name = "LapTimer"}
			end
		end
	end
end

function onActivityTimerUpdate (self,msg)
	print ("ActivityTimerUpdate received. " .. msg.timeElapsed)
end
