
require('client/ai/AG/L_AG_NPC')

function onStartup(self)
	                       
	--set the vars for interaction. NOTE: any/all of thses are optional
	SetMouseOverDistance(self, 30)
	SetProximityDistance(self, 30)
	
	-- When mousing over
	AddInteraction(self, "mouseOverAnim", "salute")
	
	-- Proximity speech
	AddInteraction(self, "proximityText", Localize("NINJAGO_GUARD3_CHAT1"))  
	AddInteraction(self, "proximityText", Localize("NINJAGO_GUARD3_CHAT2"))  
	AddInteraction(self, "proximityText", Localize("NINJAGO_GUARD3_CHAT3")) 
	AddInteraction(self, "proximityText", Localize("NINJAGO_GUARD3_CHAT4")) 
	AddInteraction(self, "proximityText", Localize("NINJAGO_GUARD3_CHAT5")) 
	AddInteraction(self, "proximityText", Localize("NINJAGO_GUARD3_CHAT6")) 
	
end


