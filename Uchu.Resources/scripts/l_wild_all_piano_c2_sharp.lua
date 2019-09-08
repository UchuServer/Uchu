require('o_mis')
require('L_NP_NPC')

function onStartup(self)
	                       
	SetMouseOverDistance(self, 60)
	AddInteraction(self, "mouseOverAnim", "click_c2sharp")

end

function onClientUse(self)

	self:PlayFXEffect{effectType = "down_c2sharp"}

end

function onCollisionPhantomn (self,msg)

	self:PlayFXEffect{effectType = "down_c2sharp"}

end

function onOffCollisionPhantom (self,msg)

	self:PlayFXEffect{effectType = "up"}

end
