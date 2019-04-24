-------------------------------------------------
--This script is on a trigger just after Epsilon in the AG Battlefield
-- if the player is a WASD player, then it shows the Q&E tutorial again.
-- created brandi 6/28/10
-- updated abeechler ... 7/16/11 - clamp display of tutorial to prevent spamming
----------------------------------------------------

function onCollisionPhantom(self,msg)
	local player = msg.senderID
	if player:GetID() == GAMEOBJ:GetControlledID():GetID() then
		UI:SendMessage("ToggleControlsTutorial", { {"bDisplay", true} , { "type", "Rotate" } } )
		-- remove object on client to prevent spamming
		GAMEOBJ:DeleteObject(self)
	end
end
