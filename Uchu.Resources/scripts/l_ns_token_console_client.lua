--------------------------------------------------------------
-- Client side script on the console behind the paradox rep.
-- relies on the general token console script 

-- created by brandi... 4/14/11
--------------------------------------------------------------

require('L_TOKEN_CONSOLE_CLIENT')

function onStartup(self,msg)
	-- because this is a quickbuild, the preconddition has to be set in script, otherwise it would apply to the quickbuild itself
	self:SetVar("CheckPrecondition","47;187;185")
end
		
-- checks to see if the player is eligible to use the console
function onCheckUseRequirements(self,msg)

	-- to make sure the player isn't interacting with the object just to quickbuild it
	-- state 2 means the quickbuild is complete
	if self:GetRebuildState{}.iState == 2 then
		
		baseCheckUseRequirements(self,msg)
		return msg
		
	end

end

function onGetInteractionDetails(self, msg)
	baseGetInteractionDetails(self,msg)
    
    return msg
end



