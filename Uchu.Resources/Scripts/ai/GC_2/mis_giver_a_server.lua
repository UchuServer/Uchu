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
           
            
              if self:GetVar("Zone") == 0 then
              self:SetVar("Zone", 1 ) 
              GAMEOBJ:GetZoneControlID():SetVar("Mission_A_State","started")
              end 
              if GAMEOBJ:GetZoneControlID():GetVar("Mission_A_State") == "ADONE" then
                GAMEOBJ:GetZoneControlID():SetVar("Mission_A_State","travleA")
                local tar =  GAMEOBJ:GetObjectByID(GAMEOBJ:GetZoneControlID():GetVar("Trigger_A_ID"))    
                setState("sendAway",tar)
                
                
              end
           

            
end


   


