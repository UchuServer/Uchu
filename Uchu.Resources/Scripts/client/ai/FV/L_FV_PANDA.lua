--------------------------------------------------------------
-- Client side script on the panda pet
-- this script controls the icon and interactablity of the panda
-- only the player who spawned the panda show see the interact icon above the panda and be able to tame it

-- created by Steve... 
-- created from Brandi's Lion script... 2/17/10
--------------------------------------------------------------


function onStartup(self)

    self:SetVar("Timer", 0.5)
    
end



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
			-- if the player is the player who spawned the pet, then set the panda to be interactable to that player
			--print("on panda player is "..tostring(player:GetID()))
			--print("on panda tamer is  "..tostring(self:GetVar("pandatamer")))
			if player:GetFlag{iFlagID = 81}.bFlag == true and self:GetVar("localPandaTamer") == player:GetID() then
				
				
				msg.ePickType = 14 
				-- Interactive pick type 
				
			else
                
                msg.ePickType = -1
                
            end
            
		else
			
			--if the player is not the player who spawned the panda, the player can't interact with the panda
			msg.ePickType = -1
            GAMEOBJ:GetTimer():AddTimerWithCancel( self:GetVar("Timer")  , "Time", self )
			
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
	
	local pandatamer = msg.tableOfVars["pandatamer"]
	
		if pandatamer then    
		
		    local player = GAMEOBJ:GetControlledID()
		    
            if player:GetID() == pandatamer then 
            
                self:SetVar("localPandaTamer",player:GetID())
		        self:RequestPickTypeUpdate()
		        
		    end
		    
		    
		    
		end
   
	
	
end



function onTimerDone(self, msg)

   if msg.name == "Time" then
   
        self:RequestPickTypeUpdate()
        
    end
end