--------------------------------------------------------------
-- Client side spaceship modular build script.
--
-- created mrb... 5/20/11 
--------------------------------------------------------------
require('client/general/MOD_BUILD_BORDER_EFFECTS')

function onModularBuildFinish(self, msg)
	if not self:GetVar("bShown") then
		self:SetVar("bShown", true)				
		self:ActivateNDAudioMusicCue{m_NDAudioMusicCueName = "Spaceship_Post-Rocket"}		
	end
end

-- Something touches the phantom object
function onModularBuildEnter(self, msg)    
    if not msg.playerID:GetFlag{iFlagID = 37}.bFlag then 
		-- Modular Build Start tutorial
        msg.playerID:Help{ iHelpID = 37 }      
    end
end 