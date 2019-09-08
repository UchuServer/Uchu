require('L_BOUNCER_BASIC')
--client-side bouncer script

function onStartup(self)
end

function onBouncerTriggered(self, msg)
	--Hackish fix for the 'bounce collision'
	local player = msg.triggerObj
	local objPos = player:GetPosition().pos
	objPos.y = objPos.y + 1
	--player:DisplayChatBubble{wsText = (objPos.x .. " " .. objPos.y .. " " .. objPos.z)}
	player:SetPosition{pos = objPos}
	
	bounceObj(self, msg.triggerObj)
end
