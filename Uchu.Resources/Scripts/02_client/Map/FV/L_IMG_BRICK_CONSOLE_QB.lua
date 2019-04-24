--------------------------------------------------------------
--  client script on the consoles that are built for the blue brick puzzle in FV
--  
-- created Brandi... 8/25/10
-- updated Brandi... 1/10/11 - allowed players on the CP mission to interact with the console again if it has already been interacted with
--------------------------------------------------------------

--------------------------------------------------------------
-- server script sends a message to this script
--------------------------------------------------------------
function onScriptNetworkVarUpdate(self,msg)
    for k,v in pairs(msg.tableOfVars) do 
        if k == "used" then
            self:RequestPickTypeUpdate()
        end
    end
end


function onGetPriorityPickListType(self, msg)  
    local myPriority = 0.8
  
    if ( myPriority > msg.fCurrentPickTypePriority ) then    
        msg.fCurrentPickTypePriority = myPriority 
        
        local player = GAMEOBJ:GetControlledID()
        
        -- the player used the console, set it so the player cant use it again, unless they are on the mission from CP
        if self:GetNetworkVar("used") and player:GetMissionState{missionID = 1302}.missionState ~= 2 then
            msg.ePickType = -1
        else
            msg.ePickType = 14    -- Interactive pick type     
        end
    end  
  
    return msg      
end 