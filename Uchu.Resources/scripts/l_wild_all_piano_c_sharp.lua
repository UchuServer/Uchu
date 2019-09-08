require('o_mis')
require('L_NP_NPC')

function onStartup(self)
	                       
	SetMouseOverDistance(self, 60)
	AddInteraction(self, "mouseOverAnim", "click_csharp")

end

function onClientUse(self)

	self:PlayFXEffect{effectType = "down_csharp"}

end

function onCollisionPhantom (self,msg)

	self:PlayFXEffect{effectType = "down_csharp"}

end

function onOffCollisionPhantom (self,msg)

	self:PlayFXEffect{effectType = "up"}

end
