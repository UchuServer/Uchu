--------------------------------------------------------------
-- Script for Forbidden Valley Guards.  There are two guards at the gate in FV.  If you emote a "Roar" animation in front of either, the other laughs.
-- edited eb... 6/25/10 Changed prox distance so you were only ever close to one of the guards
--------------------------------------------------------------

require('o_mis')
require('client/ai/AG/L_AG_NPC')

function onStartup(self)

	--set the vars for interaction. NOTE: any/all of thses are optional
	SetMouseOverDistance(self, 30)
	SetProximityDistance(self, 9)
	
	-- When approaching
    AddInteraction(self, "proximityAnim", "prox")
	
    -- Click on speech	
	AddInteraction(self, "interactionText", Localize("GUARD_NINJA_CLICK_A")) --Only those wearing the proper attire may pass through the gate.
	AddInteraction(self, "interactionText", Localize("GUARD_NINJA_CLICK_B")) --Turn back now traveler, please don't smash any of our shrines around the gate.
	AddInteraction(self, "interactionText", Localize("GUARD_NINJA_CLICK_C")) --You are very brave for coming this far.
	AddInteraction(self, "interactionText", Localize("GUARD_NINJA_CLICK_D")) --Somewhere beyond this gate lies the Great Tree.
	AddInteraction(self, "interactionText", Localize("GUARD_NINJA_CLICK_E")) --Those floating platforms beneath the bridge look very unstable.
	
	-- Proximity speech
	AddInteraction(self, "proximityText", Localize("GUARD_NINJA_RANDOM_A"))  --Look, another hopeless traveler.
	AddInteraction(self, "proximityText", Localize("GUARD_NINJA_RANDOM_B"))  --I haven't seen a visitor here for a long time.
	AddInteraction(self, "proximityText", Localize("GUARD_NINJA_RANDOM_C"))  --So many have tried to enter the Forbidden Valley, so few have survived.
	AddInteraction(self, "proximityText", Localize("GUARD_NINJA_RANDOM_D"))  --This beast will crush any traveler without the proper attire.
	AddInteraction(self, "proximityText", Localize("GUARD_NINJA_RANDOM_E"))  --I heard some pirates made it into the valley.

end