function onSetIconAboveHead(self, msg)    
    local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
    
    if not self:GetVar('bHide') and not player:CheckPrecondition{PreconditionID = 46}.bPass and not player:CheckPrecondition{PreconditionID = 47}.bPass then      
        self:SetVar('bHide', true)
        GAMEOBJ:GetTimer():AddTimerWithCancel( 0.1 , "startup", self )
    end       
end 

function onFireEvent(self, msg)
    if msg.args == 'showIcon' then    
        --print('showIcon')
        self:SetIconAboveHead{bIconOff = false, iconMode = 0, iconType = 1}
    end
end 

function onTimerDone(self, msg)
	if (msg.name == "startup") then 
        --print('hideIcon')
        self:SetIconAboveHead{bIconOff = true, iconMode = 0, iconType = 1}	    	
	end
end 