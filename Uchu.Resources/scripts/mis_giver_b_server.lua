--///////////////////////////////////////////////////////////////////////////////////////

--//            Team Awesomer NPC -- SERVER Script

--///////////////////////////////////////////////////////////////////////////////////////

 

CONSTANTS = {}
CONSTANTS["CLIENT_TOOLTIP_MISSION_ACCEPT"] = 0
CONSTANTS["CLIENT_TOOLTIP_MISSION_COMPLETE"] = 1
require('o_mis')
require('State')

function onStartup(self)
    self:SetVar("Zone", 0 ) 
   
end  
  
    
    
 
-- @TODO:ISSUE - Lua cannot send client/single messages to non client OBJID's. Need a 
function onMissionDialogueOK(self, msg)
            -- get the user
            local user = msg.responder
            -- tell zone control to prep the player to race
           
            

              if GAMEOBJ:GetZoneControlID():GetVar("MissionDone") and self:GetVar("Zone") == 0 then
                  self:SetVar("Zone", 1 )
                   PoS = getMeanderPoint(self)
                   RESMGR:LoadObject { objectTemplate = 2897  , x = PoS.x , y = PoS.y , z = PoS.z ,owner = self } 
                   user:CastSkill{skillID = 72 } 
              end
           

            
end


   


