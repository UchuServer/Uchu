
CONSTANTS = {}
CONSTANTS["CLIENT_TOOLTIP_MISSION_ACCEPT"] = 0
CONSTANTS["CLIENT_TOOLTIP_MISSION_COMPLETE"] = 1
require('o_mis')


function onStartup(self)
    self:SetVar("Zone", 0 ) 
   
end  
  
    
    
 
-- @TODO:ISSUE - Lua cannot send client/single messages to non client OBJID's. Need a 
function onMissionDialogueOK(self, msg)
         
            local user = msg.responder
            if  self:GetLOT{}.objtemplate ==  3043 then
            
            
            end 
end
