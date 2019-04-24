require('client/general/MOD_BUILD_BORDER_EFFECTS')

function onStartup(self)
	self:SetVar("hasPlayed", false)
end

function onModularBuildFinish(self, msg)

	if not self:GetVar("hasPlayed") then
		self:SetVar("hasPlayed", true)				
		self:ActivateNDAudioMusicCue{m_NDAudioMusicCueName = "Spaceship_Post-Rocket"}		
	end
end