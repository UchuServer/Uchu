require('o_mis')
require('L_NP_NPC')

function onStartup(self)
	                       
	SetMouseOverDistance(self, 60)
	AddInteraction(self, "mouseOverAnim", "click_b")

end

function onClientUse(self)

	self:PlayFXEffect{effectType = "down_b"}

end

function onCollisionPhantom (self,msg)

	self:PlayFXEffect{effectType = "down_b"}

end

function onOffCollisionPhantom (self,msg)

	self:PlayFXEffect{effectType = "up"}

end
