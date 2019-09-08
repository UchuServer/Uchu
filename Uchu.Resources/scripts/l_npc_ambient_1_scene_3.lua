require('o_mis')
require('L_NP_NPC')

function onStartup(self)
	                       
    -- register ourself to be instructed later
    registerWithZoneControlObject(self)
    	                       
	--set the vars for interaction. NOTE: any/all of thses are optional
	SetMouseOverDistance(self, 30)
	SetProximityDistance(self, 30)
	
	AddInteraction(self, "interactionText", Localize("NPC_NP_AMB_I_HEARD_THE_BEASTIE_BLOCKS_WERE_GOOD"))
	AddInteraction(self, "interactionText", Localize("NPC_NP_AMB_THIS_CONCERT_BETTER_BE_GREAT"))
	AddInteraction(self, "interactionText", Localize("NPC_NP_AMB_DID_YOU_SEE_THAT_OLD_MAN_OVER_THERE?"))
	
    AddInteraction(self, "proximityText", Localize("NPC_NP_AMB_GO_BEASTIE_BLOCKS!"))
	AddInteraction(self, "proximityText", Localize("NPC_GENERIC_YEAH!"))
	AddInteraction(self, "proximityText", Localize("NPC_GENERIC_HEY_THERE"))
	AddInteraction(self, "proximityText", Localize("NPC_NP_AMB_WHY_IS_SHERLAND_SO_OLD?"))
    
end

--------------------------------------------------------------
-- Called when this object is ready to render
--------------------------------------------------------------
function onRenderComponentReady(self, msg)

	-- let the zone control object know we are ready to be instructed
	GAMEOBJ:GetZoneControlID():FireEvent{ senderID=self, args="SceneActorReady" }

end
