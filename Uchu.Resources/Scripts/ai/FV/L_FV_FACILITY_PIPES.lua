--------------------------------------------------------------
--  Server script on the big pipes console
-- just starts fx on it on, the client side script sends a message over when the render component is ready
--  
-- created Brandi... 8/25/10
--------------------------------------------------------------
function onFireEventServerSide(self, msg)
	if msg.args == "startFX" then
	   self:PlayFXEffect{name = "LeftPipeOff", effectID = 2774, effectType = "create"}
	   self:PlayFXEffect{name = "RightPipeOff", effectID = 2777, effectType = "create"}
	   self:PlayFXEffect{name = "imagination_canister", effectID = 2750, effectType = "create"}
	end
end