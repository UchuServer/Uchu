require('State')
require('o_StateCreate')
require('o_mis')
require('o_Main')
require('c_Zorillo')


function onStartup(self) 
    
Set = {}

    self:SetVar("Set",Set)
    loadOnce(self) 
    getVarables(self)
    CreateStates(self)
    oStart(self)
end

function onUse(self, msg) 
    
    'self:PlayAnimation(animationID = "turn")
    
    'emote(self, self, "turn")
    'SetMouseOverDistance(self, 100)	                       
   ' 	local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
   '     player:PlayAnimation{animationID = "ship-wheel"}
   '     self:PlayAnimation{animationID = "turn"}
end