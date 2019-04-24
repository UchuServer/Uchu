local color = ''


function onNotifyObject(self, msg) 
	--print("i was notified but the message name was wrong")
	if ( msg.name == "PlayGameFX" ) then
		--print("square was notified")
		color = self:GetVar('color') 
		print("square "..color)
		self:PlayFXEffect{name = 'color', effectID = 229, effectType = "change"}
		GAMEOBJ:GetTimer():AddTimerWithCancel(1, "StopFX", self )
	end
	if ( msg.name == "PlayerFX" ) then
		print("square was notified")
		color = self:GetVar('color') 
		print("square "..color)
		self:PlayFXEffect{name = 'color', effectID = 379, effectType = "create"}
		GAMEOBJ:GetTimer():AddTimerWithCancel(1, "StopFX", self )
	end
end

function onTimerDone(self,msg)
	if msg.name == "StopFX" then
		print("I SAID STOP")
		color = self:GetVar('color') 
		self:StopFXEffect{ name = 'color' }
	end

end