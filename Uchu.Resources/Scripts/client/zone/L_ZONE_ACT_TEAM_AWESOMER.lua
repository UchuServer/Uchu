--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('o_mis')

--------------------------------------------------------------
-- Startup
--------------------------------------------------------------
function onStartup(self) 
	GAMEOBJ:GetTimer():AddTimerWithCancel( 5.0, "Go", self )
	self:SetVar("GameStarted", GAMEOBJ:GetSystemTime())
end


--------------------------------------------------------------
-- Timers
--------------------------------------------------------------
onTimerDone = function(self, msg)
    if (msg.name == "Go") then
    
--    LEVEL:SetLights(
--        false,0x0000ff,
--        false,0x0000ff,
--        false,0x0000ff,
--        false,0x808000,
--        false,{0.7,0.7,0.0},
--        true,-700.0,
--        true,-10000.0,
--        false,0x008080,
--        false,"mesh/env/env_sky_won_team_ninetimes.nif"
--	)    
    
	end
end

