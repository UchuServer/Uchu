--------------------------------------------------------------
-- client side script on the imagination shrine
-- 
-- created by brandi.. 6/13/11
--------------------------------------------------------------

require('02_client/Map/General/L_SET_INTERACT_WITH_VAR_CHECK')

function onCheckUseRequirements(self, msg)    
		-- to make sure the player isn't interacting with the object just to quickbuild it
	-- state 2 means the quickbuild is complete
	if self:GetRebuildState{}.iState == 2 then
		
		baseCheckUseRequirements(self,msg)
		return msg
		
	end
end

