require('o_mis')

function onStartup(self)
	registerWithZoneControlObject(self)
end

--------------------------------------------------------------
-- Called when this object is ready to render
--------------------------------------------------------------
function onRenderComponentReady(self, msg)
	-- let the zone control object know we are ready to be instructed
	GAMEOBJ:GetZoneControlID():FireEvent{ senderID=self, args="SceneActorReady" }

end
