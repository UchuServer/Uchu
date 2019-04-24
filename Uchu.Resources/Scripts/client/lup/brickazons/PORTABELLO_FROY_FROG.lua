--require('o_mis')
--require('client/ai/NP/L_NP_NPC')

function onStartup(self)
	--set the vars for interaction. NOTE: any/all of thses are optional
--	SetMouseOverDistance(self, 30)
--	SetProximityDistance(self, 30)
	self:SetProximityRadius{radius = 30}
    --AddInteraction(self, "proximityText", Localize("I'm Froy the frog... ribbit."))
    
end

function onProximityUpdate(self, msg)
	if(msg.status == "ENTER" and msg.objId:GetID() == GAMEOBJ:GetLocalCharID()) then
		self:DisplayChatBubble{wsText = Localize("LUP_PORTOBELLO_INTRO_FROG_CHAT")}
	end
end