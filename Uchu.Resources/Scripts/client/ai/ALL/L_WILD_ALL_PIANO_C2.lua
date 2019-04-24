require('o_mis')
require('client/ai/NP/L_NP_NPC')

function onStartup(self)
	                       
	SetMouseOverDistance(self, 60)
	AddInteraction(self, "mouseOverAnim", "click_c2")

end

function onClientUse(self)

	self:PlayFXEffect{effectType = "down_c2"}

end

function onCollisionPhantom (self,msg)

	self:PlayFXEffect{effectType = "down_c2"}

end

function onOffCollisionPhantom (self,msg)

	self:PlayFXEffect{effectType = "up"}

end
