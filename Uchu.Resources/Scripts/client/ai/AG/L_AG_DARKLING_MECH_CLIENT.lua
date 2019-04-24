

--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('c_AvantGardens')





--------------------------------------------------------------
-- Startup
--------------------------------------------------------------
function onStartup( self )
    self:SetVar("InvisTime", 10)
    GAMEOBJ:GetTimer():AddTimerWithCancel( self:GetVar("InvisTime")  , "InvisTime", self )
    self:SetVisible{visible = false}
    self:ActivatePhysics{bActive = false}
end



--------------------------------------------------------------
-- Timers
--------------------------------------------------------------
function onTimerDone( self, msg )
	
	if ( msg.name == "InvisTime" ) then
		
		self:SetVisible{visible = true, fadeTime = 1}
	    self:ActivatePhysics{bActive = true}
	end
	
end



