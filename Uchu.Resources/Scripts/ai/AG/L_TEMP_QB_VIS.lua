function onRebuildNotifyState(self, msg)
	if msg.iState == 2 then
		self:PlayFXEffect{effectType = "rebuild-complete"}
	end
	if msg.iState == "BreakSelf" then
		self:PlayFXEffect{effectType = "rebuild-smash"}
	end

end
