--------------------------------------------------------------
-- script on the ninjas in the tree to hide their icons until the player gets the mission from the dojo master to talk to them
-- 
-- updated Brandi... 3/19/10
--------------------------------------------------------------

-- when the map loads, hide the mission icons of the ninjas in the tree is the player is not on the mission to talk to them
function onSetIconAboveHead(self, msg)    

    local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
    
	--see if the icons arent already hidden, and check if the player is has the precondition set
    if not self:GetVar('bHide') and not player:CheckPrecondition{PreconditionID = 104}.bPass and not player:CheckPrecondition{PreconditionID = 105}.bPass then  
	
		self:SetVar('bHide', true) --set a value to true so this wont spam
        GAMEOBJ:GetTimer():AddTimerWithCancel( 0.1 , "startup", self )
		
    end       
end 

-- fire event comes from the dojo master when you take the mission to take his mission to talk to the other ninjas
function onFireEvent(self, msg)

    if msg.args == 'showIcon' then   
	
        --print('showIcon')
        -- show the mission icon over the npcs head
        self:SetIconAboveHead{bIconOff = false, iconMode = 0, iconType = 1}
		
    end
	
end 

function onTimerDone(self, msg)

	if (msg.name == "startup") then 
	
        --print('hideIcon')
        -- hide the mission icon over the npcs head
        self:SetIconAboveHead{bIconOff = true, iconMode = 0, iconType = 1}	 
		
	end
	
end 

function onCheckUseRequirements(self,msg)
    
    local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
	
	if not player:CheckPrecondition{PreconditionID = 104}.bPass and not player:CheckPrecondition{PreconditionID = 105}.bPass then
	
		-- if the player doesnt have the talk to the tree ninjsa mission, show a chat bubble that says talk to the dojo master
		self:DisplayChatBubble{wsText = player:CheckPrecondition{PreconditionID = 104}.FailedReason}
		msg.bCanUse = false
		return msg
		
	end
	
end