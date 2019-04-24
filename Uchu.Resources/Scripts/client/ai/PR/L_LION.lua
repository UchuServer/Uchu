--------------------------------------------------------------
-- Client side script on the lion pet
-- this script controls the icon and interactablity of the lion
-- only the player who spawned the lion show see the interact icon above the lion and be able to tame it

-- created by Brandi... 2/17/10
--------------------------------------------------------------

function onGetPriorityPickListType(self, msg)

	-- if the pet is someones tamed pet, ignore the rest of the script
	if self:IsPetWild{}.bIsPetWild == false then
		return
	end
	
	local myPriority = 0.8
	
    if ( myPriority > msg.fCurrentPickTypePriority ) then
	    local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
		if player:Exists() then
		
			msg.fCurrentPickTypePriority = myPriority
			-- if the player is the player who spawned the pet, then set the lion to be interactable to that player
			--print("on lion player is "..tostring(player:GetID()))
			--print("on lion tamer is  "..tostring(self:GetVar("liontamer")))
			if player:GetFlag{iFlagID = 67}.bFlag == true and self:GetVar("localLionTamer") == player:GetID() then
				
				msg.ePickType = 14 
				-- Interactive pick type 
			
            else
                
                --if the player is not the player who spawned the lion, the player can't interact with the lion
                msg.ePickType = -1
                
            end
            
        end

    end
	
    return msg
	
	
end

-- i have to do the back and forth with FireEventServerSide and FireEventClientSide because SetNetworkVar doesn't serialize properly
 function onStartup(self,msg)
 
	-- doing this because SetNetworkVar is busted and unserialized 
	if self:IsPetWild{}.bIsPetWild == false then
		return
	end
	
 end
 
 function onScriptNetworkVarUpdate(self,msg)
	
	local liontamer = msg.tableOfVars["liontamer"]
	
		if liontamer then    
		
		    local player = GAMEOBJ:GetControlledID()
		    
            if player:GetID() == liontamer then 
            
                self:SetVar("localLionTamer",player:GetID())
		        self:RequestPickTypeUpdate()
		        
		    end
		    
		    
		    
		end
   
	
	
end

		

