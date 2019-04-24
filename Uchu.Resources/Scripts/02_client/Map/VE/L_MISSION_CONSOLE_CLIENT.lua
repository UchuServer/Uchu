--------------------------------------------------------------
-- Server script for on Epsolion in the venture explorer battle instance

-- created by brandi.. 11/10/10
-- updated brandi.. 11/12/10 - to acount for all the different missions
-------------------------------------------------------------- 

-------------------------------------------
-- see if the player has used the console before 
-------------------------------------------
function onCheckUseRequirements(self, msg)
	local player = msg.objIDUser
	if not player:Exists() then return end
	-- get the number of the console that is set in happy flower
	local number = self:GetVar("num")
	-- use the number to create the flag number
	local flag = tonumber("101"..number)
	--check to make sure the player hasn't used the console yet
	
	if player:GetFlag{iFlagID = flag}.bFlag then   
		-- set the console to unuseable
		msg.bCanUse = false
		return msg	
	end
	local repeatmissionState = player:GetMissionState{missionID = 1225}.missionState
	if repeatmissionState ~= 10	and repeatmissionState ~= 2
					and player:GetMissionState{missionID = 1220}.missionState ~= 2 then
		-- set the console to unuseable
		msg.bCanUse = false
		return msg	
	end
end

----------------------------------------------
-- sent when the object checks it's pick type
----------------------------------------------
function onGetPriorityPickListType(self, msg)  
    local myPriority = 0.8
  
    if ( myPriority > msg.fCurrentPickTypePriority ) then    
        msg.fCurrentPickTypePriority = myPriority 
        
        local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
		-- get the number of the console that is set in happy flower
		local number = self:GetVar("num")
		-- use the number to create the flag number
		local flag = tonumber("101"..number) 	
		
		if player:GetFlag{iFlagID = flag}.bFlag  then   
			msg.ePickType = -1
			return msg  			
		end
		local repeatmissionState = player:GetMissionState{missionID = 1225}.missionState
		if repeatmissionState ~= 10	and repeatmissionState ~= 2
						and player:GetMissionState{missionID = 1220}.missionState ~= 2 then
			msg.ePickType = -1
			return msg  
		end

        msg.ePickType = 14    -- Interactive pick type     
        return msg  
    end  
end

----------------------------------------------
-- when someone notifies the client update the picktype
----------------------------------------------
function onNotifyClientObject(self, msg) 
	self:RequestPickTypeUpdate{}
end