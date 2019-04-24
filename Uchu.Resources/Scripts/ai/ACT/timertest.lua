

function onStartup(self)
	print("Starting timer")
	self:ActivityTimerSet{name = "testTimer",
							duration = 30,
							updateInterval = 5}
end

function onActivityTimerUpdate(self, msg)
	print("Timer " .. msg.name .. " sent us an update.")
	print("  " .. tostring(msg.timeElapsed) ..
			" seconds have passed, " ..
 			tostring(msg.timeRemaining) .. 
			" seconds remain.")
end

function onActivityTimerDone(self, msg)
	print("Timer " .. msg.name .. " has ended, " .. 
			tostring(msg.timeElapsed) ..
			" seconds passed since it was set.")
end
