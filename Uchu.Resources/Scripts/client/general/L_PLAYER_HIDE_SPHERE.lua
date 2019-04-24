function onCollisionPhantom(self, msg)
	    
	local localPlayerID = GAMEOBJ:GetLocalCharID()
	local collidingID = msg.senderID:GetID()

	if(localPlayerID ~= collidingID) then
		msg.senderID:SetVisible{visible=false, fadeTime=0}
		--print("onCollisionPhantom")
	end
    
end


function onOffCollisionPhantom(self, msg )

	local localPlayerID = GAMEOBJ:GetLocalCharID()
	local collidingID = msg.senderID:GetID()

	if(localPlayerID ~= collidingID) then
		msg.senderID:SetVisible{visible=true, fadeTime=0}
		--print("offCollisionPhantom")
	end

end

