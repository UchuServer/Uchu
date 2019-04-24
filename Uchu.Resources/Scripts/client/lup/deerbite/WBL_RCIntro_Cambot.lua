require('o_mis')
require('client/ai/NP/L_NP_NPC')

function onStartup(self)

	--set the vars for interaction. NOTE: any/all of thses are optional
	SetMouseOverDistance(self, 30)
	SetProximityDistance(self, 30)

	AddInteraction(self, "proximityText", Localize("LUP_RC_Cambot"))	
    --AddInteraction(self, "interactionText", Localize("WBL_ROBOT_CITY_CONSTRUCTBOT_INTERACT"))
	
    --AddInteraction(self, "proximityText", Localize("WBL_ROBOT_CITY_CONSTRUCTBOT_PROXIMITY"))
    
end


--function onGetOverridePickType(self, msg)

--	msg.ePickType = 14	--interactive type
--	return msg

--end