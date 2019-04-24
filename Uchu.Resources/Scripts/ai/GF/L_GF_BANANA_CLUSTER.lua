--------------------------------------------------------------
-- Script on the banana clusters in Gnarled Forest to kill them after 100 seconds

-- 
-- updated Brandi... 3/19/10
--------------------------------------------------------------

function onStartup(self)

	GAMEOBJ:GetTimer():AddTimerWithCancel( 100, "startup", self )

end

function onTimerDone(self, msg)

	if (msg.name == "startup") then 
	
		self:RequestDie{killerID = self, killType = SILENT}
		
	end
	
end