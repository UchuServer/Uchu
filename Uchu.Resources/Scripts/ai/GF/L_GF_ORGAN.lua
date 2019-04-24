--------------------------------------------------------------
-- Server side script for the GF Organ next to Captin Jack
--
-- updated mrb... 9/24/10 -- updated audio and fixed interaction
--------------------------------------------------------------
function onCheckUseRequirements(self, msg)
    if self:GetNetworkVar('bIsInUse') then 
        msg.bCanUse = false
        
        return msg
    end        
end

function onUse(self,msg)
	-- play nd audio
	self:PlayNDAudioEmitter{m_NDAudioEventGUID = "{15d5f8bd-139a-4c31-8904-970c480cd70f}" } -- old {500434f3-3e21-48f6-87d7-928487ff280e}  
	self:SetNetworkVar('bIsInUse', true)
	GAMEOBJ:GetTimer():AddTimerWithCancel( 5  , "reset", self )

    msg.user:PlayAnimation{ animationID = "jig", bPlayImmediate = true }
end 

function onTimerDone(self, msg)
    if msg.name == "reset" then
        self:SetNetworkVar('bIsInUse', false)
    end
end 