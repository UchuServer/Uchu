require('o_mis')

--------------------------------------------------------------
--Rescue Tourists Mission = 136
--Pet Rancher = 3257
--“tourist” = 3386
--------------------------------------------------------------
function onStartup(self) -- The object with this script component is defined as "self" when the script first starts up. (duplicated in PET_MISSION_NPCS_CLIENT.lua)
    local PID = {}
     PID[1] = "PlayerObjects"
     self:SetVar("PID", PID ) 
end

function IsLocalCharacter(target) -- Check to see if the "local character" and the "target" which is triggering this are the same (duplicated in PET_MISSION_NPCS_CLIENT.lua)
    return GAMEOBJ:GetLocalCharID() == target:GetID() -- CLIENT ONLY Get the player object, check to see if it's the same as the ID of the target (duplicated in PET_MISSION_NPCS_CLIENT.lua)
end


function onGetOverridePickType(self, msg) -- Get the Pick Type (cursor clicking options) for the script object, in preparation of changing it.

    msg.ePickType = 14 -- Set the Pick Type to 14 (duplicated in PET_MISSION_NPCS_CLIENT.lua)
	return msg -- Send Pick Type 14 back to the script object (duplicated in PET_MISSION_NPCS_CLIENT.lua)
    
end

function onUse(self, msg)

   local targetID = msg.user 				-- Target OBJ ID 
   
                --GGJ No idea what this section does.
               for  i = 1, table.maxn(self:GetVar("PID")) do
                   if self:GetVar("PID")[i] == msg.user:GetName().name then
                     self:SetVar("PlayerFound" , true ) 
                     break
                   else
                      self:SetVar("PlayerFound" , false ) 
                   end
               end
            
   
                local myMissionState = targetID:GetMissionState{missionID = 136}.missionState --Find out what the mission state is for the Rescue Tourists Mission, and save it as "myMissionState"
               

                if myMissionState == 1 or myMissionState == 9 then -- STATE_AVAILABLE --GGJ Player has not talked to Pet Rancher yet to initiate Mission. Mission is "Available" (1) or "Completed & Available" (9)
                  targetID:DisplayTooltip{ bShow = true, strText = "I'M LOST. HAVE YOU SEEN THE PET RANCHER?", iTime = 0 } --GGJ Test message
                  
                end --GGJ Quit here. You can't rescue her yet.
                
                if myMissionState == 2  or myMissionState == 10  and  not self:GetVar("PlayerFound") then -- STATE_ACTIVE --GGJ Mission is active. Mission is "Accepted" (2) or "Completed & Accepted" (10)
                  targetID:DisplayTooltip{ bShow = true, strText = "YOU SAVED ME!", iTime = 0 } --GGJ Test message
                  targetID:UpdateMissionTask{target = self, value = 1, taskType= "kill" } --GGJ Mission is set up as a "kill" mission; to "kill" 3 tourists (3386). This line sends "1 kill" to self (3386), which increments the mission task by 1.
                  local num = table.maxn(self:GetVar("PID")) + 1 --GGJ No idea.
                
                   
                    self:SetVar("PID."..num,  msg.user:GetName().name ) 
                    
                    -- self:Teleport{ x= -167.09, y= 206.77, z= 327.38, w= 1.0, 0, -1 } -- GGJ - return tourist to camp. This doesn't work. It just makes her disappear. Should be done in client instead?
               
               end  
               
                if myMissionState == 4 or myMissionState == 12 then -- READY_TO_COMPLETE --GGJ Player has just clicked on 3 tourists and is trying to save a 4th; Mission is "Ready to Complete" (4) or "Completed & Ready to Completed" (12)
                 targetID:DisplayTooltip{ bShow = true, strText = "YOU SAVED 3. REPORT BACK BEFORE SAVING MORE.", iTime = 0 } --GGJ Test message
                end --GGJ Quit here. You can't rescue her until you have reported back to the Pet Rancher.             
              
 
end 

-- Script by Trent, Comments by Geoff (to help learn LUA)